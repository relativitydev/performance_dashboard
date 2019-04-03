USE [EDDSQoS]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (select 1 from sysobjects where [name] = 'QoS_VLFServerMonitor' and type = 'P')  
BEGIN
	DROP PROCEDURE eddsdbo.QoS_VLFServerMonitor
END
GO
CREATE PROCEDURE eddsdbo.QoS_VLFServerMonitor
	@SummaryDayHour DATETIME,
	@ServerArtifactID INT,
	@logging BIT = 1,
	@AgentID INT = -1,
	@eddsPerformanceServerName nvarchar(255)
AS
BEGIN
	DECLARE
		@SQL nvarchar(max),
		@parmDefinition nvarchar(max),
		@i INT = 1,
		@iMax INT = 0,
		@dbName NVARCHAR(255),
		@loggingVars NVARCHAR(MAX),
		@logMsg NVARCHAR(MAX),
		@QoSHourID BIGINT = eddsdbo.QoS_GetServerHourID(@ServerArtifactID, @SummaryDayHour),
		@module NVARCHAR(50) = 'QoS_VLFServerMonitor';
		
	IF @logging = 1
	BEGIN
		SET @loggingVars = '@SummaryDayHour = ' + CAST(@SummaryDayHour as varchar)
			+ ', @QoSHourID = ' + CAST(@QoSHourID as varchar)
			+ ', @ServerArtifactID = ' + CAST(@ServerArtifactID as varchar);
		SET @logMsg = 'Started VLF server monitor on ' + @@SERVERNAME;
		EXEC EDDSQoS.eddsdbo.QoS_LogAppend
				@AgentID = @AgentID,
				@module = @Module,
				@taskCompleted = @logMsg,
				@otherVars = @loggingVars,
				@nextTask = 'Clearing out VLF data from previous runs'
				,@eddsPerformanceServerName = @eddsPerformanceServerName
	END

	--Clean out data from previous runs
	TRUNCATE TABLE eddsdbo.VirtualLogFileDW

	IF @logging = 1
	BEGIN
		EXEC EDDSQoS.eddsdbo.QoS_LogAppend
				@AgentID = @AgentID,
				@module = @Module,
				@taskCompleted = 'Cleared out VLF data from previous runs',
				@nextTask = 'Determining databases to check for VLFs'
				,@eddsPerformanceServerName = @eddsPerformanceServerName
	END

	--Determine all the databases we need to check
	CREATE TABLE #VLFDatabases
	(
		DatabaseID INT,
		CONSTRAINT PK_VLFDatabases PRIMARY KEY CLUSTERED (DatabaseID ASC),
		DatabaseName NVARCHAR(255)
	)

	INSERT INTO #VLFDatabases
		(DatabaseID, DatabaseName)
	SELECT
		database_id, name
	FROM sys.databases
	WHERE [name] LIKE 'EDDS%' OR [name] LIKE 'Inv%'
	
	SELECT
		@i = MIN(DatabaseID),
		@iMax = MAX(DatabaseID)
	FROM #VLFDatabases

	IF @logging = 1
	BEGIN
		SET @loggingVars = '@i = ' + CAST(@i as varchar) + ', @imax = ' + CAST(@imax as varchar);
		EXEC EDDSQoS.eddsdbo.QoS_LogAppend
				@AgentID = @AgentID,
				@module = @Module,
				@taskCompleted = 'Determined databases to check for VLFs',
				@otherVars = @loggingVars,
				@nextTask = 'Count VLFs for each database'
				,@eddsPerformanceServerName = @eddsPerformanceServerName
	END

	WHILE (@i <= @iMax)
	BEGIN
		--Set up the database name for the VLF counter
		SELECT @dbName = DatabaseName
		FROM #VLFDatabases
		WHERE DatabaseID = @i;

		--Try to count the number of VLFs, skipping the DB if we're unable
		BEGIN TRY
			EXEC [{{resourcedbname}}].eddsdbo.QoS_VirtualLogFileCounter
				@DatabaseName = @dbName
		END TRY
		BEGIN CATCH
			IF @logging = 1
			BEGIN
				SET @loggingVars = ERROR_MESSAGE();
				SET @logMsg = 'Failed to check database ' + ISNULL(@dbName, 'NULL') + ' for VLFs due to an error - skipping';
				EXEC EDDSQoS.eddsdbo.QoS_LogAppend
						@AgentID = @AgentID,
						@module = @Module,
						@taskCompleted = @logMsg,
						@otherVars = @loggingVars,
						@nextTask = 'Continue checking remaining databases for VLFs'
						,@eddsPerformanceServerName = @eddsPerformanceServerName
			END
		END CATCH

		--Increment loop variable
		SET @i = ISNULL((
			SELECT MIN(DatabaseID)
			FROM #VLFDatabases
			WHERE DatabaseID > @i
		), @iMax + 1);
	END

	SET @SQL = N'
		INSERT INTO ' + QUOTENAME(@eddsPerformanceServerName) + '.EDDSPerformance.eddsdbo.VirtualLogFileSummary
			(QoSHourID, SummaryDayHour, ServerArtifactID, DatabaseName, VirtualLogFiles)
		SELECT TOP 1
			@QoSHourID, @SummaryDayHour, @ServerArtifactID, DatabaseName, VirtualLogFiles
		FROM EDDSQoS.eddsdbo.VirtualLogFileDW
		ORDER BY VirtualLogFiles DESC
	';

	IF @logging = 1
	BEGIN
		SET @loggingVars = @SQL;
		EXEC EDDSQoS.eddsdbo.QoS_LogAppend
				@AgentID = @AgentID,
				@module = @Module,
				@taskCompleted = 'Finished counting VLFs per database',
				@otherVars = @loggingVars,
				@nextTask = 'Push maximum VLF count and associated database metadata to EDDSPerformance'
				,@eddsPerformanceServerName = @eddsPerformanceServerName
	END

	SET @parmDefinition = N'@QoSHourID BIGINT, @SummaryDayHour DATETIME, @ServerArtifactID INT';

	EXEC sp_executesql @SQL, @parmDefinition,
		@QoSHourID,
		@SummaryDayHour,
		@ServerArtifactID;

	IF @logging = 1
	BEGIN
		SET @loggingVars = @SQL;
		EXEC EDDSQoS.eddsdbo.QoS_LogAppend
				@AgentID = @AgentID,
				@module = @Module,
				@taskCompleted = 'Pushed maximum VLF count and associated database metadata to EDDSPerformance',
				@otherVars = @loggingVars
				,@eddsPerformanceServerName = @eddsPerformanceServerName
	END
END