USE EDDSQoS
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_FileStatsServerMonitor]') AND type in (N'P', N'PC'))
	DROP PROCEDURE [eddsdbo].[QoS_FileStatsServerMonitor]

GO

CREATE PROCEDURE [eddsdbo].[QoS_FileStatsServerMonitor]
	@ioWaitsHigh BIT = 0,
	@serverArtifactID INT,
	@summaryDayHour DATETIME,
	@logging bit = 1,
	@AgentID int = -1,
	@eddsPerformanceServerName nvarchar(255),
	@eddsServerName nvarchar(255)
AS
BEGIN
	SET NOCOUNT ON

	DECLARE
		@module nvarchar(25) = 'QoS_FileStatsMonitor',
		@msg nvarchar(max) = '',
		@loggingVars nvarchar(max) = '',
		@sql nvarchar(max) = '',
		@parmDefinition nvarchar(max) = '';

	IF @logging = 1
	BEGIN
		SET @loggingVars = '@summaryDayHour = ' + CAST(isNull(CAST(@summaryDayHour as varchar),'NULL') as varchar)
		EXEC EDDSQoS.eddsdbo.QoS_LogAppend
				@AgentID = @AgentID,
				@module = @module,
				@taskCompleted = 'Starting',
				@otherVars = @loggingVars,
				@nextTask = 'Remove old rows from file-level latency tables'
				,@eddsPerformanceServerName = @eddsPerformanceServerName
	END

	--Note: the worst-performing file information stays in EDDSPerformance for 180 days
	--We keep latency scores across all database files for an hour
	TRUNCATE TABLE eddsdbo.QoS_FileStats

	--The full history is still available for six hours, however, in case it needs to be reviewed
	DELETE FROM eddsdbo.QoS_FileStatsHistory
	WHERE SummaryDayHour < DATEADD(hh, -6, @summaryDayHour)

	IF @logging = 1
	BEGIN
		SET @loggingVars = '@summaryDayHour = ' + CAST(isNull(CAST(@summaryDayHour as varchar),'NULL') as varchar)
		EXEC EDDSQoS.eddsdbo.QoS_LogAppend
				@AgentID = @AgentID,
				@module = @module,
				@taskCompleted = 'Removed old rows from file-level latency tables',
				@otherVars = @loggingVars,
				@nextTask = 'Collect file-level latency statistics'
				,@eddsPerformanceServerName = @eddsPerformanceServerName
	END

	IF NOT EXISTS (SELECT TOP 1 1 FROM EDDSQoS.eddsdbo.QoS_FileStatsHistory WHERE SummaryDayHour = @summaryDayHour)
	BEGIN
		INSERT INTO eddsdbo.QoS_FileStatsHistory
			(SummaryDayHour, DatabaseID, FileID, DBName, CumulativeReads, CumulativeIOStallReadMs, CumulativeWrites, CumulativeIOStallWriteMs, IsDataFile)
		SELECT
			@summaryDayHour,
			[vfs].[database_id],
			[vfs].[file_id],
			DB_NAME ([vfs].[database_id]) AS [DBName],
			[num_of_reads],
			[io_stall_read_ms],
			[num_of_writes],
			[io_stall_write_ms],
			[IsDataFile] =
			CASE WHEN [mf].[type] = 0 THEN 1
				ELSE 0
			END
		FROM sys.dm_io_virtual_file_stats (NULL,NULL) vfs
		INNER JOIN sys.master_files mf
			ON vfs.[database_id] = mf.[database_id] AND vfs.[file_id] = mf.[file_id]
		
		UPDATE fsh
		SET fsh.DifferentialReads =
				CASE
					WHEN fsh.CumulativeReads >= fshlast.CumulativeReads THEN fsh.CumulativeReads - fshlast.CumulativeReads
					ELSE NULL
				END,
			fsh.DifferentialWrites =
				CASE
					WHEN fsh.CumulativeWrites >= fshlast.CumulativeWrites THEN fsh.CumulativeWrites - fshlast.CumulativeWrites
					ELSE NULL
				END,
			fsh.DifferentialIOStallReadMs =
				CASE
					WHEN fsh.CumulativeIOStallReadMs >= fshlast.CumulativeIOStallReadMs THEN fsh.CumulativeIOStallReadMs - fshlast.CumulativeIOStallReadMs
					ELSE NULL
				END,
			fsh.DifferentialIOStallWriteMs =
				CASE
					WHEN fsh.CumulativeIOStallWriteMs >= fshlast.CumulativeIOStallWriteMs THEN fsh.CumulativeIOStallWriteMs - fshlast.CumulativeIOStallWriteMs
					ELSE NULL
				END
		FROM eddsdbo.QoS_FileStatsHistory fsh
		INNER JOIN eddsdbo.QoS_FileStatsHistory fshlast
		ON fsh.DBName = fshlast.DBName
			AND fsh.FileID = fshlast.FileID
		WHERE fshlast.SummaryDayHour = DATEADD(hh, -1, @summaryDayHour) --If this doesn't exist, we just don't get differentials
	END

	INSERT INTO eddsdbo.QoS_FileStats
		(DBName, ReadLatency, WriteLatency, IsDataFile)
	SELECT
		DBName,
		ReadLatency =
			CASE WHEN DifferentialReads = 0 THEN 0
				ELSE (DifferentialIOStallReadMs / DifferentialReads)
			END,
		WriteLatency =
			CASE WHEN DifferentialWrites = 0 THEN 0
				ELSE (DifferentialIOStallWriteMs / DifferentialWrites)
			END,
		IsDataFile
	FROM eddsdbo.QoS_FileStatsHistory WITH(NOLOCK)
	WHERE SummaryDayHour = @summaryDayHour

	--delete old workspaces and not edds, case db, or tempdb from file stats
	SET @sql = N'
	delete from eddsqos.eddsdbo.QoS_FileStats
	where DBName not in (select ''EDDS'' + Convert(varchar(10), c.ArtifactID) from ' + QUOTENAME(@eddsServerName) + '.edds.eddsdbo.[Case] as c) and DBName <> ''EDDS'' and DBName <> ''tempdb''
	'
	EXEC sp_executesql @sql
	
	UPDATE eddsdbo.QoS_FileStats
	SET LatencyScore =
		CASE
			WHEN @ioWaitsHigh = 0 THEN 100
			WHEN ReadLatency > 100 AND IsDataFile = 1 THEN 0
			WHEN WriteLatency > 30 AND IsDataFile = 1 THEN 0
			WHEN WriteLatency > 10 AND IsDataFile = 0 THEN 0
			ELSE 100
		END

	SET @sql = N'
		INSERT INTO ' + QUOTENAME(@eddsPerformanceServerName) + '.EDDSPerformance.eddsdbo.QoS_FileLatencySummary
			(ServerArtifactID, QoSHourID, SummaryDayHour, HighestLatencyDatabase, ReadLatencyMs, WriteLatencyMs, LatencyScore, IOWaitsHigh, IsDataFile)
		SELECT TOP 1
			@serverArtifactID,
			eddsdbo.QoS_GetServerHourID(@serverArtifactID, @summaryDayHour),
			@summaryDayHour,
			DBName,
			ReadLatency,
			WriteLatency,
			LatencyScore,
			@ioWaitsHigh,
			IsDataFile
		FROM eddsdbo.QoS_FileStats WITH(NOLOCK)
		ORDER BY LatencyScore ASC, (ReadLatency + WriteLatency) DESC'

	SET @parmDefinition = N'@ioWaitsHigh BIT, @serverArtifactID INT, @summaryDayHour DATETIME';

	IF @logging = 1
	BEGIN
		SET @loggingVars = @sql
		EXEC EDDSQoS.eddsdbo.QoS_LogAppend
				@AgentID = @AgentID,
				@module = @module,
				@taskCompleted = 'Gathered file-level latency statistics',
				@otherVars = @loggingVars,
				@nextTask = 'Process file-level latency statistics'
				,@eddsPerformanceServerName = @eddsPerformanceServerName
	END

	EXEC sp_executesql @sql, @parmDefinition,
				@ioWaitsHigh, @serverArtifactId, @summaryDayHour;

	IF @logging = 1
	BEGIN
		EXEC EDDSQoS.eddsdbo.QoS_LogAppend
				@AgentID = @AgentID,
				@module = @module,
				@taskCompleted = 'Finished processing file-level latency statistics'
				,@eddsPerformanceServerName = @eddsPerformanceServerName
	END

END