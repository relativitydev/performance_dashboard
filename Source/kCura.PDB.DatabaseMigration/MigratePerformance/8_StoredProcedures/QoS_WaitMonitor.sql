USE EDDSPerformance
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_WaitMonitor]') AND type in (N'P', N'PC'))
	DROP PROCEDURE [eddsdbo].[QoS_WaitMonitor]

GO

CREATE PROCEDURE [eddsdbo].[QoS_WaitMonitor]
	@logging bit = 1,
	@AgentID int = -1,
	@eddsPerformanceServerName nvarchar(255),
	@eddsServerName nvarchar(255)
AS
BEGIN
	SET NOCOUNT ON

	DECLARE
		@module nvarchar(25) = 'QoS_WaitMonitor',
		@msg nvarchar(max) = '',
		@loggingVars nvarchar(max) = '',
		@sql nvarchar(max) = '',
		@parmDefinition nvarchar(max) = '',
		@runCondition tinyint = 1, -- [0: error mode] [1: differential mode] [2: init/restart mode]
		@difference bigint,
		@summaryDayHour datetime = DATEADD(hh, DATEDIFF(hh, 0, getutcdate()) - 1, 0),
		@restartDetected bit = 0,
		@lastSqlRestart datetime,
		@waitSummaryID int,
		@previousWaitSummaryID int,
		@serverArtifactId int,
		@serverName nvarchar(255),
		@maxWaitSummaryID int,
		@ioWaitsHigh BIT,
		@numOfProcessesors int,
		@totalWaitTime bigint,
		@include_SOS_SCHEDULER_YIELD bit,
		@SOS_SCHEDULER_YIELD_WaitTypeID int = (select WaitTypeID from eddsdbo.QoS_Waits as w where w.WaitType = 'SOS_SCHEDULER_YIELD'),
		@batchSize int = 10000;

	set @numOfProcessesors = (select count(*) from sys.dm_os_schedulers where [status] = 'VISIBLE ONLINE')
	
	IF @logging = 1
	BEGIN
		SET @loggingVars = '@summaryDayHour = ' + CAST(isNull(CAST(@summaryDayHour as varchar),'NULL') as varchar)
		EXEC EDDSPerformance.eddsdbo.QoS_LogAppend
				@AgentID = @AgentID,
				@module = @module,
				@taskCompleted = 'Starting',
				@otherVars = @loggingVars,
				@nextTask = 'Check for existing wait statistics'
	END

	--Skip gathering waits if we've already collected this hour (or some hour in the future)
	IF EXISTS (SELECT TOP 1 WaitSummaryID FROM eddsdbo.QoS_WaitSummary WHERE SummaryDayHour >= @summaryDayHour)
	BEGIN
		IF @logging = 1
		BEGIN
			SET @loggingVars = '@summaryDayHour = ' + CAST(isNull(CAST(@summaryDayHour as varchar),'NULL') as varchar)
			EXEC EDDSPerformance.eddsdbo.QoS_LogAppend
					@AgentID = @AgentID,
					@module = @module,
					@taskCompleted = 'Found existing wait statistics for this hour',
					@otherVars = @loggingVars,
					@nextTask = 'Terminating'
		END

		RETURN;
	END

	IF @logging = 1
	BEGIN
		EXEC EDDSPerformance.eddsdbo.QoS_LogAppend
				@AgentID = @AgentID,
				@module = @module,
				@taskCompleted = 'Checked for existing wait statistics',
				@nextTask = 'Deleting old wait/file statistics'
	END
	
	--Cleanup old data
	WHILE EXISTS (
		SELECT TOP 1 1
		FROM eddsdbo.QoS_WaitDetail hws
		INNER JOIN eddsdbo.QoS_WaitSummary hwsm
			ON hws.WaitSummaryID = hwsm.WaitSummaryID
		WHERE hwsm.SummaryDayHour < DATEADD(dd, -180, getutcdate())
	)
	BEGIN	
		DELETE TOP (@batchSize) hws
		FROM eddsdbo.QoS_WaitDetail hws
		INNER JOIN eddsdbo.QoS_WaitSummary hwsm
			ON hws.WaitSummaryID = hwsm.WaitSummaryID
		WHERE hwsm.SummaryDayHour < DATEADD(dd, -180, getutcdate())
		OPTION (MAXDOP 2)
	END

	WHILE EXISTS (
		SELECT TOP 1 1
		FROM eddsdbo.QoS_WaitSummary
		WHERE SummaryDayHour < DATEADD(dd, -180, getutcdate())
	)
	BEGIN
		DELETE TOP(@batchSize)
		FROM eddsdbo.QoS_WaitSummary
		WHERE SummaryDayHour < DATEADD(dd, -180, getutcdate())
		OPTION (MAXDOP 2)
	END

	WHILE EXISTS (
		SELECT TOP 1 1
		FROM eddsdbo.QoS_FileLatencySummary
		WHERE SummaryDayHour < DATEADD(dd, -180, getutcdate())
	)
	BEGIN
		DELETE TOP(@batchSize)
		FROM eddsdbo.QoS_FileLatencySummary
		WHERE SummaryDayHour < DATEADD(dd, -180, getutcdate())
		OPTION (MAXDOP 2)
	END

	IF @logging = 1
	BEGIN
		EXEC EDDSPerformance.eddsdbo.QoS_LogAppend
				@AgentID = @AgentID,
				@module = @module,
				@taskCompleted = 'Deleted old wait/file statistics',
				@nextTask = 'Insert new wait statistics for active SQL servers'
	END

	INSERT INTO eddsdbo.QoS_WaitSummary
		(QoSHourID, SummaryDayHour, ServerArtifactID, ServerName, SignalWaitsRatio)
	SELECT
		eddsdbo.QoS_GetServerHourID(ArtifactID, @summaryDayHour),
		@summaryDayHour,
		ArtifactID,
		ServerName,
		0
	FROM eddsdbo.[Server] WITH(NOLOCK)
	WHERE ServerTypeID = 3
		AND (IgnoreServer IS NULL OR IgnoreServer = 0)
		AND DeletedOn IS NULL
		AND ArtifactID IS NOT NULL

	SELECT @waitSummaryID = MIN(WaitSummaryID),
		@maxWaitSummaryID = MAX(WaitSummaryID)
	FROM eddsdbo.QoS_WaitSummary WITH(NOLOCK)
	WHERE SummaryDayHour = @summaryDayHour;

	IF @logging = 1
	BEGIN
		SET @loggingVars = '@waitSummaryID = ' + ISNULL(CAST(@waitSummaryID as varchar), 'NULL') + ', @maxWaitSummaryID = ' + ISNULL(CAST(@maxWaitSummaryID as varchar), 'NULL');
		EXEC EDDSPerformance.eddsdbo.QoS_LogAppend
				@AgentID = @AgentID,
				@module = @module,
				@taskCompleted = 'Inserted work items',
				@otherVars = @loggingVars,
				@nextTask = 'Loop through work items to collect wait statistics per server'
	END

	WHILE (@waitSummaryID <= @maxWaitSummaryID)
	BEGIN
		SET @difference = NULL;
		SET @previousWaitSummaryID = NULL;

		--If any errors occur while processing a server, log it and move on
		BEGIN TRY
			SELECT
				@serverArtifactId = ServerArtifactID,
				@serverName = ServerName
			FROM eddsdbo.QoS_WaitSummary WITH(NOLOCK)
			WHERE WaitSummaryID = @waitSummaryID;
			


			SELECT @previousWaitSummaryID = WaitSummaryID,
				@lastSqlRestart = LastSqlRestart, --Last time we tracked waits, what was the last restart time?
				@difference = DATEDIFF(hh, SummaryDayHour, @summaryDayHour) --How many hours ago did we track this?
			FROM eddsdbo.QoS_WaitSummary WITH(NOLOCK)
			WHERE ServerArtifactID = @serverArtifactId
				AND WaitSummaryID < @waitSummaryID;

			--We need the last SQL server restart time to determine whether to calculate differentials this run
			SET @sql = N'
			UPDATE eddsdbo.QoS_WaitSummary
			SET LastSqlRestart = (
				SELECT sqlserver_start_time
				FROM ' + QUOTENAME(@serverName) + '.Master.sys.dm_os_sys_info WITH(NOLOCK)
			)
			WHERE WaitSummaryID = ' + CAST(@waitSummaryID as varchar);

			IF @logging = 1
			BEGIN
				SET @loggingVars = @sql;
				SET @msg = 'Determined current and prior run state for ' + @serverName;
				EXEC EDDSPerformance.eddsdbo.QoS_LogAppend
						@AgentID = @AgentID,
						@module = @module,
						@taskCompleted = @msg,
						@otherVars = @loggingVars,
						@nextTask = 'Check last SQL server restart time'
			END

			EXEC sp_executesql @sql;

			--This will collect cumulative resource/signal waits (since the last restart)
			SET @sql = N'
			INSERT INTO eddsdbo.QoS_WaitDetail (WaitSummaryID, WaitTypeID, CumulativeWaitMs, CumulativeSignalWaitMs, CumulativeWaitingTasksCount)
			SELECT
				' + CAST(@waitSummaryID AS varchar) + ',
				qw.WaitTypeID,
				wait_time_ms,
				signal_wait_time_ms,
				waiting_tasks_count
			FROM ' + QUOTENAME(@serverName) + '.Master.sys.dm_os_wait_stats ws WITH(NOLOCK)
			INNER JOIN eddsdbo.QoS_Waits qw WITH(NOLOCK)
			ON ws.wait_type COLLATE DATABASE_DEFAULT = qw.WaitType'
			
			IF @logging = 1
			BEGIN
				SET @loggingVars = @sql
				EXEC EDDSPerformance.eddsdbo.QoS_LogAppend
						@AgentID = @AgentID,
						@module = @module,
						@taskCompleted = 'Checked last SQL server restart time',
						@otherVars = @loggingVars,
						@nextTask = 'Gather wait statistics'
			END

			EXEC sp_executesql @sql;
			
			

			IF @logging = 1
			BEGIN
				EXEC EDDSPerformance.eddsdbo.QoS_LogAppend
						@AgentID = @AgentID,
						@module = @module,
						@taskCompleted = 'Gathered wait statistics',
						@nextTask = 'Set run condition'
			END

			--if SQL has been restarted since our last check, we can't calculate a differential
			SELECT TOP 1
				@restartDetected = CASE
					WHEN @lastSqlRestart = LastSqlRestart THEN 0
					ELSE 1
				END
			FROM eddsdbo.QoS_WaitSummary WITH(NOLOCK)
			WHERE WaitSummaryID = @waitSummaryID

			SET @runCondition =
				CASE WHEN @restartDetected = 1 OR ISNULL(@difference, -1) < 1
						THEN 2 --skip the differential if SQL has been restarted or we can't determine a time difference
					ELSE 1 --run in normal mode otherwise
				END;

			UPDATE eddsdbo.QoS_WaitSummary
			SET RunCondition = @runCondition
			WHERE WaitSummaryID = @waitSummaryID

			IF @logging = 1
			BEGIN
				SET @loggingVars =
					'@runCondition = ' + ISNULL(CAST(@runCondition as varchar), 'NULL') +
					', @difference = ' + ISNULL(CAST(@difference as varchar), 'NULL') +
					', @restartDetected = ' + ISNULL(CAST(@restartDetected as varchar), 'NULL')
				SET @msg = (CASE
					WHEN @runCondition = 1 THEN 'Calculate differential waits and signal waits ratio'
					ELSE 'Calculate signal waits ratio'
				END);
				EXEC EDDSPerformance.eddsdbo.QoS_LogAppend
						@AgentID = @AgentID,
						@module = @module,
						@taskCompleted = 'Determined run condition',
						@otherVars = @loggingVars,
						@nextTask = @msg
			END

			--If we're not starting over, we need to calculate differentials
			IF @runCondition = 1
			BEGIN
				--this is a normal run with no missed hours
				;WITH lastWaits AS (
					SELECT WaitTypeID, CumulativeWaitMs, CumulativeSignalWaitMs, CumulativeWaitingTasksCount
					FROM eddsdbo.QoS_WaitDetail WITH(NOLOCK)
					WHERE WaitSummaryID = @previousWaitSummaryID
				)
				UPDATE hws
				SET DifferentialWaitMs =
					 (hws.CumulativeWaitMs - lastWaits.CumulativeWaitMs) / @difference,
					DifferentialSignalWaitMs =
					 (hws.CumulativeSignalWaitMs - lastWaits.CumulativeSignalWaitMs) / @difference,
					DifferentialWaitingTasksCount = 
					 (hws.CumulativeWaitingTasksCount - lastWaits.CumulativeWaitingTasksCount) / @difference
				FROM eddsdbo.QoS_WaitDetail hws WITH(NOLOCK)
				INNER JOIN lastWaits
				ON lastWaits.WaitTypeID = hws.WaitTypeID
				WHERE WaitSummaryID = @waitSummaryID
			END

			set @include_SOS_SCHEDULER_YIELD = 
			(
				select top(1)
				case when DifferentialWaitMs >= wd.DifferentialWaitingTasksCount --ratio is >= 1.0 if DiffWaitMs >= DiffWaitingTasksCount
					then convert(bit, 1)
					else convert(bit, 0) 
				end
				from eddsdbo.QoS_WaitDetail as wd
				where wd.WaitSummaryID = @waitSummaryID and wd.WaitTypeID = @SOS_SCHEDULER_YIELD_WaitTypeID
			)
			
			UPDATE eddsdbo.QoS_WaitSummary
			SET SignalWaitsRatio = (
				SELECT
					SUM(DifferentialSignalWaitMs) / (1 + 1.0 * SUM(DifferentialWaitMs))
				FROM eddsdbo.QoS_WaitDetail WITH(NOLOCK)
				WHERE WaitSummaryID = @waitSummaryID
				AND (WaitTypeID <> @SOS_SCHEDULER_YIELD_WaitTypeID OR @include_SOS_SCHEDULER_YIELD = 1)
			)
			WHERE WaitSummaryID = @waitSummaryID
			
			select @totalWaitTime = isnull(sum(wd.DifferentialWaitMs),0) from eddsdbo.QoS_WaitSummary as ws
			inner join eddsdbo.QoS_WaitDetail as wd on ws.WaitSummaryID = wd.WaitSummaryID
			where ws.WaitSummaryID = @waitSummaryID
			group by ws.QoSHourID
			
			if(@numOfProcessesors > 0)
			BEGIN
				UPDATE eddsdbo.QoS_WaitSummary
				set PercentOfCPUThreshold = convert(decimal(16,2),@totalWaitTime) / convert(decimal(16,2), 3600 * 1000 * @numOfProcessesors)
				where WaitSummaryID = @waitSummaryID
			END

			IF @logging = 1
			BEGIN
				SET @loggingVars = '';
				SET @msg = (CASE
					WHEN @runCondition = 1 THEN 'Calculated differential waits and signal waits ratio'
					ELSE 'Calculated signal waits ratio'
				END);
				EXEC EDDSPerformance.eddsdbo.QoS_LogAppend
						@AgentID = @AgentID,
						@module = @module,
						@taskCompleted = @msg,
						@otherVars = @loggingVars,
						@nextTask = 'Check for high IO waits'
			END

			SET @ioWaitsHigh = ISNULL((
				SELECT TOP 1
					CASE WHEN w.WaitType LIKE 'PAGEIOLATCH%' THEN 1
						ELSE 0
					END
				FROM EDDSPerformance.eddsdbo.QoS_WaitSummary ws WITH(NOLOCK)
				INNER JOIN EDDSperformance.eddsdbo.QoS_WaitDetail wd WITH(NOLOCK)
				ON ws.WaitSummaryID = wd.WaitSummaryID
				INNER JOIN EDDSPerformance.eddsdbo.QoS_Waits w WITH(NOLOCK)
				ON wd.WaitTypeID = w.WaitTypeID
				WHERE ws.WaitSummaryID = @waitSummaryID
				ORDER BY DifferentialWaitMs DESC
			), 0)

			SET @sql = N'
				EXEC ' + QUOTENAME(@serverName) + '.EDDSQoS.eddsdbo.QoS_FileStatsServerMonitor
					@ioWaitsHigh = ' + CAST(@ioWaitsHigh AS varchar) + ',
					@serverArtifactId = ' + CAST(@serverArtifactId AS varchar) + ',
					@summaryDayHour = ''' + CAST(@summaryDayHour AS varchar) + ''',
					@eddsPerformanceServerName = @eddsPerformanceServerName,
					@eddsServerName = @eddsServerName,
					@logging = ' + CAST(@logging AS varchar) + ',
					@AgentID = ' + CAST(@AgentID AS varchar);

			IF @logging = 1
			BEGIN
				SET @loggingVars = @sql
				EXEC EDDSPerformance.eddsdbo.QoS_LogAppend
						@AgentID = @AgentID,
						@module = @module,
						@taskCompleted = 'Checked for high IO waits',
						@otherVars = @loggingVars,
						@nextTask = 'Monitor file latency statistics'
			END

			EXEC sp_executesql @sql, N'@eddsPerformanceServerName nvarchar(255), @eddsServerName nvarchar(255)', @eddsPerformanceServerName, @eddsServerName;

			IF @logging = 1
			BEGIN
				EXEC EDDSPerformance.eddsdbo.QoS_LogAppend
						@AgentID = @AgentID,
						@module = @module,
						@taskCompleted = 'Monitored file latency statistics',
						@nextTask = 'Move to the next server'
			END

		END TRY
		BEGIN CATCH
			IF @logging = 1
			BEGIN
				SET @loggingVars = ERROR_MESSAGE();
				EXEC EDDSPerformance.eddsdbo.QoS_LogAppend
						@AgentID = @AgentID,
						@module = @module,
						@taskCompleted = 'Encountered an error gathering/processing waits for a server',
						@otherVars = @loggingVars,
						@nextTask = 'Set error state, move to the next server'
			END

			UPDATE eddsdbo.QoS_WaitSummary
			SET RunCondition = 0
			WHERE WaitSummaryID = @waitSummaryID
		END CATCH

		SET @waitSummaryID = (ISNULL(
			(SELECT MIN(WaitSummaryID)
			 FROM eddsdbo.QoS_WaitSummary WITH(NOLOCK)
			 WHERE WaitSummaryID > @waitSummaryID),
			@maxWaitSummaryID + 1)
		);
	END

	IF @logging = 1
	BEGIN
		EXEC EDDSPerformance.eddsdbo.QoS_LogAppend
				@AgentID = @AgentID,
				@module = @module,
				@taskCompleted = 'Finished processing all active servers registered with Relativity',
				@nextTask = 'Terminating'
	END
END