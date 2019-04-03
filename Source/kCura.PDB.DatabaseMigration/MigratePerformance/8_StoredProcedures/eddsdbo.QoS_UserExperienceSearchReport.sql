IF EXISTS (select 1 from sysobjects where [name] = 'QoS_UserExperienceSearchReport' and type = 'P')  
BEGIN
	DROP PROCEDURE EDDSDBO.QoS_UserExperienceSearchReport
END
GO
CREATE PROCEDURE EDDSDBO.QoS_UserExperienceSearchReport
	/* Grid conditions */
	@SortColumn VARCHAR(50) = 'PercentLongRunning',
	@SortDirection CHAR(4) = 'DESC',
	@TimezoneOffset INT = 0, --Offset to use (in minutes) for UTC dates
	@StartRow INT = 1,
	@EndRow INT = 25,
	/* Filter conditions */
	@SummaryDayHour DATETIME = NULL,
	@CaseArtifactId INT,
	@Search NVARCHAR(MAX) = NULL,
	@User NVARCHAR(200) = NULL,
	@TotalRunTime BIGINT = NULL,
	@AverageRunTime INT = NULL,
	@TotalRuns INT = NULL,
	@PercentLongRunning INT = NULL,
	@QoSHourID BIGINT = NULL,
	@IsComplex BIT = NULL,
	@IsActiveArrivalRateSample BIT = NULL,
	/* Filter operands */
	@PercentLongRunningOperand NVARCHAR(2) = '=',
	@TotalRunTimeOperand NVARCHAR(2) = '=',
	@AverageRunTimeOperand NVARCHAR(2) = '=',
	@TotalRunsOperand NVARCHAR(2) = '=',
	@QoSHourIDOperand NVARCHAR(2) = '=',
	/* Time range */
	@StartHour DATETIME = NULL,
	@EndHour DATETIME = NULL
AS
BEGIN
	--Declarations
	DECLARE @SQL NVARCHAR(MAX) = N'',
		@Where NVARCHAR(MAX) = N'',
		@SearchId INT = NULL,
		@UserId INT = NULL;
		
	-- Filter Sort Params
	IF UPPER(@SortDirection) NOT IN ('ASC','DESC')
	BEGIN
		SET @SortDirection = 'DESC'
	END 
 
	IF LOWER(@SortColumn) NOT IN (N'search', N'summarydayhour', N'user', N'percentlongrunning', N'iscomplex', N'totalruntime', N'averageruntime', N'totalruns', N'qoshourid', N'isactiveweeklysample')
	BEGIN
		SET @SortColumn = 'PercentLongRunning'
	END
	
	--Handle ambiguous sort columns
	IF (@SortColumn = 'SummaryDayHour')
		SET @SortColumn = 'SS.SummaryDayHour'
	IF (@SortColumn = 'QoSHourID')
		SET @SortColumn = 'SS.QoSHourID';
	IF (@SortColumn = 'User')
		SET @SortColumn = '[User]';
	IF (LOWER(@SortColumn) = 'isactiveweeklysample')
		SET @SortColumn = 'IsActiveArrivalRateSample'

	--Support ArtifactID filtering
	IF (ISNUMERIC(@Search) = 1)
		SET @SearchId = CAST(@Search as int);
	IF (ISNUMERIC(@User) = 1)
		SET @UserId = CAST(@User as int);

	--Prepare string filter inputs
	SET @Search = '%' + REPLACE(@Search, '[', '[[]') + '%';
	SET @User = '%' + REPLACE(@User, '[', '[[]') + '%';
	
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
	IF (@Search IS NOT NULL)
		SET @Where += '
		AND (Search LIKE @Search OR SearchArtifactID = @SearchId)'
	IF (@User IS NOT NULL)
		SET @Where += '
		AND ([User] LIKE @User OR UserArtifactID = @UserId)';
	IF (@TotalRunTime IS NOT NULL)
		SET @Where += '
		AND TotalRunTime ' + @TotalRunTimeOperand + ' @TotalRunTime';
	IF (@AverageRunTime IS NOT NULL)
		SET @Where += '
		AND AverageRunTime ' + @AverageRunTimeOperand + ' @AverageRunTime';
	IF (@TotalRuns IS NOT NULL)
		SET @Where += '
		AND TotalRuns ' + @TotalRunsOperand + ' @TotalRuns';
	IF (@PercentLongRunning IS NOT NULL)
		SET @Where += '
		AND PercentLongRunning ' + @PercentLongRunningOperand + ' @PercentLongRunning';
	IF (@IsComplex IS NOT NULL)
		SET @Where += '
		AND IsComplex = @IsComplex';
	IF (@QoSHourID IS NOT NULL)
		SET @Where += '
		AND SS.QoSHourID ' + @QoSHourIDOperand + ' @QoSHourID'
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
		[CaseArtifactID] INT,
		[SearchArtifactID] INT,
		[Search] NVARCHAR(150),
		[LastAuditID] BIGINT,
		[UserArtifactID] INT,
		[User] NVARCHAR(150),
		[TotalRunTime] BIGINT,
		[AverageRunTime] INT,
		[TotalRuns] INT,
		[PercentLongRunning] INT,
		[IsComplex] BIT,
		[SummaryDayHour] DATETIME,
		[QoSHourID] BIGINT,
		[IsActiveArrivalRateSample] BIT
	);
	
	DECLARE @totalRows INT = (
		SELECT
			COUNT(*)
		FROM eddsdbo.QoS_UserExperienceSearchSummary SS
		INNER JOIN eddsdbo.Hours H WITH(NOLOCK)
		ON SS.SummaryDayHour = H.HourTimeStamp and h.Status != 4
		INNER JOIN eddsdbo.QoS_SampleHistoryUX SH WITH(NOLOCK)
		ON SS.ServerID = SH.ServerID
		AND H.Id = SH.HourId
		WHERE CaseArtifactID = @CaseArtifactId
		AND H.HourTimeStamp > DATEADD(dd, -90, getutcdate())
		' + ISNULL(@Where, '') + '
	);

	WITH Paging AS
	(
	SELECT
		ROW_NUMBER() OVER (ORDER BY ' + @SortColumn + ' ' + @SortDirection + ') AS RowNumber,
		@totalRows TotalRows,
		[CaseArtifactID],
		[SearchArtifactID],
		[Search],
		[LastAuditID],
		[UserArtifactID],
		[User],
		[TotalRunTime],
		[AverageRunTime],
		TotalRuns,
		PercentLongRunning,
		[IsComplex],
		DATEADD(MINUTE, @TimezoneOffset, SS.[SummaryDayHour]) [SummaryDayHour],
		SS.QoSHourID,
		[IsActiveArrivalRateSample]
	FROM eddsdbo.QoS_UserExperienceSearchSummary SS
	INNER JOIN eddsdbo.Hours H
		ON SS.SummaryDayHour = H.HourTimeStamp and h.Status != 4
	INNER JOIN eddsdbo.QoS_SampleHistoryUX SH WITH(NOLOCK)
		ON SS.ServerID = SH.ServerID
		AND H.Id = SH.HourId
	WHERE CaseArtifactID = @CaseArtifactId
	AND H.HourTimeStamp > DATEADD(dd, -90, getutcdate())
	' + ISNULL(@Where, '') + '
	)
	INSERT INTO @Data
	SELECT *
	FROM Paging
	WHERE RowNumber BETWEEN @StartRow AND @EndRow
	
	SELECT
		[RowNumber],
		[CaseArtifactID],
		[SearchArtifactID],
		[Search],
		[LastAuditID],
		[UserArtifactID],
		[User],
		[TotalRunTime],
		[AverageRunTime],
		[TotalRuns],
		[PercentLongRunning],
		[IsComplex],
		[SummaryDayHour],
		[QoSHourID],
		[IsActiveArrivalRateSample]
	FROM @Data
	
	SELECT TOP 1 @StartRow AS StartIndex,
		@StartRow + @@ROWCOUNT - 1 AS EndIndex,
		[TotalRows] AS FilteredCount
	FROM @Data';
	
	PRINT @SQL;
	
	EXEC sp_executesql @SQL,
		N'@StartRow INT, @EndRow INT, @TimezoneOffset INT, @CaseArtifactId INT, @Search NVARCHAR(MAX), @SearchId INT, @User NVARCHAR(MAX), @UserId INT, @TotalRunTime BIGINT, @AverageRunTime INT, @TotalRuns INT, @PercentLongRunning INT, @IsComplex BIT, @QoSHourID BIGINT, @IsActiveArrivalRateSample BIT, @StartHour DATETIME, @EndHour DATETIME',
		@StartRow,
		@EndRow,
		@TimezoneOffset,
		@CaseArtifactId,
		@Search,
		@SearchId,
		@User,
		@UserId,
		@TotalRunTime,
		@AverageRunTime,
		@TotalRuns,
		@PercentLongRunning,
		@IsComplex,
		@QoSHourID,
		@IsActiveArrivalRateSample,
		@StartHour,
		@EndHour;
END