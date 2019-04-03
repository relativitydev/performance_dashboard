IF EXISTS (select 1 from sysobjects where [name] = 'QoS_UserExperienceServerReport' and type = 'P')  
BEGIN
	DROP PROCEDURE EDDSDBO.QoS_UserExperienceServerReport
END
GO
CREATE PROCEDURE EDDSDBO.QoS_UserExperienceServerReport
	/* Grid conditions */
	@SortColumn VARCHAR(50) = 'Score',
	@SortDirection CHAR(4) = 'ASC',
	@TimezoneOffset INT = 0, --Offset to use (in minutes) for UTC dates
	@StartRow INT = 1,
	@EndRow INT = 25,
	/* Filter conditions */
	@SummaryDayHour DATETIME = NULL,
	@Server NVARCHAR(150) = NULL,
	@Workspace NVARCHAR(150) = NULL,
	@TotalUsers INT = NULL,
	@TotalSearchAudits INT = NULL,
	@TotalNonSearchAudits INT = NULL,
	@Score INT = NULL,
	@TotalLongRunning INT = NULL,
	@TotalExecutionTime BIGINT = NULL,
	@TotalAudits INT = NULL,
	@IsActiveArrivalRateSample BIT = NULL,
	/* Filter operands */
	@TotalUsersOperand NVARCHAR(2) = '=',
	@TotalSearchOperand NVARCHAR(2) = '=',
	@TotalNonSearchOperand NVARCHAR(2) = '=',
	@ScoreOperand NVARCHAR(2) = '=',
	@TotalLongRunningOperand NVARCHAR(2) = '=',
	@TotalExecutionTimeOperand NVARCHAR(2) = '=',
	@TotalAuditsOperand NVARCHAR(2) = '=',
	/* Time range */
	@StartHour DATETIME = NULL,
	@EndHour DATETIME = NULL
AS
BEGIN
	--Declarations
	DECLARE @SQL NVARCHAR(MAX) = N'',
		@Where NVARCHAR(MAX) = N'',
		@ServerId INT = NULL,
		@WorkspaceId INT = NULL;
		
	-- Filter Sort Params
	IF UPPER(@SortDirection) NOT IN ('ASC','DESC')
	BEGIN
		SET @SortDirection = 'ASC'
	END 
 
	IF LOWER(@SortColumn) NOT IN (N'score', N'summarydayhour', N'server', N'workspace', N'totallongrunning', N'totalusers', N'totalsearchaudits', N'totalnonsearchaudits', N'totalaudits', N'totalexecutiontime', N'isactiveweeklysample')
	BEGIN
		SET @SortColumn = 'Score'
	END
	
	--Handle ambiguous sort columns
	IF (@SortColumn = 'SummaryDayHour')
		SET @SortColumn = 'SS.SummaryDayHour'
	IF (@SortColumn = 'Score')
		SET @SortColumn = 'SS.Score'
	IF (LOWER(@SortColumn) = 'isactiveweeklysample')
		SET @SortColumn = 'IsActiveArrivalRateSample'
	

	--Support ArtifactID filtering
	IF (ISNUMERIC(@Server) = 1)
		SET @ServerId = CAST(@Server as int);
	IF (ISNUMERIC(@Workspace) = 1)
		SET @WorkspaceId = CAST(@Workspace as int);	

	--Prepare string filter inputs
	SET @Server = '%' + @Server + '%';
	SET @Workspace = '%' + REPLACE(@Workspace, '[', '[[]') + '%';
	
	--Massage start/end dates
	IF (@SummaryDayHour IS NOT NULL)
	BEGIN
		SET @SummaryDayHour = DATEADD(MINUTE, -1 * @TimezoneOffset, @SummaryDayHour);
		IF (@StartHour IS NULL OR @StartHour < @SummaryDayHour)
			SET @StartHour = @SummaryDayHour;
		IF (@EndHour IS NULL OR @SummaryDayHour < @EndHour)
			SET @EndHour = @SummaryDayHour;
	END
	
	--Build SQL
	IF (@Server IS NOT NULL)
		SET @Where += '
		AND (Server LIKE @Server OR SS.ServerArtifactID = @ServerId)';
	IF (@Workspace IS NOT NULL)
		SET @Where += '
		AND (Workspace LIKE @Workspace OR CaseArtifactID = @WorkspaceId)'
	IF (@Score IS NOT NULL)
		SET @Where += '
		AND SS.Score ' + @ScoreOperand + ' @Score';
	IF (@TotalUsers IS NOT NULL)
		SET @Where += '
		AND TotalUsers ' + @TotalUsersOperand + ' @TotalUsers';
	IF (@TotalSearchAudits IS NOT NULL)
		SET @Where += '
		AND TotalSearchAudits ' + @TotalSearchOperand + ' @TotalSearchAudits';
	IF (@TotalNonSearchAudits IS NOT NULL)
		SET @Where += '
		AND TotalNonSearchAudits ' + @TotalNonSearchOperand + ' @TotalNonSearchAudits';
	IF (@TotalLongRunning IS NOT NULL)
		SET @Where += '
		AND TotalLongRunning ' + @TotalLongRunningOperand + ' @TotalLongRunning';
	IF (@TotalExecutionTime IS NOT NULL)
		SET @Where += '
		AND TotalExecutionTime ' + @TotalExecutionTimeOperand + ' @TotalExecutionTime';
	IF (@TotalAudits IS NOT NULL)
		SET @Where += '
		AND TotalAudits ' + @TotalAuditsOperand + ' @TotalAudits';
	IF (@StartHour IS NOT NULL AND @EndHour IS NOT NULL)
		SET @Where += '
		AND SS.[SummaryDayHour] BETWEEN @StartHour AND @EndHour';
	IF (@IsActiveArrivalRateSample IS NOT NULL)
		SET @Where += '
		AND IsActiveArrivalRateSample = @IsActiveArrivalRateSample';
	
	SET @SQL = N'
	DECLARE @Data TABLE
	(
		[RowNumber] INT,
		[TotalRows] INT,
		[ServerArtifactId] INT,
		[Server] NVARCHAR(150),
		[CaseArtifactId] INT,
		[Workspace] NVARCHAR(150),
		[TotalUsers] INT,
		[TotalSearchAudits] INT,
		[TotalNonSearchAudits] INT,
		[Score] INT,
		[TotalLongRunning] INT,
		[TotalExecutionTime] BIGINT,
		[TotalAudits] INT,
		[SummaryDayHour] DATETIME,
		[IsActiveArrivalRateSample] BIT
	);

	DECLARE @totalRows INT = (
		SELECT
			COUNT(*)
		FROM eddsdbo.QoS_UserExperienceServerSummary SS WITH(NOLOCK)
		INNER JOIN eddsdbo.Hours H WITH(NOLOCK)
			ON SS.SummaryDayHour = H.HourTimeStamp
		INNER JOIN eddsdbo.Server S WITH(NOLOCK)
			ON SS.ServerArtifactID = S.ArtifactID
		INNER JOIN eddsdbo.QoS_SampleHistoryUX SH WITH(NOLOCK)
			ON S.ServerID = SH.ServerId
			AND H.Id = SH.HourId
		WHERE H.HourTimeStamp > DATEADD(dd, -90, getutcdate())
		' + ISNULL(@Where, '') + '
	);
	
	WITH Paging AS
	(
	SELECT
		ROW_NUMBER() OVER (ORDER BY	' + @SortColumn + ' ' + @SortDirection + ') AS RowNumber,
		@totalRows TotalRows,
		SS.ServerArtifactID,
		[Server],
		[CaseArtifactID],
		[Workspace],
		[TotalUsers],
		[TotalSearchAudits],
		[TotalNonSearchAudits],
		SS.[Score],
		TotalLongRunning,
		TotalExecutionTime,
		TotalAudits,
		DATEADD(MINUTE, @TimezoneOffset, SS.[SummaryDayHour]) [SummaryDayHour],
		[IsActiveArrivalRateSample]
	FROM eddsdbo.QoS_UserExperienceServerSummary SS WITH(NOLOCK)
	INNER JOIN eddsdbo.Hours H WITH(NOLOCK)
		ON SS.SummaryDayHour = H.HourTimeStamp
	INNER JOIN eddsdbo.Server S WITH(NOLOCK)
		ON SS.ServerArtifactID = S.ArtifactID
	INNER JOIN eddsdbo.QoS_SampleHistoryUX SH WITH(NOLOCK)
		ON S.ServerID = SH.ServerId
		AND H.Id = SH.HourId
	WHERE H.HourTimeStamp > DATEADD(dd, -90, getutcdate())
	' + ISNULL(@Where, '') + '
	)
	INSERT INTO @Data
	SELECT *
	FROM Paging
	WHERE RowNumber BETWEEN @StartRow AND @EndRow
	
	SELECT
		[RowNumber],
		[ServerArtifactId],
		[Server],
		[CaseArtifactId],
		[Workspace],
		[TotalUsers],
		[TotalSearchAudits],
		[TotalNonSearchAudits],
		[Score],
		[TotalLongRunning],
		[TotalExecutionTime],
		[TotalAudits],
		[SummaryDayHour],	
		[IsActiveArrivalRateSample]
	FROM @Data
	
	SELECT TOP 1
		@StartRow AS StartIndex,
		@StartRow + @@ROWCOUNT - 1 AS EndIndex,
		TotalRows AS FilteredCount
	FROM @Data';
	
	PRINT @SQL;
	
	EXEC sp_executesql @SQL,
		N'@StartRow INT, @EndRow INT, @TimezoneOffset INT, @Server NVARCHAR(MAX), @ServerId INT, @Workspace NVARCHAR(MAX), @WorkspaceId INT, @Score INT, @TotalUsers INT, @TotalSearchAudits INT, @TotalNonSearchAudits INT, @TotalAudits INT, @TotalLongRunning INT, @TotalExecutionTime BIGINT, @IsActiveArrivalRateSample BIT, @StartHour DATETIME, @EndHour DATETIME',
		@StartRow,
		@EndRow,
		@TimezoneOffset,
		@Server,
		@ServerId,
		@Workspace,
		@WorkspaceId,
		@Score,
		@TotalUsers,
		@TotalSearchAudits,
		@TotalNonSearchAudits,
		@TotalAudits,
		@TotalLongRunning,
		@TotalExecutionTime,
		@IsActiveArrivalRateSample,
		@StartHour,
		@EndHour;
END