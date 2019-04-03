IF EXISTS (select 1 from sysobjects where [name] = 'QoS_SystemLoadWaitsReport' and type = 'P')  
BEGIN
	DROP PROCEDURE EDDSDBO.QoS_SystemLoadWaitsReport
END
GO
CREATE PROCEDURE EDDSDBO.QoS_SystemLoadWaitsReport
	/* Grid conditions */
	@SortColumn VARCHAR(50) = 'WaitsScore',
	@SortDirection CHAR(4) = 'ASC',
	@TimezoneOffset INT = 0, --Offset to use (in minutes) for UTC dates
	@StartRow INT = 1,
	@EndRow INT = 25,
	/* Filter conditions */
	@SummaryDayHour DATETIME = NULL,
	@Server NVARCHAR(150) = NULL,
	@Score INT = NULL,
	@SignalWaitsRatio INT = NULL,
	@WaitType NVARCHAR(60) = NULL,
	@SignalWaitTime BIGINT = NULL,
	@TotalWaitTime BIGINT = NULL,
	@IsPoisonWait BIT = NULL,
	@IsActiveArrivalRateSample BIT = NULL,
	@PercentOfCPUThreshold decimal(18,2) = null,
	@DifferentialWaitingTasksCount bigint = null,
	/* Filter operands */
	@ScoreOperand NVARCHAR(2) = '=',
	@SignalWaitsRatioOperand NVARCHAR(2) = '=',
	@SignalWaitTimeOperand NVARCHAR(2) = '=',
	@TotalWaitTimeOperand NVARCHAR(2) = '=',
	@PercentOfCPUThresholdOperand NVARCHAR(2) = '=',
	@DifferentialWaitingTasksCountOperand NVARCHAR(2) = '=',
	/* Time range */
	@StartHour DATETIME = NULL,
	@EndHour DATETIME = NULL
AS
BEGIN
	--Declarations
	DECLARE @SQL NVARCHAR(MAX) = N'',
		@Where NVARCHAR(MAX) = N'',
		@ServerId INT = NULL;

	-- Filter Sort Params
	IF UPPER(@SortDirection) NOT IN ('ASC','DESC')
	BEGIN
		SET @SortDirection = 'ASC'
	END 
 
	IF LOWER(@SortColumn) NOT IN (N'waitsscore', N'summarydayhour', N'servername', N'signalwaitsratio', N'processortimeutilization', N'waittype', N'differentialsignalwaitms', N'differentialwaitms', N'waitingtaskcount', N'ispoisonwait', N'isactiveweeklysample')
	BEGIN
		SET @SortColumn = 'WaitsScore'
	END

	--Handle ambiguous sort columns
	IF (@SortColumn = 'SummaryDayHour')
		SET @SortColumn = 'h.HourTimeStamp'
	IF (@SortColumn = 'SignalWaitsRatio')
		SET @SortColumn = 'sls.SignalWaitsRatio'
	if(@SortColumn = 'ProcessorTimeUtilization')
		SET @SortColumn = 'PercentOfCPUThreshold'
	if(@SortColumn = 'WaitingTaskCount')	
		SET @SortColumn = 'DifferentialWaitingTasksCount'
	IF (LOWER(@SortColumn) = 'isactiveweeklysample')
		SET @SortColumn = 'IsActiveArrivalRateSample'
	IF (LOWER(@SortColumn) = 'servername')
		SET @SortColumn = 'ws.ServerName'

	--Support ArtifactID filtering
	IF (ISNUMERIC(@Server) = 1)
		SET @ServerId = CAST(@Server as int);

	--Prepare string filter inputs
	SET @Server = '%' + @Server + '%';
	SET @WaitType = '%' + @WaitType + '%';
	
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
		AND (sh.ServerId = @ServerId OR ws.[ServerName] LIKE @Server)'
	IF (@Score IS NOT NULL)
		SET @Where += '
		AND CAST(WaitsScore as int) ' + @ScoreOperand + ' @Score'
	IF (@SignalWaitsRatio IS NOT NULL)
		SET @Where += '
		AND CAST(SignalWaitsRatio * 100 as int) ' + @SignalWaitsRatioOperand + ' @SignalWaitsRatio'
	IF (@WaitType IS NOT NULL)
		SET @Where += '
		AND WaitType LIKE @WaitType'
	IF (@SignalWaitTime IS NOT NULL)
		SET @Where += '
		AND DifferentialSignalWaitMs ' + @SignalWaitTimeOperand + ' @SignalWaitTime'
	IF (@TotalWaitTime IS NOT NULL)
		SET @Where += '
		AND DifferentialWaitMs ' + @TotalWaitTimeOperand + ' @TotalWaitTime'
	IF (@IsPoisonWait IS NOT NULL)
		SET @Where += '
		AND IsPoisonWait = @IsPoisonWait'
	IF (@StartHour IS NOT NULL AND @EndHour IS NOT NULL)
		SET @Where += '
		AND h.[HourTimeStamp] BETWEEN @StartHour AND @EndHour';
	IF (@IsActiveArrivalRateSample IS NOT NULL)
		SET @Where += '
		AND IsActiveArrivalRateSample = @IsActiveArrivalRateSample'
	IF (@PercentOfCPUThreshold is not null)
		SET @Where += '
		AND PercentOfCPUThreshold ' + @PercentOfCPUThresholdOperand + ' @PercentOfCPUThreshold'
	IF (@DifferentialWaitingTasksCount is not null)
		SET @Where += '
		AND DifferentialWaitingTasksCount ' + @DifferentialWaitingTasksCountOperand + ' @DifferentialWaitingTasksCount'

	SET @SQL = N'
	DECLARE @Data TABLE
	(
		[RowNumber] INT,
		[TotalRows] INT,
		[SummaryDayHour] DATETIME,
		[ServerArtifactID] INT,
		[Server] NVARCHAR(150),
		[WaitsScore] INT,
		[SignalWaitsRatio] INT,
		[WaitType] NVARCHAR(60),
		[SignalWaitTime] BIGINT,
		[TotalWaitTime] BIGINT,
		[IsPoisonWait] BIT,
		[IsActiveArrivalRateSample] BIT,
		[PercentOfCPUThreshold] decimal(18,2),
		[DifferentialWaitingTasksCount] bigint
	);
	
	DECLARE @totalRows INT = (
		SELECT
			COUNT(*)
		FROM eddsdbo.QoS_SampleHistoryUX sh WITH(NOLOCK)
		INNER JOIN eddsdbo.Server s WITH(NOLOCK)
		ON sh.ServerId = s.ServerID
		INNER JOIN eddsdbo.Hours h WITH(NOLOCK)
		ON sh.HourId = h.Id and h.Status != 4
		INNER JOIN eddsdbo.QoS_SystemLoadSummary sls WITH(NOLOCK)
		ON h.HourTimeStamp = sls.SummaryDayHour
			AND s.ArtifactID = sls.ServerArtifactID
		INNER JOIN eddsdbo.QoS_WaitSummary ws WITH(NOLOCK)
		ON h.HourTimeStamp = ws.SummaryDayHour 
			AND s.ArtifactID = ws.ServerArtifactID
		INNER JOIN eddsdbo.QoS_WaitDetail wd WITH(NOLOCK)
		ON ws.WaitSummaryID = wd.WaitSummaryID
		INNER JOIN eddsdbo.QoS_Waits w WITH(NOLOCK)
		ON wd.WaitTypeID = w.WaitTypeID
		WHERE h.HourTimeStamp > DATEADD(dd, -90, getutcdate())
		' + ISNULL(@Where, '') + '
	);

	WITH Paging AS
	(
	SELECT
		ROW_NUMBER() OVER (ORDER BY ' + @SortColumn + ' ' + @SortDirection + ') AS RowNumber,
		@totalRows AS TotalRows,
		DATEADD(MINUTE, @TimezoneOffset, h.HourTimeStamp) SummaryDayHour,
		s.ArtifactID,
		ws.ServerName,
		CAST(sls.WaitsScore as int) WaitsScore,
		CAST(ws.SignalWaitsRatio * 100 as int) SignalWaitsRatio,
		w.WaitType,
		wd.DifferentialSignalWaitMs,
		wd.DifferentialWaitMs,
		w.IsPoisonWait,
		sh.IsActiveArrivalRateSample,
		ws.PercentOfCPUThreshold,
		wd.DifferentialWaitingTasksCount
	FROM eddsdbo.QoS_SampleHistoryUX sh WITH(NOLOCK)
	INNER JOIN eddsdbo.Server s WITH(NOLOCK)
	ON sh.ServerId = s.ServerID
	INNER JOIN eddsdbo.Hours h WITH(NOLOCK)
	ON sh.HourId = h.Id and h.Status != 4
	INNER JOIN eddsdbo.QoS_SystemLoadSummary sls WITH(NOLOCK)
	ON h.HourTimeStamp = sls.SummaryDayHour
		AND s.ArtifactID = sls.ServerArtifactID
	INNER JOIN eddsdbo.QoS_WaitSummary ws WITH(NOLOCK)
	ON h.HourTimeStamp = ws.SummaryDayHour
		AND s.ArtifactID = ws.ServerArtifactID
	INNER JOIN eddsdbo.QoS_WaitDetail wd WITH(NOLOCK)
	ON ws.WaitSummaryID = wd.WaitSummaryID
	INNER JOIN eddsdbo.QoS_Waits w WITH(NOLOCK)
	ON wd.WaitTypeID = w.WaitTypeID
	WHERE h.HourTimeStamp > DATEADD(dd, -90, getutcdate())
	' + ISNULL(@Where, '') + '
	)
	INSERT INTO @Data
	SELECT *
	FROM Paging
	WHERE RowNumber BETWEEN @StartRow AND @EndRow
	
	SELECT
		[RowNumber],
		[SummaryDayHour],
		[ServerArtifactID],
		[Server],
		[WaitsScore],
		[SignalWaitsRatio],
		[WaitType],
		[SignalWaitTime],
		[TotalWaitTime],
		[IsPoisonWait],
		[IsActiveArrivalRateSample],
		[PercentOfCPUThreshold],
		[DifferentialWaitingTasksCount]
	FROM @Data
	
	SELECT TOP 1 @StartRow AS StartIndex,
		@StartRow + @@ROWCOUNT - 1 AS EndIndex,
		[TotalRows] AS FilteredCount
	FROM @Data';
	
	PRINT @SQL;
	
	EXEC sp_executesql @SQL,
		N'@StartRow INT, @EndRow INT, @TimezoneOffset INT, @ServerId INT, @Server NVARCHAR(150), @Score INT, @SignalWaitsRatio INT, @WaitType NVARCHAR(60), @SignalWaitTime BIGINT, @TotalWaitTime BIGINT, @IsPoisonWait BIT, @IsActiveArrivalRateSample BIT, @PercentOfCPUThreshold decimal(18,2), @DifferentialWaitingTasksCount bigint, @StartHour DATETIME, @EndHour DATETIME',
		@StartRow,
		@EndRow,
		@TimezoneOffset,
		@ServerId,
		@Server,
		@Score,
		@SignalWaitsRatio,
		@WaitType,
		@SignalWaitTime,
		@TotalWaitTime,
		@IsPoisonWait,
		@IsActiveArrivalRateSample,
		@PercentOfCPUThreshold,
		@DifferentialWaitingTasksCount,
		@StartHour,
		@EndHour;
		
	
END