--EDDSPerformance
IF EXISTS (select 1 from sysobjects where [name] = 'QoS_RecoveryObjectivesReport' and type = 'P')  
BEGIN
	DROP PROCEDURE eddsdbo.QoS_RecoveryObjectivesReport
END
GO
CREATE PROCEDURE eddsdbo.QoS_RecoveryObjectivesReport
	/* Grid conditions */
	@SortColumn VARCHAR(50) = '(RTOScore + RPOScore)/2',
	@SortDirection CHAR(4) = 'ASC',
	@StartRow INT = 1,
	@EndRow INT = 25,
	/* Filter conditions */
	@Server NVARCHAR(255) = NULL,
	@DBName NVARCHAR(255) = NULL, 
	@RPOScore INT = NULL,
	@RTOScore INT = NULL,
	@PotentialDataLossMinutes INT = NULL,
	@EstimatedTimeToRecoverHours INT = NULL,
	/* Filter operands */
	@RPOScoreOperand NVARCHAR(2) = '=',
	@RTOScoreOperand NVARCHAR(2) = '=',
	@PotentialDataLossMinutesOperand NVARCHAR(2) = '=',
	@EstimatedTimeToRecoverHoursOperand NVARCHAR(2) = '='
AS
BEGIN
	--Declarations
	DECLARE @SQL NVARCHAR(MAX) = N'',
		@Where NVARCHAR(MAX) = N'',
		@ServerId INT,
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
	SET @DBName = '%' + @DBName + '%';
	SET @Server = '%' + @Server + '%';

	-- Filter Sort Params
	IF UPPER(@SortDirection) NOT IN ('ASC','DESC')
	BEGIN
		SET @SortDirection = 'ASC'
	END 
 
	IF LOWER(@SortColumn) NOT IN (N'(rtoscore + rposcore)/2', N'servername', N'dbname', N'potentialdatalossminutes', N'estimatedtimetorecoverhours', N'rposcore', N'rtoscore')
	BEGIN
		SET @SortColumn = '(rtoscore + rposcore)/2'
	END

	--Build SQL
	IF (@Server IS NOT NULL)
		SET @Where += '
		AND (ros.ServerName LIKE @Server OR s.ArtifactId = @ServerId)';
	IF (@DBName IS NOT NULL)
		SET @Where += '
		AND DBName LIKE @DBName'
	IF (@PotentialDataLossMinutes IS NOT NULL)
		SET @Where += '
		AND CAST(PotentialDataLossMinutes AS INT) ' + @PotentialDataLossMinutesOperand + ' @PotentialDataLossMinutes';
	IF (@EstimatedTimeToRecoverHours IS NOT NULL)
		SET @Where += '
		AND CAST(EstimatedTimeToRecoverHours AS INT) ' + @EstimatedTimeToRecoverHoursOperand + ' @EstimatedTimeToRecoverHours';
	IF (@RPOScore IS NOT NULL)
		SET @Where += '
		AND CAST(RPOScore AS INT) ' + @RPOScoreOperand + ' @RPOScore';
	IF (@RTOScore IS NOT NULL)
		SET @Where += '
		AND CAST(RTOScore AS INT) ' + @RTOScoreOperand + ' @RTOScore';
	IF (@includeInvariant = 0)
		SET @Where += '
		AND DBName NOT LIKE ''INV%''';

	SET @SQL = N'
	DECLARE @Data TABLE
	(
		[RowNumber] INT,
		[TotalRows] INT,
		[ServerId] INT,
		[ServerName] NVARCHAR(255),
		[DBName] NVARCHAR(255),
		[RPOScore] INT,
		[RTOScore] INT,
		[PotentialDataLossMinutes] INT,
		[EstimatedTimeToRecoverHours] INT
	);

	DECLARE @totalRows INT = (
		SELECT
			COUNT(*)
		FROM eddsdbo.QoS_RecoveryObjectiveSummary ros WITH(NOLOCK)
		CROSS APPLY (
			SELECT TOP 1 ArtifactID
			FROM eddsdbo.[Server] S WITH(NOLOCK)
			WHERE S.ServerName = ros.ServerName
				AND S.ArtifactID IS NOT NULL
				AND S.ServerTypeID = 3
				AND DeletedOn IS NULL
				AND (IgnoreServer = 0 OR IgnoreServer IS NULL)
		) s
		WHERE 1 = 1
		' + ISNULL(@Where, '') + '
	);
	
	WITH Paging AS
	(
	SELECT
		ROW_NUMBER() OVER (ORDER BY	' + @SortColumn + ' ' + @SortDirection + ') AS RowNumber,
		COUNT(*) OVER () TotalRows,
		s.[ArtifactID] AS ServerId,
		ros.[ServerName],
		ros.[DBName],
		ros.[RPOScore],
		ros.[RTOScore],
		ros.[PotentialDataLossMinutes],
		ros.[EstimatedTimeToRecoverHours]
	FROM eddsdbo.QoS_RecoveryObjectiveSummary ros WITH(NOLOCK)
	CROSS APPLY (
		SELECT TOP 1 ArtifactID
		FROM eddsdbo.[Server] S WITH(NOLOCK)
		WHERE S.ServerName = ros.ServerName
			AND S.ArtifactID IS NOT NULL
			AND S.ServerTypeID = 3
			AND DeletedOn IS NULL
			AND (IgnoreServer = 0 OR IgnoreServer IS NULL)
	) s
	WHERE 1 = 1
	' + ISNULL(@Where, '') + '
	)
	INSERT INTO @Data
	SELECT *
	FROM Paging
	WHERE RowNumber BETWEEN @StartRow AND @EndRow
	
	SELECT
		[RowNumber],
		[ServerId],
		[ServerName],
		[DBName],
		[RPOScore],
		[RTOScore],
		[PotentialDataLossMinutes],
		[EstimatedTimeToRecoverHours]
	FROM @Data
	
	SELECT TOP 1
		@StartRow AS StartIndex,
		@StartRow + @@ROWCOUNT - 1 AS EndIndex,
		TotalRows AS FilteredCount
	FROM @Data';

	PRINT @SQL;
	
	EXEC sp_executesql @SQL,
		N'@StartRow INT, @EndRow INT, @Server NVARCHAR(255), @ServerId INT, @DBName NVARCHAR(255), @RPOScore INT, @RTOScore INT, @PotentialDataLossMinutes INT, @EstimatedTimeToRecoverHours INT',
		@StartRow,
		@EndRow,
		@Server,
		@ServerId,
		@DBName,
		@RPOScore,
		@RTOScore,
		@PotentialDataLossMinutes,
		@EstimatedTimeToRecoverHours
END