IF EXISTS (select 1 from sysobjects where [name] = 'QoS_SystemLoadServerReport' and type = 'P')  
BEGIN
	DROP PROCEDURE EDDSDBO.QoS_SystemLoadServerReport
END
GO
CREATE PROCEDURE EDDSDBO.QoS_SystemLoadServerReport
	/* Grid conditions */
	@SortColumn VARCHAR(50) = 'Score',
	@SortDirection CHAR(4) = 'ASC',
	@TimezoneOffset INT = 0, --Offset to use (in minutes) for UTC dates
	@StartRow INT = 1,
	@EndRow INT = 25,
	/* Filter conditions */
	@SummaryDayHour DATETIME = NULL,
	@Server NVARCHAR(150) = NULL,
	@ServerType NVARCHAR(3) = NULL, --Acceptable values are 'SQL' or 'Web'
	@Score INT = NULL,
	@CPUScore INT = NULL,
	@RAMScore INT = NULL,
	@MemorySignalStateScore INT = NULL,
	@WaitsScore INT = NULL,
	@VirtualLogFilesScore INT = NULL,
	@LatencyScore INT = NULL,
	@IsActiveArrivalRateSample BIT = NULL,
	/* Filter operands */
	@ScoreOperand NVARCHAR(2) = '=',
	@CPUScoreOperand NVARCHAR(2) = '=',
	@RAMScoreOperand NVARCHAR(2) = '=',
	@MemorySignalStateScoreOperand NVARCHAR(2) = '=',
	@WaitsScoreOperand NVARCHAR(2) = '=',
	@VirtualLogFilesScoreOperand NVARCHAR(2) = '=',
	@LatencyScoreOperand NVARCHAR(2) = '=',
	/* Time range */
	@StartHour DATETIME = NULL,
	@EndHour DATETIME = NULL
AS
BEGIN
	--Declarations
	DECLARE @SQL NVARCHAR(MAX) = N'',
		@Where NVARCHAR(MAX) = N'',
		@ServerId INT = NULL,
		@ServerTypeID INT = CASE
			WHEN @ServerType = 'Web' THEN 1
			WHEN @ServerType = 'SQL' THEN 3
		END;

	--Support ArtifactID filtering
	IF (ISNUMERIC(@Server) = 1)
		SET @ServerId = CAST(@Server as int);

	--Prepare string filter inputs
	SET @Server = '%' + @Server + '%';
	
	--Massage start/end dates
	IF (@SummaryDayHour IS NOT NULL)
	BEGIN
		SET @SummaryDayHour = DATEADD(MINUTE, -1 * @TimezoneOffset, @SummaryDayHour);
		IF (@StartHour IS NULL OR @StartHour < @SummaryDayHour)
			SET @StartHour = @SummaryDayHour;
		IF (@EndHour IS NULL OR @SummaryDayHour < @EndHour)
			SET @EndHour = @SummaryDayHour;
	END
	
	-- Filter Sort Params
	IF UPPER(@SortDirection) NOT IN ('ASC','DESC')
	BEGIN
		SET @SortDirection = 'ASC'
	END 
 
	IF LOWER(@SortColumn) NOT IN (N'score', N'summarydayhour', N'server', N'servertype', N'cpuscore', N'rampctscore', N'memorysignalstatescore', N'waitsscore', N'virtuallogfilesscore', N'latencyscore', N'isactiveweeklysample')
	BEGIN
		SET @SortColumn = 'score'
	END
	
	--Handle ambiguous sort columns
	IF (LOWER(@SortColumn) = 'isactiveweeklysample')
		SET @SortColumn = 'IsActiveArrivalRateSample'
	
	--Build SQL
	IF (@Server IS NOT NULL)
		SET @Where += '
		AND (Server LIKE @Server OR ServerArtifactID = @ServerId)';
	IF (@ServerTypeID IS NOT NULL)
		SET @Where += '
		AND ServerType = @ServerTypeID'
	IF (@Score IS NOT NULL)
		SET @Where += '
		AND CAST(Score AS INT) ' + @ScoreOperand + ' @Score';
	IF (@CPUScore IS NOT NULL)
		SET @Where += '
		AND CAST(CPUScore AS INT) ' + @CPUScoreOperand + ' @CPUScore';
	IF (@RAMScore IS NOT NULL)
		SET @Where += '
		AND CAST(RAMPctScore AS INT) ' + @RAMScoreOperand + ' @RAMScore';
	IF (@MemorySignalStateScore IS NOT NULL)
		SET @Where += '
		AND CAST(MemorySignalStateScore AS INT) ' + @MemorySignalStateScoreOperand + ' @MemorySignalStateScore';
	IF (@WaitsScore IS NOT NULL)
		SET @Where += '
		AND CAST(WaitsScore AS INT) ' + @WaitsScoreOperand + ' @WaitsScore';
	IF (@VirtualLogFilesScore IS NOT NULL)
		SET @Where += '
		AND CAST(VirtualLogFilesScore AS INT) ' + @VirtualLogFilesScoreOperand + ' @VirtualLogFilesScore';
	IF (@LatencyScore IS NOT NULL)
		SET @Where += '
		AND CAST(LatencyScore AS INT) ' + @LatencyScoreOperand + ' @LatencyScore';
	IF (@StartHour IS NOT NULL AND @EndHour IS NOT NULL)
		SET @Where += '
		AND [SummaryDayHour] BETWEEN @StartHour AND @EndHour';
	IF (@IsActiveArrivalRateSample IS NOT NULL)
		SET @Where += '
		AND IsActiveArrivalRateSample = @IsActiveArrivalRateSample';
	
	SET @SQL = N'
	DECLARE @Data TABLE
	(
		[RowNumber] INT,
		[TotalRows] INT,
		[ServerArtifactID] INT,
		[Server] NVARCHAR(150),
		[ServerType] NVARCHAR(3),
		[SummaryDayHour] DATETIME,
		[Score] INT,
		[CPUScore] INT,
		[RAMScore] INT,
		[MemorySignalStateScore] INT,
		[MemorySignalStateRatio] INT,
		[Pageouts] INT,
		[WaitsScore] INT,
		[VirtualLogFilesScore] INT,
		[MaxVirtualLogFiles] INT,
		[LatencyScore] INT,
		[HighestLatencyDatabase] NVARCHAR(255),
		[ReadLatencyMs] INT,
		[WriteLatencyMs] INT,
		[IsDataFile] BIT,
		[IsActiveArrivalRateSample] BIT
	);

	DECLARE @totalRows INT = (
		SELECT
			COUNT(*)
		FROM eddsdbo.SystemLoadServerDetail
		WHERE 1=1
		' + ISNULL(@Where, '') + '
	);

	WITH Paging AS
	(
	SELECT
		ROW_NUMBER() OVER (ORDER BY ' + @SortColumn + ' ' + @SortDirection + ') AS RowNumber,
		@totalRows TotalRows,
		[ServerArtifactID],
		[Server],
		CASE
			WHEN [ServerType] = 1 THEN ''Web''
			ELSE ''SQL''
		END [ServerType],
		DATEADD(MINUTE, @TimezoneOffset, [SummaryDayHour]) [SummaryDayHour],
		[Score],
		[CPUScore],
		[RAMPctScore],
		[MemorySignalStateScore],
		[MemorySignalStateRatio],
		[Pageouts],
		[WaitsScore],
		[VirtualLogFilesScore],
		[MaxVirtualLogFiles],
		[LatencyScore],
		[HighestLatencyDatabase],
		[ReadLatencyMs],
		[WriteLatencyMs],
		[IsDataFile],
		[IsActiveArrivalRateSample]
	FROM eddsdbo.SystemLoadServerDetail
	WHERE 1=1
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
		[ServerType],
		[SummaryDayHour],
		[Score],
		[CPUScore],
		[RAMScore],
		[MemorySignalStateScore],
		[MemorySignalStateRatio],
		[Pageouts],
		[WaitsScore],
		[VirtualLogFilesScore],
		[MaxVirtualLogFiles],
		[LatencyScore],
		[HighestLatencyDatabase],
		[ReadLatencyMs],
		[WriteLatencyMs],
		[IsDataFile],
		[IsActiveArrivalRateSample]
	FROM @Data
	
	SELECT TOP 1
		@StartRow AS StartIndex,
		@StartRow + @@ROWCOUNT - 1 AS EndIndex,
		TotalRows AS FilteredCount
	FROM @Data';
	
	PRINT @SQL;
	
	EXEC sp_executesql @SQL,
		N'@StartRow INT, @EndRow INT, @TimezoneOffset INT, @Server NVARCHAR(MAX), @ServerId INT, @ServerTypeID INT, @Score INT, @CPUScore INT, @RAMScore INT, @MemorySignalStateScore INT, @WaitsScore INT, @VirtualLogFilesScore INT, @LatencyScore INT, @IsActiveArrivalRateSample BIT, @StartHour DATETIME, @EndHour DATETIME',
		@StartRow,
		@EndRow,
		@TimezoneOffset,
		@Server,
		@ServerId,
		@ServerTypeID,
		@Score,
		@CPUScore,
		@RAMScore,
		@MemorySignalStateScore,
		@WaitsScore,
		@VirtualLogFilesScore,
		@LatencyScore,
		@IsActiveArrivalRateSample,
		@StartHour,
		@EndHour;
END