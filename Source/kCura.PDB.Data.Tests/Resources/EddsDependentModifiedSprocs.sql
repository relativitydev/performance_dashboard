-- EDDSPerformance
IF EXISTS (select 1 from sysobjects where [name] = 'RecoverabilityGapsReport' and type = 'P')  
BEGIN
	DROP PROCEDURE eddsdbo.RecoverabilityGapsReport
END
GO

CREATE PROCEDURE eddsdbo.RecoverabilityGapsReport
	/* Grid conditions */
	@SortColumn VARCHAR(50) = 'GapSize',
	@SortDirection CHAR(4) = 'DESC',
	@TimezoneOffset INT = 0, --Offset to use (in minutes) for UTC dates
	@StartRow INT = 1,
	@EndRow INT = 25,
	/* Filter conditions */
	@Server NVARCHAR(150) = NULL,
	@Database NVARCHAR(150) = NULL,
	@LastActivityDate DATETIME = NULL,
	@ActivityType BIT = NULL,
	@ResolutionDate DATETIME = NULL,
	@GapSize INT = NULL,
	/* Filter operands */
	@GapSizeOperand NVARCHAR(2) = '=',
	/* Time range */
	@StartHour DATETIME = NULL,
	@EndHour DATETIME = NULL
AS
BEGIN
	--Declarations
	DECLARE @SQL NVARCHAR(MAX) = N'',
		@Where NVARCHAR(MAX) = N'',
		@ServerId INT = NULL,
		@sqlTimezoneOffset int = DATEDIFF(MINUTE, getutcdate(), getdate()), --Difference in minutes between SQL local time and UTC
		@includeInvariant BIT = (
			CASE WHEN ISNULL((
				SELECT TOP 1 [Value]
				FROM eddsdbo.Configuration WITH(NOLOCK)
				WHERE [Section] = 'kCura.PDB'
					AND [Name] = 'ShowInvariantBackupDBCCHistory'
				), 'False') = 'True' THEN 1
				ELSE 0
			END
		);

	--Support ArtifactID filtering
	IF (ISNUMERIC(@Server) = 1)
		SET @ServerId = CAST(@Server as int);

	--Prepare string filter inputs
	SET @Server = '%' + @Server + '%';
	SET @Database = '%' + @Database + '%';
	
	--Massage start/end dates
	IF (@ResolutionDate IS NOT NULL)
	BEGIN
		SET @ResolutionDate = DATEADD(MINUTE, @sqlTimezoneOffset - @TimezoneOffset, @ResolutionDate);
	END
	
	IF (@LastActivityDate IS NOT NULL)
	BEGIN
		SET @LastActivityDate = DATEADD(MINUTE, @sqlTimezoneOffset - @TimezoneOffset, @LastActivityDate);
	END
	
	-- Filter Sort Params
	IF UPPER(@SortDirection) NOT IN ('ASC','DESC')
	BEGIN
		SET @SortDirection = 'DESC'
	END 
 
	IF LOWER(@SortColumn) NOT IN (N'servername', N'databasename', N'isbackup', N'lastactivitydate', N'resolutiondate', N'gapsize')
	BEGIN
		SET @SortColumn = 'GapSize'
	END

	--Handle ambiguous sort columns
	IF (LOWER(@SortColumn) = 'lastactivitydate')
		SET @SortColumn = 'LastActivity'
	IF (LOWER(@SortColumn) = 'resolutiondate')
		SET @SortColumn = 'GapResolutionDate'
	IF (LOWER(@SortColumn) = 'isbackup')
		SET @SortColumn = 'ActivityType'
	IF (LOWER(@SortColumn) = 'databasename')
		SET @SortColumn = 'db.[Name]'
	
	--Build SQL
	IF (@Server IS NOT NULL)
		SET @Where += '
		AND (s.ServerName LIKE @Server OR s.ArtifactID = @ServerId)';
	IF (@Database IS NOT NULL)
		SET @Where += '
		AND db.[Name] LIKE @Database'
	IF (@ActivityType IS NOT NULL) -- TODO investigate
		SET @Where += '
		AND IsBackup = @ActivityType'
	IF (@LastActivityDate IS NOT NULL)
		SET @Where += '
		AND LastActivity = @LastActivityDate'
	IF (@ResolutionDate IS NOT NULL)
		SET @Where += '
		AND GapResolutionDate = @ResolutionDate'
	IF (@GapSize IS NOT NULL)
		SET @Where += '
		AND GapSize ' + @GapSizeOperand + ' @GapSize';
	IF (@StartHour IS NOT NULL AND @EndHour IS NOT NULL)
		SET @Where += '
		AND ISNULL(GapResolutionDate, @EndHour) BETWEEN @StartHour AND @EndHour';
	IF (@includeInvariant = 0) -- TODO investigate
		SET @Where += '
		AND db.WorkspaceId >= -1';

	SET @SQL = N'
	DECLARE @Data TABLE
	(
		[RowNumber] INT,
		[TotalRows] INT,
		[ServerArtifactId] INT,
		[ServerName] NVARCHAR(150),
		[DatabaseName] NVARCHAR(50),
		[WorkspaceName] NVARCHAR(50),
		[IsBackup] BIT,
		[LastActivityDate] DATETIME,
		[ResolutionDate] DATETIME,
		[GapSize] INT
	);

	DECLARE @totalRows INT = (
		SELECT
			COUNT(*)
		FROM eddsdbo.[Reports_RecoveryGaps] as rg WITH(NOLOCK)
        INNER JOIN eddsdbo.[Databases] as db WITH(NOLOCK) on rg.DatabaseId = db.Id
	    INNER JOIN eddsdbo.[Server] as s WITH(NOLOCK) on db.ServerId = s.ServerId
		WHERE 1 = 1
		' + ISNULL(@Where, '') + '
	);
	
	WITH Paging AS
	(
	SELECT
		ROW_NUMBER() OVER (ORDER BY	' + @SortColumn + ' ' + @SortDirection + ') AS RowNumber,
		@totalRows TotalRows,
		s.[ArtifactID] as [ServerArtifactID],
		s.[ServerName],
		db.[Name] as [DatabaseName],
		''Testing'' as [WorkspaceName],
		CASE WHEN [ActivityType] = 1 THEN 0 ELSE 1 END as [IsBackup], 
		DATEADD(MINUTE, @TimezoneOffset - @sqlTimezoneOffset, [LastActivity]) LastActivityDate,
		DATEADD(MINUTE, @TimezoneOffset - @sqlTimezoneOffset, [GapResolutionDate]) ResolutionDate,
		[GapSize]
	FROM eddsdbo.[Reports_RecoveryGaps] as rg WITH(NOLOCK)
	INNER JOIN eddsdbo.[Databases] as db WITH(NOLOCK) on rg.DatabaseId = db.Id
	INNER JOIN eddsdbo.[Server] as s WITH(NOLOCK) on db.ServerId = s.ServerId
	WHERE 1 = 1
	' + ISNULL(@Where, '') + '
	)
	INSERT INTO @Data
	SELECT *
	FROM Paging
	WHERE RowNumber BETWEEN @StartRow AND @EndRow
	
	SELECT
		[RowNumber],
		[ServerArtifactID],
		[ServerName],
		[DatabaseName],
		[WorkspaceName],
		[IsBackup],
		[LastActivityDate],
		[ResolutionDate],
		[GapSize]
	FROM @Data
	
	SELECT TOP 1
		@StartRow AS StartIndex,
		@StartRow + @@ROWCOUNT - 1 AS EndIndex,
		TotalRows AS FilteredCount
	FROM @Data';

	PRINT @SQL;
	
	EXEC sp_executesql @SQL,
		N'@StartRow INT, @EndRow INT, @TimezoneOffset INT, @sqlTimezoneOffset INT, @Server NVARCHAR(MAX), @ServerId INT, @Database NVARCHAR(50), @ActivityType BIT, @LastActivityDate DATETIME, @ResolutionDate DATETIME, @GapSize INT, @StartHour DATETIME, @EndHour DATETIME',
		@StartRow,
		@EndRow,
		@TimezoneOffset,
		@sqlTimezoneOffset,
		@Server,
		@ServerId,
		@Database,
		@ActivityType,
		@LastActivityDate,
		@ResolutionDate,
		@GapSize,
		@StartHour,
		@EndHour;
END
GO