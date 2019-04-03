IF EXISTS (select 1 from sysobjects where [name] = 'QoS_UptimeReport' and type = 'P')  
BEGIN
	DROP PROCEDURE EDDSDBO.QoS_UptimeReport
END
GO

CREATE PROCEDURE EDDSDBO.QoS_UptimeReport
	/* Grid conditions */
	@SortColumn VARCHAR(50) = 'Uptime ASC, SummaryDayHour', --Lowest uptime first, then most recent
	@SortDirection CHAR(4) = 'DESC',
	@TimezoneOffset INT = 0, --Offset to use (in minutes) for UTC dates
	@StartRow INT = 1,
	@EndRow INT = 25,
	/* Filter conditions */
	@SummaryDayHour DATETIME = NULL,
	@Score INT = NULL,
	@Status VARCHAR(30) = NULL, --'Accessible', 'All Web Servers Down', 'SQL/Agent Servers Down'
	@Uptime DECIMAL(5,2) = NULL,
	/* Filter operands */
	@ScoreOperand NVARCHAR(2) = '=',
	@UptimeOperand NVARCHAR(2) = '=',
	/* Time range */
	@StartHour DATETIME = NULL,
	@EndHour DATETIME = NULL
AS
BEGIN
	--Declarations
	DECLARE @SQL NVARCHAR(MAX) = N'',
		@Where NVARCHAR(MAX) = N'';
	
	-- Filter Sort Params
	IF UPPER(@SortDirection) NOT IN ('ASC','DESC')
	BEGIN
		SET @SortDirection = 'DESC'
	END 
 
	IF LOWER(@SortColumn) NOT IN (N'uptime asc, summarydayhour', N'summarydayhour', N'score', N'status', N'uptime')
	BEGIN
		SET @SortColumn = 'Uptime ASC, SummaryDayHour'
	END
	
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
	IF (@Score IS NOT NULL)
		SET @Where += '
		AND Score ' + @ScoreOperand + ' @Score';
	IF (@Status IS NOT NULL)
		SET @Where += '
		AND [Status] = @Status';
	IF (@Uptime IS NOT NULL)
		SET @Where += '
		AND Uptime ' + @UptimeOperand + ' @Uptime';
	IF (@StartHour IS NOT NULL AND @EndHour IS NOT NULL)
		SET @Where += '
		AND [SummaryDayHour] BETWEEN @StartHour AND @EndHour';
	
	SET @SQL = N'
	DECLARE @Data TABLE
	(
		[RowNumber] INT,
		[TotalRows] INT,
		[Score] INT,
		[Status] VARCHAR(30),
		[Uptime] DECIMAL(5,2),
		[SummaryDayHour] DATETIME,
		[AffectedByMaintenanceWindow] BIT
	);

	DECLARE @totalRows INT = (
		SELECT
			COUNT(*)
		FROM eddsdbo.UptimeDetail WITH(NOLOCK)
		WHERE SummaryDayHour > DATEADD(dd, -90, getutcdate())
		' + ISNULL(@Where, '') + '
	);
	
	WITH Paging AS
	(
	SELECT
		ROW_NUMBER() OVER (ORDER BY ' + @SortColumn + ' ' + @SortDirection + ') AS [RowNumber],
		@totalRows AS [TotalRows],
		[Score],
		[Status],
		[Uptime],
		DATEADD(MINUTE, @TimezoneOffset, SummaryDayHour) AS [SummaryDayHour],
		[AffectedByMaintenanceWindow]
	FROM eddsdbo.UptimeDetail WITH(NOLOCK)
	WHERE SummaryDayHour > DATEADD(dd, -90, getutcdate())
	' + ISNULL(@Where, '') + '
	)
	INSERT INTO @Data
	SELECT *
	FROM Paging
	WHERE RowNumber BETWEEN @StartRow AND @EndRow
	
	SELECT
		[RowNumber],
		[Score],
		[Status],
		[Uptime],
		[SummaryDayHour],
		[AffectedByMaintenanceWindow]
	FROM @Data
	
	SELECT TOP 1
		@StartRow AS StartIndex,
		@StartRow + @@ROWCOUNT - 1 AS EndIndex, 
		TotalRows AS FilteredCount
	FROM @Data';
	
	PRINT @SQL;
	
	EXEC sp_executesql @SQL,
		N'@StartRow INT, @EndRow INT, @TimezoneOffset INT, @Score INT, @Status VARCHAR(30), @Uptime DECIMAL(5,2), @StartHour DATETIME, @EndHour DATETIME',
		@StartRow,
		@EndRow,
		@TimezoneOffset,
		@Score,
		@Status,
		@Uptime,
		@StartHour,
		@EndHour;
END