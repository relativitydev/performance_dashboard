USE [EDDSPerformance]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (select 1 from sysobjects where [name] = 'QoS_VLFMonitor' and type = 'P')  
BEGIN
	DROP PROCEDURE eddsdbo.QoS_VLFMonitor
END
GO
CREATE PROCEDURE eddsdbo.QoS_VLFMonitor
	@logging BIT = 1,
	@AgentID INT = -1,
	@eddsPerformanceServerName nvarchar(255)
AS
BEGIN
	DECLARE
		@SQL NVARCHAR(MAX),
		@serverName NVARCHAR(255),
		@loggingVars NVARCHAR(MAX),
		@logMsg NVARCHAR(MAX),
		@summaryDayHour DATETIME = DATEADD(hh, DATEDIFF(hh, 0, getutcdate()) - 1, 0),
		@module NVARCHAR(50) = 'QoS_VLFMonitor',
		@i INT = 1,
		@imax INT = 0;		

	IF @logging = 1
	BEGIN
		EXEC EDDSPerformance.eddsdbo.QoS_LogAppend
				@AgentID = @AgentID,
				@module = @Module,
				@taskCompleted = 'Launched VLF server monitor',
				@nextTask = 'Determining server list for VLF monitoring'
	END

	CREATE TABLE #VLFServers
	(
		ServerArtifactID INT,
		CONSTRAINT PK_VLFServers PRIMARY KEY CLUSTERED (ServerArtifactID ASC),
		ServerName NVARCHAR(255)
	)

	INSERT INTO #VLFServers
		(ServerArtifactID, ServerName)
	SELECT
		s.ArtifactID, s.ServerName
	FROM eddsdbo.[Server] AS s WITH(NOLOCK)
	INNER JOIN edds.eddsdbo.ResourceServer AS rs WITH(NOLOCK)
		ON rs.ArtifactID = s.ArtifactID
	WHERE s.ServerTypeID = 3
		AND s.DeletedOn IS NULL
		AND (s.IgnoreServer = 0 OR s.IgnoreServer IS NULL)
		AND s.ArtifactID IS NOT NULL

	SELECT
		@i = MIN(ServerArtifactID),
		@imax = MAX(ServerArtifactID)
	FROM #VLFServers;

	IF @logging = 1
	BEGIN
		SET @loggingVars = '@i = ' + CAST(@i as varchar) + ', @imax = ' + CAST(@imax as varchar);
		EXEC EDDSPerformance.eddsdbo.QoS_LogAppend
				@AgentID = @AgentID,
				@module = @Module,
				@taskCompleted = 'Determined server list for VLF monitoring',
				@otherVars = @loggingVars,
				@nextTask = 'Counting VLFs on each server'
	END

	WHILE (@i <= @imax)
	BEGIN
		SELECT @serverName = ServerName
		FROM #VLFServers
		WHERE ServerArtifactID = @i;

		SET @SQL = N'
			EXEC ' + QUOTENAME(@serverName) + '.EDDSQoS.eddsdbo.QoS_VLFServerMonitor
				@SummaryDayHour = ''' + CAST(@summaryDayHour as varchar) + ''',
				@ServerArtifactID = ' + CAST(@i as varchar) + ',
				@logging = ' + CAST(@logging as varchar) + ',
				@AgentID = ' + CAST(@AgentID as varchar) + ',
				@eddsPerformanceServerName = @eddsPerformanceServerName'

		IF @logging = 1
		BEGIN
			SET @loggingVars = @SQL;
			EXEC EDDSPerformance.eddsdbo.QoS_LogAppend
					@AgentID = @AgentID,
					@module = @Module,
					@taskCompleted = 'Prepared QoS_VLFServerMonitor call SQL',
					@otherVars = @loggingVars,
					@nextTask = 'Executing QoS_VLFServerMonitor'
		END

		--Invoke QoS_VLFServerMonitor for this server, logging any errors
		BEGIN TRY
			EXEC sp_executesql @SQL, N'@eddsPerformanceServerName nvarchar(255)', @eddsPerformanceServerName;

			IF @logging = 1
			BEGIN
				EXEC EDDSPerformance.eddsdbo.QoS_LogAppend
						@AgentID = @AgentID,
						@module = @Module,
						@taskCompleted = 'Completed QoS_VLFServerMonitor',
						@nextTask = 'Checking remaining servers for VLFs'
			END
		END TRY
		BEGIN CATCH
			IF @logging = 1
			BEGIN
				SET @loggingVars = ERROR_MESSAGE();
				EXEC EDDSPerformance.eddsdbo.QoS_LogAppend
						@AgentID = @AgentID,
						@module = @Module,
						@taskCompleted = 'QoS_VLFServerMonitor call failed',
						@otherVars = @loggingVars,
						@nextTask = 'Checking remaining servers for VLFs'
			END
		END CATCH

		SET @i = ISNULL((
			SELECT MIN(ServerArtifactID)
			FROM #VLFServers
			WHERE ServerArtifactID > @i
		), @imax + 1);
	END

	IF @logging = 1
	BEGIN
		EXEC EDDSPerformance.eddsdbo.QoS_LogAppend
				@AgentID = @AgentID,
				@module = @Module,
				@taskCompleted = 'Completed VLF monitoring on all servers',
				@nextTask = 'Terminating'
	END
END