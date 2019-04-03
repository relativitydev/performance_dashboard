-- EDDSPerformance
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_BackupAndDBCCCheckMonLauncherTest]') AND type in (N'P', N'PC'))
DROP PROCEDURE [eddsdbo].[QoS_BackupAndDBCCCheckMonLauncherTest]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO
 
CREATE PROCEDURE [eddsdbo].[QoS_BackupAndDBCCCheckMonLauncherTest]
	@eddsPerformanceServerName nvarchar(255),
	@eddsServerName nvarchar(255),
	@beginDate DATETIME = NULL,
	@endDate DATETIME = NULL,
	@CreateHistory bit = 0,
	@logging bit = 0,
		@AgentID int = -1
	AS
	BEGIN
		DECLARE @now datetime = (select top 1 HourTimeStamp from eddsdbo.MockHours mh inner join eddsdbo.[Hours] h on mh.HourId = h.Id order by HourTimeStamp desc);
		DECLARE @nowUtc datetime = (select top 1 HourTimeStamp from eddsdbo.MockHours mh inner join eddsdbo.[Hours] h on mh.HourId = h.Id order by HourTimeStamp desc);
		
		DECLARE
			@USESQL nvarchar(max),
		@loggingVars nvarchar(max),
		@ParmDefinition NVARCHAR(500),
		@DatabaseName NVARCHAR(100),
		@Module NVARCHAR(100) = 'QoS_BackupAndDBCCCheckMonLauncherTest',
		@lastDate datetime,
		@DBCClastDate datetime,
		@selectBeginDate datetime,
		@selectEndDate datetime, --so that this procedure can create history if it is needed.
		@summaryDayHour datetime = dateadd(hour,datediff(hour,0,@nowUtc),0),
		@key int,
		@DBCCIsNew BIT = 0,
		@BackIsNew BIT = 0,
		@dbID INT,
		@window INT = 9,
		@useDbccDbinfo bit = 0, --If UseDbccCommandMonitoring is True, we will include DBCC DBINFO in the DBCC history
		@useDbccLogs bit = 0, --If UseDbccViewMonitoring is True, we will include DBCC log views in the DBCC history
		@timezoneOffset int = DATEDIFF(MINUTE, @nowUtc, @now), --Difference in minutes between SQL local time and UTC
		@tempServer nVARCHAR(255),
		@targetServer nvarchar(255),
		@targetDatabase nvarchar(150) = 'EDDSQoS',
		@primaryServerId INT = (
			SELECT TOP 1 ArtifactID
			FROM EDDS.eddsdbo.[ExtendedResourceServer] WITH(NOLOCK)
			WHERE [Type] = 'SQL - Primary'
		),
		@BackupDBCCFailedServers int,
		@ptsLostPerDay INT = 5, --Contributes to the backup/DBCC frequency scores
		@coverageMinPct DECIMAL(5,4) = 0.0075, --At/below this threshold, backup/DBCC coverage scores are 100
		@coverageMaxPct DECIMAL(5,4) = 0.10, --At this threshold, backup/DBCC coverage scores are 0
		@dbTotal DECIMAL(10,0), --Determines coverage severity of gaps in backup/DBCCs
		@backupFrequency DECIMAL(5,2) = 100,
		@backupCoverage DECIMAL(5,2) = 100,
		@dbccFrequency DECIMAL(5,2) = 100,
		@dbccCoverage DECIMAL(5,2) = 100,
		@backupScore DECIMAL(5,2) = 100,
		@dbccScore DECIMAL(5,2) = 100,
		@rpoScore DECIMAL(5,2) = 100,
		@rtoScore DECIMAL(5,2) = 100,
		@worstRPO NVARCHAR(255) = '',
		@worstRTO NVARCHAR(255) = '',
		@potentialDataLoss INT = 0,
		@estimatedTTR DECIMAL(6,2) = 0,
		@batchSize int = 10000,
		@allgapsServerName nvarchar(255),
		@allgapsDBName nvarchar(255),
		@allgapsDateCreated datetime;

	declare @backupHistoryDataDays int = -30;
	declare @backupHistoryDataDeleteDays int = -90;	
	
		
	--This gives us a consistent local time comparison across RPO/RTO calculations
	DECLARE @localSummaryDayHour DATETIME = DATEADD(MINUTE, @timezoneOffset, @summaryDayHour);
	--Cache weekRanges so they aren't calculated multiple times in the script.
	DECLARE @localSummaryDayHourWeekRange DATETIME = DATEADD(dd, -7, @localSummaryDayHour);
	DECLARE @summaryDayHourWeekRange DATETIME = DATEADD(dd, -7, @summaryDayHour);

	IF @beginDate IS NULL
		SET @beginDate = @nowUtc
	IF @endDate IS NULL
		SET @endDate = @beginDate

	IF @logging = 1 
	BEGIN
		SET @loggingVars = 'AuditStartDate = ' + CAST(isNull(CAST(@beginDate as varchar),'NULL') as varchar)
			+ ', @summaryDayHour = ' + ISNULL(CAST(@summaryDayHour as varchar(max)), 'NULL')
			+ ', @localSummaryDayHour = ' + ISNULL(CAST(@localSummaryDayHour as varchar(max)), 'NULL');
		EXEC eddsdbo.QoS_LogAppend
				@AgentID = @AgentID,
				@module = @Module,
				@taskCompleted = 'Launched QoS_BackupAndDBCCCheckMonLauncher',
				@otherVars = @loggingVars,
				@nextTask = 'Creating tables'
	END
		
	IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_DBCCBACKKEY]') AND type in (N'U'))
		DROP TABLE eddsdbo.QoS_DBCCBACKKEY
	CREATE TABLE eddsdbo.QoS_DBCCBACKKEY
	(
		KID INT IDENTITY (1, 1), Primary Key(KID),
		run bit
	)

	--Keep a running list of all databases checked.
	TRUNCATE TABLE [eddsdbo].[QoS_AllDatabasesChecked]

	--Create the eddsdbo.QoS_DBInfoServer temp table. Will hold all the SQL servers known by Performance Dashboard
	IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_tempServersTest]') AND type in (N'U'))
		DROP TABLE EDDSDBO.QoS_tempServersTest
	CREATE TABLE EDDSDBO.QoS_tempServersTest
	(
		ServerID INT IDENTITY ( 1 , 1 ),Primary Key (ServerID),
		ServerName nVARCHAR(255),
		Failed bit,
		Errors varchar(max)
	)
	
	-- When a server is accessible, but one or more databases could not be checked, those databases will appear here
	IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_FailedDatabasesTest]') AND type in (N'U'))
		DROP TABLE EDDSDBO.QoS_FailedDatabasesTest
	CREATE TABLE EDDSDBO.QoS_FailedDatabasesTest
	(
		DatabaseID  INT    IDENTITY ( 1 , 1 ),Primary Key (DatabaseID),
		DBName nvarchar(255),
		ServerName nvarchar(255),
		Errors varchar(max)
	)

	IF @logging = 1 
	BEGIN
		EXEC eddsdbo.QoS_LogAppend
				@AgentID = @AgentID,
				@module = @Module,
				@taskCompleted = 'Completed table and variable declarations.'
	END

	IF @endDate < @begindate
		RETURN

	IF EXISTS (SELECT TOP 1 1 FROM eddsdbo.QoS_BackResults WHERE LoggedDate = @summaryDayHour)
	OR EXISTS (SELECT TOP 1 1 FROM eddsdbo.QoS_DBCCResults WHERE LoggedDate = @summaryDayHour)
	OR EXISTS (SELECT TOP 1 1 FROM eddsdbo.QoS_RecoverabilityIntegritySummary WHERE SummaryDayHour = @summaryDayHour)
	BEGIN
		IF @logging = 1
		BEGIN
			SET @loggingVars = 'SummaryDayHour = ' + isNull(CAST(@summaryDayHour as varchar),'NULL')
			EXEC eddsdbo.QoS_LogAppend
					@AgentID = @AgentID,
					@module = @Module,
					@taskCompleted = 'Found existing data for the current hour',
					@otherVars = @loggingVars,
					@nextTask = 'Terminating'
		END
		
		RETURN
	END

	--Determine which methods to use for DBCC monitoring
	SELECT @useDbccDbinfo =
		(CASE WHEN Value = 'True' THEN 1
		ELSE 0
		END)
	FROM eddsdbo.Configuration
	WHERE Section = 'kCura.PDB' AND Name = 'UseDbccCommandMonitoring';

	SELECT @useDbccLogs =
		(CASE WHEN Value = 'True' THEN 1
		ELSE 0
		END)
	FROM eddsdbo.Configuration
	WHERE Section = 'kCura.PDB' AND Name = 'UseDbccViewMonitoring';

	--if creating new history is needed, the @begin and end dates will be adjusted, behind the scenes. 
	SET @selectBeginDate = @beginDate
	SET @selectEndDate = @endDate

	IF @logging = 1
	BEGIN
		SET @loggingVars = 'AuditStartDate = ' + ISNULL(CAST(@beginDate as varchar), 'NULL') + ', @endDate = ' + ISNULL(CAST(@endDate as varchar), 'NULL')
		EXEC eddsdbo.QoS_LogAppend
				@AgentID = @AgentID,
				@module = @Module,
				@taskCompleted = 'Completed table and variable declarations.',
				@otherVars = @loggingVars,
				@nextTask = 'Get the date of the last check'
	END

	---------------------------------------------------------------------------------------------------------------
	--
	--	Check Date Logic
	--	
	--	Check to see if the scripts have ever been run (successfully) before. If not, then create history. 
	--	Set @lastdate to the last backup date
	--	If @lastdate is null (no backup dates recorded, QoS_BackResults is empty)
	--	Then set @createHistory = 1, set @begindate to one year ago, set @enddate to today
	--
	---------------------------------------------------------------------------------------------------------------
		
	SELECT TOP 1 @lastDate = [LastBackupDate]
	FROM eddsdbo.QoS_BackResults WITH(NOLOCK)
	ORDER by [LastBackupDate] DESC

	IF ISNULL(@lastDate,1) = 1 
	BEGIN
		SET @CreateHistory = 1
		SET @beginDate = DATEADD(yy,-1, @now)
		SET @endDate = @now
	END

	SELECT TOP 1 @DBCClastDate = [lastCleanDBCCDate]
	FROM eddsdbo.QoS_DBCCResults WITH(NOLOCK)
	ORDER by lastCleanDBCCDate DESC

	IF @DBCClastDate > @lastDate
	SET @lastDate = @DBCClastDate

	IF (@beginDate > @lastdate or (@beginDate < @lastDate and @endDate > @lastdate))
	BEGIN
		SET @CreateHistory = 1
		SET @beginDate = @lastDate 
		SET @endDate = @now
	END 

	IF @endDate < @lastDate 
		SET @CreateHistory = 0

	----------------------------------------------
	--
	-- Create History
	--
	----------------------------------------------

	--Set IsNew bits if Result tables do not exist
	IF (SELECT COUNT(kdbbuID) FROM eddsdbo.QoS_BackResults WITH(NOLOCK)) = 0
		SET @BackIsNew = 1
	IF (SELECT COUNT(kdbccbID) FROM eddsdbo.QoS_DBCCResults WITH(NOLOCK)) = 0
		SET @DBCCIsNew = 1

	IF @logging = 1 
	BEGIN
		SET @loggingVars = 'Create History = ' + Cast(@CreateHistory as Varchar) +', @BackIsNew = ' + CAST(@BackIsNew as VARCHAR(1)) + ', @DBCCIsNew = ' + CAST(@DBCCIsNew as VARCHAR(1))		
		EXEC eddsdbo.QoS_LogAppend
			@AgentID = @AgentID,
			@module = @Module,
			@taskCompleted = 'Completed date and create history logic ',
			@otherVars = @loggingVars,
			@nextTask = 'Create History, Populating EDDSDBO.QoS_tempServersTest table, and truncate the QoS_tempServersTest table.'
	END
			
	----------------------------------------------------------------------------------------
	--
	--	Get list of servers
	--
	--	Insert list of server in the environment into the EDDSDBO.QoS_tempServersTest table
	--	from the EDDSPerformance.eddsdbo.Server table. (ServerTypeID = 3)
	--
	----------------------------------------------------------------------------------------
		
	TRUNCATE TABLE EDDSDBO.QoS_tempServersTest
		
		INSERT INTO EDDSDBO.QoS_tempServersTest 
		SELECT ServerName, 0, NULL
		FROM eddsdbo.[Server] s WITH(NOLOCK)
		inner join eddsdbo.[MockServer] ms on s.ServerId = ms.ServerId
		WHERE ServerTypeID = 3
		AND DeletedOn IS NULL
		AND (IgnoreServer = 0 OR
		IgnoreServer IS NULL)

	----------------------------------------------------------------------------------------
	--
	--	Check date ranges for NULL values
	--	SET to past 24 hours if they are
	--
	----------------------------------------------------------------------------------------
	
	-- Check date range.  If it is NULL then set @enddate to now and set @begindate to 23 hours ago
	IF(@beginDate IS NULL OR @endDate IS NULL)
	BEGIN
		SET @endDate = @now
		SET @beginDate = DATEADD(HH, -23, @endDate)
	END
	
	IF @logging = 1 
	BEGIN
		SET @loggingVars =  'Date values so far: End date = ' + Convert(varchar,@enddate) + ', Begin date = ' + convert(varchar,@begindate)
		EXEC eddsdbo.QoS_LogAppend
			@AgentID = @AgentID,
			@module = @Module,
			@taskCompleted = 'Inserted servers into QoS_tempServersTest table ',
			@otherVars = @loggingVars,
			@nextTask = 'Execute QoS_DBCCCheckServerMon on each server.'
	END

	----------------------------------------------------------------------------------------
	--
	--	Execute QoS_BackupAndDBCCServerMon stored procedure
	--
	-- 	Cycle through each server in the EDDSDBO.QoS_DBInfoServer temp table
	-- 	Connect to the selected server and execute the QoS_DBCCCheckServerMon sproc
	-- 	When the sproc returns remove the server from the table
	--	make sure this isn't already running
	----------------------------------------------------------------------------------------

	DECLARE @ID INT = (SELECT MIN(ServerID) FROM EDDSDBO.QoS_tempServersTest)
	DECLARE @MaxID INT = (SELECT MAX(ServerID) FROM EDDSDBO.QoS_tempServersTest)

	INSERT INTO eddsdbo.QoS_DBCCBACKKEY (run) VALUES (1)
	SELECT TOP 1 @key = KID FROM eddsdbo.QoS_DBCCBACKKEY order by KID DESC 

	--	IF @lastDate IS NULL then backups were never collected
	--	Set @lastDate back to a very long time ago
	
	IF @lastDate IS NULL 
		SET @lastDate = '1900-01-01 00:00:00.000'

	WHILE(@ID <= @MaxID)
	BEGIN
		-- Work on the top 1 server from the EDDSDBO.QoS_tempServersTest table	
		SET @tempServer = (SELECT TOP 1 ServerName from EDDSDBO.QoS_tempServersTest WHERE ServerID = @Id)
		
		-- Check date range.  If it is NULL then set @enddate to now and set @begindate to 23 hours ago
		IF(@beginDate IS NULL OR @endDate IS NULL)
		BEGIN
			SET @endDate = @now
			SET @beginDate = DATEADD(HH, -23, @endDate)
		END
		
		-- Call inside sproc with top 1 serverName	 -- in order for this to work, the 'tempServer' has to be real, but these 'MockServer's aren't.  We should just call against EDDSPerformance for these
		IF @logging = 1 
		BEGIN
			SET @loggingVars =   '@lastDate = ' + CAST(@lastDate as varchar(23)) + '; Current Server = ' + QUOTENAME(@tempServer) + '; @USESQL = ' + @USESQL;
			EXEC eddsdbo.QoS_LogAppend
				@AgentID = @AgentID,
				@module = @Module,
				@taskCompleted = 'Entered loop to call DBCC and BACKUP Checks',
				@otherVars = @loggingVars,
				@nextTask = 'Execute DBCC and BACKUP Checks on each server. See OtherVars for the server I am about to attempt to reach.'
		END
	
		BEGIN TRY
			EXECUTE [eddsdbo].[QoS_DBCCCheckServerMonTest]
			@beginDate = @beginDate
			,@enddate = @endDate
			,@summaryDayHour = @summaryDayHour
			,@AgentID = @AgentID
			,@currentServerName = @tempServer
			,@eddsPerformanceServerName = @eddsPerformanceServerName 
			,@eddsServerName = @eddsServerName 
			,@lastDate = @lastDate
			,@useDbccDbinfo = @useDbccDbinfo
			,@logging = @logging
			
			EXECUTE [eddsdbo].[QoS_BackupCheckServerMonTest]
			@summaryDayHour = @summaryDayHour
			,@currentServerName = @tempServer
			,@eddsPerformanceServerName = @eddsPerformanceServerName
		END TRY
		BEGIN CATCH
			--If we fall into this block, the server will not be marked as completed, indicating a problem
			IF @logging = 1 
			BEGIN
				SET @loggingVars = '@lastDate = ' + CAST(@lastDate as varchar(23)) + 'Current Server = ' + QUOTENAME(@tempServer) + '; Message: Error running stored procedure on ' + QUOTENAME(@tempServer) + ' from ' + QUOTENAME(@eddsPerformanceServerName) + '.' + CONVERT(varchar(250), REPLACE(ERROR_MESSAGE(), '''', '')) + 'Error ' + CONVERT(varchar(50), ERROR_NUMBER()) +
				', Severity ' + CONVERT(varchar(5), ERROR_SEVERITY()) +
				', State ' + CONVERT(varchar(5), ERROR_STATE()) + 
				', Procedure ' + ISNULL(ERROR_PROCEDURE(), '-') + 
				', Line ' + CONVERT(varchar(5), ERROR_LINE());
				EXEC eddsdbo.QoS_LogAppend
					@AgentID = @AgentID,
					@module = @Module,
					@taskCompleted = 'Error occurred during QoS_DBCCCheckServerMon call',
					@otherVars = @loggingVars,
					@nextTask = 'Execute QoS_DBCCCheckServerMon on remaining servers. See OtherVars for the server I am about to attempt to reach. If this error was severe enough, this may be my final entry.'
			END
			
			UPDATE EDDSDBO.QoS_tempServersTest
			SET Failed = 1, Errors = ERROR_MESSAGE()
			WHERE ServerID = @Id;
		END CATCH
		
		IF @logging = 1 
		BEGIN
			SET @loggingVars = '@lastDate = ' + CAST(@lastDate as varchar(23)) + 'Current Server = ' + QUOTENAME(@tempServer)
			EXEC eddsdbo.QoS_LogAppend
				@AgentID = @AgentID,
				@module = @Module,
				@taskCompleted = 'Completed QoS_DBCCCheckServerMon',
				@otherVars = @loggingVars,
				@nextTask = 'Execute QoS_DBCCCheckServerMon on each server. See OtherVar for the server I just finished. Incrementing @Id and moving to next server now, if there is one.'
		END
		
		SET @Id = @Id + 1;
	END

	IF @logging = 1 
	BEGIN
		SET @loggingVars = '@lastDate = ' + CAST(@lastDate as varchar(23)) + 'Last Server Worked on = ' + QUOTENAME(@tempServer)
		EXEC eddsdbo.QoS_LogAppend
			@AgentID = @AgentID,
			@module = @Module,
			@taskCompleted = 'Completed all calls to QoS_DBCCCheckServerMon',
			@otherVars = @loggingVars,
			@nextTask = '(Skipped in Test) Checking view-based monitoring status'
	END
	
-- Tests do not currently support DBCC View based monitoring
--	IF (@useDbccLogs = 1)
--	BEGIN
--		--DBCC command log views on all target servers will be included in the DBCC history			
--		SELECT @ID = MIN(DbccTargetId),
--			@MaxID = MAX(DbccTargetId)
--		FROM eddsdbo.DBCCTarget
--		WHERE Active = 1;
--		
--		WHILE (@ID <= @MaxID)
--		BEGIN
--			BEGIN TRY
--				SELECT TOP 1 @targetDatabase = DatabaseName, @targetServer = ServerName
--				FROM eddsdbo.DBCCTarget
--				WHERE DbccTargetId = @ID;
--		
--				--We pull back at most 90 days of DBCC history per database, and for databases with existing data, we pull incrementally
--				SET @USESQL = 'INSERT INTO ' + QUOTENAME(@eddsPerformanceServerName) + '.EDDSPerformance.EDDSDBO.QoS_DBCCHistory
--					(DBName, CompletedOn, IsCommandBased)
--				SELECT
--					DatabaseName,
--					DbccTime,
--					0
--				FROM ' + QUOTENAME(@targetServer) + '.' + QUOTENAME(@targetDatabase) + '.eddsdbo.QoS_DBCCLog L WITH(NOLOCK)
--				WHERE DbccTime > ''' + convert(nvarchar(50), DATEADD(dd, @backupHistoryDataDays, @now)) + '''
--					AND DbccTime > ISNULL((
--						SELECT MAX(CompletedOn)
--						FROM ' + QUOTENAME(@eddsPerformanceServerName) + '.EDDSPerformance.eddsdbo.QoS_DBCCHistory WITH(NOLOCK)
--						WHERE DBName = L.DatabaseName
--							AND CompletedOn > ''' + convert(nvarchar(50), DATEADD(dd, @backupHistoryDataDays, @now)) + '''
--							AND IsCommandBased = 0
--					), '''')'
--				
--				IF @logging = 1
--				BEGIN
--					SET @loggingVars =   '@targetServer = ' + QUOTENAME(@targetServer) + ', @targetDatabase = ' + QUOTENAME(@targetDatabase) + ', @USESQL = ' + @USESQL;
--					EXEC EDDSPerformance.eddsdbo.QoS_LogAppend
--						@AgentID = @AgentID,
--						@module = @Module,
--						@taskCompleted = 'Checking for DBCC history in views',
--						@otherVars = @loggingVars,
--						@nextTask = 'Look for remaining DBCC history targets'
--				END
--				
--				EXEC(@USESQL);
--			END TRY
--			BEGIN CATCH
--				IF @logging = 1 
--				BEGIN
--					SET @loggingVars = ERROR_MESSAGE();
--					EXEC EDDSPerformance.eddsdbo.QoS_LogAppend
--						@AgentID = @AgentID,
--						@module = @Module,
--						@taskCompleted = 'Checking for DBCC history failed',
--						@otherVars = @loggingVars,
--						@nextTask = 'Look for remaining DBCC history targets'
--				END
--			END CATCH
--			
--			SET @ID = ISNULL((SELECT MIN(DbccTargetId)
--				FROM eddsdbo.DBCCTarget
--				WHERE DbccTargetId > @ID AND Active = 1), @MaxID + 1);
--		END
--	END
	
	IF @logging = 1 
	BEGIN
		EXEC eddsdbo.QoS_LogAppend
			@AgentID = @AgentID,
			@module = @Module,
			@taskCompleted = 'Finished checking for backups and DBCC',
			@otherVars = @loggingVars,
			@nextTask = 'Delete old backup and DBCC data'
	END
	
	--Clean up old data
	WHILE EXISTS (
		SELECT TOP 1 1
		FROM eddsdbo.QoS_BackupHistory 
		WHERE LoggedDate < DATEADD(dd, @backupHistoryDataDeleteDays, @nowUtc)
	)
	BEGIN
		DELETE TOP(@batchSize)
		FROM eddsdbo.QoS_BackupHistory
		WHERE LoggedDate < DATEADD(dd, @backupHistoryDataDeleteDays, @nowUtc)
		OPTION (MAXDOP 2)
	END

	WHILE EXISTS (
		SELECT TOP 1 1
		FROM eddsdbo.QoS_DBCCHistory
		WHERE CompletedOn < DATEADD(dd, @backupHistoryDataDeleteDays, @nowUtc)
	)
	BEGIN
		DELETE TOP (@batchSize)
		FROM eddsdbo.QoS_DBCCHistory
		WHERE CompletedOn < DATEADD(dd, @backupHistoryDataDeleteDays, @nowUtc)
		OPTION (MAXDOP 2)
	END
	
	WHILE EXISTS (
		SELECT TOP 1 1
		FROM eddsdbo.QoS_BackResults
		WHERE LoggedDate < DATEADD(dd, -180, @nowUtc)
	)
	BEGIN	
		DELETE TOP (@batchSize)
		FROM eddsdbo.QoS_BackResults
		WHERE LoggedDate < DATEADD(dd, -180, @nowUtc)
		OPTION (MAXDOP 2)
	END
	
	WHILE EXISTS (
		SELECT TOP 1 1
		FROM eddsdbo.QoS_DBCCResults
		WHERE LoggedDate < DATEADD(dd, -180, @nowUtc)
	)
	BEGIN	
		DELETE TOP (@batchSize)
		FROM eddsdbo.QoS_DBCCResults
		WHERE LoggedDate < DATEADD(dd, -180, @nowUtc)
		OPTION (MAXDOP 2)
	END
	
	WHILE EXISTS (
		SELECT TOP 1 1
		FROM eddsdbo.QoS_BackSummary
		WHERE EntryDate < DATEADD(dd, -180, @nowUtc)
	)
	BEGIN	
		DELETE TOP (@batchSize)
		FROM eddsdbo.QoS_BackSummary
		WHERE EntryDate < DATEADD(dd, -180, @nowUtc)
		OPTION (MAXDOP 2)
	END
	
	WHILE EXISTS (
		SELECT TOP 1 1
		FROM eddsdbo.QoS_DBCCSummary
		WHERE EntryDate < DATEADD(dd, -180, @nowUtc)
	)
	BEGIN	
		DELETE TOP (@batchSize)
		FROM eddsdbo.QoS_DBCCSummary
		WHERE EntryDate < DATEADD(dd, -180, @nowUtc)
		OPTION (MAXDOP 2)
	END

	IF @logging = 1 
	BEGIN
		EXEC eddsdbo.QoS_LogAppend
				@AgentID = @AgentID,
				@module = @Module,
				@taskCompleted = 'Deleted old backup and DBCC data',
				@nextTask = 'Gathering final backup/DBCC results for analysis'
		END
		
	--Collect the last backup time for all databases
	INSERT INTO eddsdbo.QoS_BackResults
		(ServerName, DBName, CaseArtifactID, dateDBCreated, LastBackupDate, LoggedDate)
	SELECT
		D.ServerName,
		D.DBName,
		D.CaseArtifactID,
		D.DateCreated,
		CASE WHEN ISNULL(MAX(H.CompletedOn), D.DateCreated) <= D.DateCreated THEN D.DateCreated --Only penalize as far back as the DB creation time
			ELSE ISNULL(MAX(H.CompletedOn), D.DateCreated)
		END,
		@summaryDayHour
	FROM eddsdbo.QoS_AllDatabasesChecked D WITH(NOLOCK)
	LEFT JOIN (
		SELECT DBName, MAX(CompletedOn) CompletedOn
		FROM eddsdbo.QoS_BackupHistory WITH(NOLOCK)
		WHERE CompletedOn > DATEADD(d, @backupHistoryDataDays, @summaryDayHour)
		and BackupType IN ('D','I') --Log backups do not satisfy the frequency/coverage requirements
		GROUP BY DBName
	) H
	ON D.DBName = H.DBName
	WHERE D.CaseArtifactID IS NOT NULL
	and d.ServerName not in (select ExclusionName from eddsdbo.QoS_MonitoringExclusions me where me.SkipBackups = 1)
	and d.DBName not in (select ExclusionName from eddsdbo.QoS_MonitoringExclusions me where me.SkipBackups = 1)
	GROUP BY D.ServerName, D.DBName, D.CaseArtifactID, D.DateCreated, H.CompletedOn

	--Collect the last DBCC time for all databases
	IF (@useDbccLogs = 1 OR @useDbccDbinfo = 1)
	BEGIN
		INSERT INTO eddsdbo.QoS_DBCCResults
			(ServerName, DBName, CaseArtifactID, dateDBCreated, LastCleanDBCCDate, LoggedDate)
		SELECT
			D.ServerName,
			D.DBName,
			D.CaseArtifactID,
			D.DateCreated,
			CASE WHEN ISNULL(MAX(H.CompletedOn), D.DateCreated) <= D.DateCreated THEN D.DateCreated --Only penalize as far back as the DB creation time
				ELSE ISNULL(MAX(H.CompletedOn), D.DateCreated)
			END,
			@summaryDayHour
		FROM eddsdbo.QoS_AllDatabasesChecked D
		LEFT JOIN eddsdbo.QoS_DBCCHistory H
		ON D.DBName = H.DBName
		WHERE CaseArtifactID IS NOT NULL
		and d.ServerName not in (select ExclusionName from eddsdbo.QoS_MonitoringExclusions me where me.SkipDBCC = 1)
		and d.DBName not in (select ExclusionName from eddsdbo.QoS_MonitoringExclusions me where me.SkipDBCC = 1)
		GROUP BY D.ServerName, D.DBName, D.CaseArtifactID, D.DateCreated
	END
	
	IF @logging = 1
	BEGIN
		EXEC eddsdbo.QoS_LogAppend
			@AgentID = @AgentID,
			@module = @Module,
			@taskCompleted = 'Gathered final backup/DBCC results for analysis',
			@nextTask = 'Removing exclusions from backup/DBCC results'
	END
	
	--Exclude databases that need to be skipped
	DELETE bs
	FROM eddsdbo.QoS_BackSummary bs
	INNER JOIN eddsdbo.QoS_AllDatabasesChecked adc
	ON bs.CaseArtifactID = adc.CaseArtifactID
	INNER JOIN eddsdbo.QoS_MonitoringExclusions me
	ON bs.DBName = me.ExclusionName
		OR adc.ServerName = me.ExclusionName
	WHERE me.SkipBackups = 1

	DELETE ds
	FROM eddsdbo.QoS_DBCCSummary ds
	INNER JOIN eddsdbo.QoS_AllDatabasesChecked adc
	ON ds.CaseArtifactID = adc.CaseArtifactID
	INNER JOIN eddsdbo.QoS_MonitoringExclusions me
	ON ds.DBName = me.ExclusionName
		OR adc.ServerName = me.ExclusionName
	WHERE me.SkipDBCC = 1
	
	IF @logging = 1 
	BEGIN
		EXEC eddsdbo.QoS_LogAppend
			@AgentID = @AgentID,
			@module = @Module,
			@taskCompleted = 'Removed exclusions from backup/DBCC results',
			@nextTask = 'Updating server names in historic results'
	END
	
	--Update server names for databases that have moved
	UPDATE br
	SET ServerName = adc.ServerName
	FROM eddsdbo.QoS_AllDatabasesChecked adc
	INNER JOIN eddsdbo.QoS_BackResults br
	ON adc.CaseArtifactID = br.CaseArtifactID
	WHERE adc.ServerName != br.ServerName

	UPDATE dr
	SET ServerName = adc.ServerName
	FROM eddsdbo.QoS_AllDatabasesChecked adc
	INNER JOIN eddsdbo.QoS_DBCCResults dr
	ON adc.CaseArtifactID = dr.CaseArtifactID
	WHERE adc.ServerName != dr.ServerName
	
	IF @logging = 1 
	BEGIN
		EXEC eddsdbo.QoS_LogAppend
				@AgentID = @AgentID,
				@module = @Module,
				@taskCompleted = 'Updated server names in historic results',
				@nextTask = 'Update DaysSinceLast and Status for backup/DBCC data'
		END
		
	----------------------------------------------------------------------------------------
	--
	--	Set DaysSinceLast
	--
	----------------------------------------------------------------------------------------

	UPDATE eddsdbo.QoS_DBCCResults
	SET DaysSinceLast = DATEDIFF(DAY, LastCleanDBCCDate, DATEADD(MINUTE, @timezoneOffset, LoggedDate))
	WHERE LoggedDate = @summaryDayHour

	UPDATE eddsdbo.QoS_BackResults
	SET DaysSinceLast = DATEDIFF(DAY, LastBackupDate, DATEADD(MINUTE, @timezoneOffset, LoggedDate))
	WHERE LoggedDate = @summaryDayHour

	----------------------------------------------------------------------------------------
	--
	--	Update Backup and DBCC statuses
	--
	----------------------------------------------------------------------------------------
	
	--	If the days since the most recent backup to today is more than 9, status = 0.  
	--	Where the ..Status is already 1, it means that the case is predetermined to be inactive. 
	
	UPDATE eddsdbo.QoS_BackResults
	SET BackupStatus =
	CASE
		WHEN DaysSinceLast > @window THEN 0
		ELSE 1
	END
	WHERE LoggedDate = @summaryDayHour
	
	UPDATE eddsdbo.QoS_DBCCResults
	SET DBCCStatus =
	CASE
		WHEN DaysSinceLast > @window THEN 0
		ELSE 1
	END
	WHERE LoggedDate = @summaryDayHour
			
	IF @logging = 1 
	BEGIN
		EXEC eddsdbo.QoS_LogAppend
			@AgentID = @AgentID,
			@module = @Module,
			@taskCompleted = 'Updated DaysSinceLast and Status for backup/DBCC data',
			@nextTask = 'Populating summary tables with gap information'
	END
			
	----------------------------------------------------------------------------------------
	--
	--	Populate DBCCSummary Table
	--
	----------------------------------------------------------------------------------------
	
	INSERT INTO eddsdbo.QoS_DBCCSummary
		(DBName, CaseArtifactID, LastDBCCDate, EntryDate, WindowExceededBy, GapResolvedDate)
	SELECT D1.DBName, D1.CaseArtifactID, D1.LastCleanDBCCDate, D1.LoggedDate, D1.DaysSinceLast - @window as Window, NULL
	FROM eddsdbo.QoS_DBCCResults D1
	WHERE D1.LoggedDate = @summaryDayHour
		AND D1.DaysSinceLast > @window
		AND D1.CaseArtifactID NOT IN (
			SELECT D3.CaseArtifactID
			FROM eddsdbo.QoS_DBCCSummary D3
			WHERE GapResolvedDate IS NULL AND D3.LastDBCCDate = D1.LastCleanDBCCDate
		)
		AND D1.LastCleanDBCCDate NOT IN(
			SELECT D2.LastDBCCDate
			FROM eddsdbo.QoS_DBCCSummary D2
			WHERE D2.CaseArtifactID = D1.CaseArtifactID
		)

	UPDATE eddsdbo.QoS_DBCCSummary
	SET WindowExceededBy = DATEDIFF(dd, LastDBCCDate, @localSummaryDayHour) - @window
	WHERE GapResolvedDate IS NULL
	
	---------------------------------------------------------------
	--
	--	Check if DBCC gap has been resolved
	--		Check if entry date in DBCC summary table is 
	--		earlier than the corresponding last DBCC date
	--		in the DBCC results table
	--
	---------------------------------------------------------------

	UPDATE ds
	SET GapResolvedDate = d.GapResolvedOn,
		WindowExceededBy = DATEDIFF(dd, LastDBCCDate, d.GapResolvedOn) - @window
	FROM eddsdbo.QoS_DBCCSummary ds
	CROSS APPLY (
		SELECT
			MIN(CompletedOn) GapResolvedOn
		FROM eddsdbo.QoS_DBCCHistory
		WHERE DBName = ds.DBName
		AND CompletedOn > ds.LastDBCCDate
	) d
	WHERE ds.GapResolvedDate IS NULL
	AND d.GapResolvedOn IS NOT NULL
	
	--Remove gaps if we discover they were actually compliant
	DELETE FROM eddsdbo.QoS_DBCCSummary
	WHERE WindowExceededBy <= 0;
	
	----------------------------------------------------------------------------------------
	--
	--	Populate BackSummary Table
	--
	----------------------------------------------------------------------------------------
	
	INSERT INTO eddsdbo.QoS_BackSummary
		(DBName, CaseArtifactID, LastBackupDate, EntryDate, WindowExceededBy, GapResolvedDate)
	SELECT D1.DBName, D1.CaseArtifactID, D1.LastBackupDate, D1.LoggedDate, D1.DaysSinceLast - @window as Window, NULL
	FROM eddsdbo.QoS_BackResults D1
	WHERE D1.Loggeddate = @summaryDayHour
		AND D1.DaysSinceLast > @window
		AND D1.CaseArtifactID NOT IN (
			SELECT D3.CaseArtifactID
			FROM eddsdbo.QoS_BackSummary D3
			WHERE GapResolvedDate IS NULL AND D3.LastBackupDate = D1.LastBackupDate
		)
		AND D1.LastBackupDate NOT IN (
			SELECT D2.LastBackupDate
			FROM eddsdbo.QoS_BackSummary D2
			WHERE D2.CaseArtifactID = D1.CaseArtifactID
		)
	
	UPDATE eddsdbo.QoS_BackSummary
	SET WindowExceededBy = DATEDIFF(dd, LastBackupDate, @localSummaryDayHour) - @window
	WHERE GapResolvedDate IS NULL
	
	---------------------------------------------------------------
	--
	--	Check if Backup gap has been resolved
	--		Check if entry date in backup summary table is 
	--		earlier than the corresponding last backup date
	--		in the backup results table
	--
	---------------------------------------------------------------
	
	UPDATE bs
	SET GapResolvedDate = d.GapResolvedOn,
		WindowExceededBy = DATEDIFF(dd, LastBackupDate, d.GapResolvedOn) - @window
	FROM eddsdbo.QoS_BackSummary bs
	CROSS APPLY (
		SELECT
			MIN(CompletedOn) GapResolvedOn
		FROM eddsdbo.QoS_BackupHistory
		WHERE DBName = bs.DBName
		AND BackupType IN ('D','I') --Log backups are not eligible for gap resolution
		AND CompletedOn > bs.LastBackupDate
	) d
	WHERE bs.GapResolvedDate IS NULL
	AND d.GapResolvedOn IS NOT NULL

	--Remove gaps if we discover they were actually compliant
	DELETE FROM eddsdbo.QoS_BackSummary
	WHERE WindowExceededBy <= 0;

	IF @logging = 1 
	BEGIN
		EXEC eddsdbo.QoS_LogAppend
			@AgentID = @AgentID,
			@module = @Module,
			@taskCompleted = 'Populated summary tables with gap information',
			@nextTask = 'Refreshing gap summary data for reports'
	END
	
	--Clear out yesterday's summary information
	TRUNCATE TABLE eddsdbo.QoS_GapSummary

	--Insert latest results and known violators
	INSERT INTO eddsdbo.QoS_GapSummary
		(DatabaseName, CaseArtifactID, IsBackup, LastActivityDate, ResolutionDate, GapSize)
	SELECT
		DBName,
		CaseArtifactID,
		1,
		DATEADD(hh, DATEDIFF(hh, 0, LastBackupDate), 0),
		DATEADD(hh, DATEDIFF(hh, 0, GapResolvedDate), 0),
		WindowExceededBy + 9
	FROM eddsdbo.QoS_BackSummary WITH(NOLOCK)
	WHERE GapResolvedDate IS NULL
		OR GapResolvedDate > DATEADD(dd, @backupHistoryDataDays, @localSummaryDayHour)

	INSERT INTO eddsdbo.QoS_GapSummary
		(DatabaseName, CaseArtifactID, IsBackup, LastActivityDate, ResolutionDate, GapSize)
	SELECT
		DBName,
		CaseArtifactID,
		0,
		DATEADD(hh, DATEDIFF(hh, 0, LastDBCCDate), 0),
		DATEADD(hh, DATEDIFF(hh, 0, GapResolvedDate), 0),
		WindowExceededBy + 9
	FROM eddsdbo.QoS_DBCCSummary WITH(NOLOCK)
	WHERE GapResolvedDate IS NULL
		OR GapResolvedDate > DATEADD(dd, @backupHistoryDataDays, @localSummaryDayHour)

	INSERT INTO eddsdbo.QoS_GapSummary
		(DatabaseName, CaseArtifactID, IsBackup, LastActivityDate, ResolutionDate, GapSize)
	SELECT
		DBName,
		CaseArtifactID,
		1,
		DATEADD(hh, DATEDIFF(hh, 0, LastBackupDate), 0),
		NULL,
		DaysSinceLast
	FROM eddsdbo.QoS_BackResults WITH(NOLOCK)
	WHERE LoggedDate = @summaryDayHour

	INSERT INTO eddsdbo.QoS_GapSummary
		(DatabaseName, CaseArtifactID, IsBackup, LastActivityDate, ResolutionDate, GapSize)
	SELECT
		DBName,
		CaseArtifactID,
		0,
		DATEADD(hh, DATEDIFF(hh, 0, LastCleanDBCCDate), 0),
		NULL,
		DaysSinceLast
	FROM eddsdbo.QoS_DBCCResults WITH(NOLOCK)
	WHERE LoggedDate = @summaryDayHour

	--Get names of servers and workspaces associated with each database
		UPDATE g
		SET ServerName = s.ServerName,
			ServerArtifactID = s.ArtifactID,
			WorkspaceName = ''
		FROM eddsdbo.QoS_GapSummary g
		INNER JOIN eddsdbo.QoS_AllDatabasesChecked adc WITH(NOLOCK)
			ON g.DatabaseName = adc.DBName
		INNER JOIN eddsdbo.[Server] s WITH(NOLOCK)
			ON adc.ServerName = s.ServerName
		inner join eddsdbo.[MockServer] ms on s.ServerId = ms.ServerId
		WHERE s.ServerTypeID = 3
			AND s.ArtifactID IS NOT NULL
			AND ISNULL(s.IgnoreServer, 0) = 0
		AND s.DeletedOn IS NULL

	IF @logging = 1
	BEGIN
		EXEC eddsdbo.QoS_LogAppend
			@AgentID = @AgentID,
			@module = @Module,
			@taskCompleted = 'Refreshed gap summary data for reports',
			@nextTask = 'Analyzing maximum data loss'
	END
	
	CREATE TABLE #allGaps (
		AllGapsId INT IDENTITY (1,1), PRIMARY KEY (AllGapsId),
		ServerName nvarchar(255) COLLATE database_default NULL,
		DBName nvarchar(255) COLLATE database_default NULL,
		GapStart datetime NULL,
		GapEnd datetime NULL,
		DataLossSeconds INT NULL
	)
	CREATE NONCLUSTERED INDEX IX_DBName ON #allGaps (
		DBName
	) INCLUDE (DataLossSeconds)
	CREATE NONCLUSTERED INDEX IX_DataLossSeconds ON #allGaps (
		DataLossSeconds
	) INCLUDE (DBName)


-- Begin inserting into #allGaps
	
	-- Gaps between backups over 7 days and size of gaps at the end of the window
	;WITH backups (completedon, dbname, rowid)	AS
	(
		SELECT CompletedOn, dbname, ROW_NUMBER() OVER(PARTITION BY DBName ORDER BY CompletedOn ASC) AS rowid 
		FROM eddsdbo.QoS_BackupHistory bh WITH(NOLOCK)
		WHERE CompletedOn > DATEADD(dd, -7, @localSummaryDayHour) 
			AND bh.CompletedOn <= @localSummaryDayHour 
	)
	INSERT INTO #allGaps 
		(ServerName, DBName, GapStart, GapEnd, DataLossSeconds)
	SELECT 
		adc.ServerName as ServerName,
		bh.DBName as DBName,
		bh.CompletedOn as GapStart,
		ISNULL(rh.CompletedOn, @localSummaryDayHour) as GapEnd,
		DATEDIFF(SECOND, bh.CompletedOn, ISNULL(rh.CompletedOn, @localSummaryDayHour)) as DataLossSeconds
	FROM backups bh WITH(NOLOCK)
	inner join eddsdbo.QoS_AllDatabasesChecked adc WITH(NOLOCK) on adc.dbname = bh.dbname
	LEFT OUTER JOIN backups rh ON rh.rowid - 1 = bh.rowid and bh.dbname = rh.dbname
	OPTION (MAXDOP 2)
	
	--Size of gaps at the start of the window
	--If the database was created this week, treat the creation time as the initial backup
	INSERT INTO #allGaps
		(ServerName, DBName, GapStart, GapEnd, DataLossSeconds)
	SELECT
		adc.ServerName as ServerName,
		bh.dbname as DBName,
		DATEADD(dd, -7, @localSummaryDayHour),
		MIN(bh.CompletedOn),
		CASE WHEN MIN(adc.DateCreated) > DATEADD(dd, -7, @localSummaryDayHour)
			THEN DATEDIFF(SECOND, MIN(adc.DateCreated), MIN(bh.CompletedOn))
			ELSE DATEDIFF(SECOND, DATEADD(dd, -7, @localSummaryDayHour), MIN(bh.CompletedOn))
		END
	FROM eddsdbo.QoS_BackupHistory bh WITH(NOLOCK)
	inner join eddsdbo.QoS_AllDatabasesChecked adc WITH(NOLOCK) on adc.dbname = bh.dbname
	WHERE bh.CompletedOn > DATEADD(dd, -7, @localSummaryDayHour) 
		AND bh.CompletedOn <= @localSummaryDayHour
	group by adc.ServerName, bh.dbname
	OPTION (MAXDOP 2)

	--Databases with no backups in the window
	--This only applies to databases with NO backups this week
	INSERT INTO #allGaps
		(ServerName, DBName, GapStart, GapEnd, DataLossSeconds)
	SELECT
		adc.ServerName as ServerName,
		adc.dbname as DBName,
		DATEADD(dd, -7, @localSummaryDayHour),
		@localSummaryDayHour,
		CASE WHEN adc.DateCreated > DATEADD(dd, -7, @localSummaryDayHour)
			THEN DATEDIFF(SECOND, adc.DateCreated, @localSummaryDayHour) --Database was created this week and hasn't been backed up yet, track the gap from DB creation to now
			ELSE 604800 --7 days * 24 hours * 60 minutes = 10080 mins * 60 seconds = 604800 seconds
		END
		from eddsdbo.QoS_AllDatabasesChecked adc WITH(NOLOCK)
		where adc.DateCreated <= @localSummaryDayHour
		and adc.dbname not in
		(
			SELECT distinct bh.dbName
			FROM eddsdbo.QoS_BackupHistory bh WITH(NOLOCK)
			WHERE bh.CompletedOn > DATEADD(dd, -7, @localSummaryDayHour) 
				AND bh.CompletedOn <= @localSummaryDayHour
		)

-- End inserting into #allGaps

	TRUNCATE TABLE eddsdbo.QoS_RecoveryObjectiveSummary
	INSERT INTO eddsdbo.QoS_RecoveryObjectiveSummary
		(ServerName, DBName, EstimatedTimeToRecoverHours, PotentialDataLossMinutes)
	SELECT
		ag.ServerName,
		ag.DBName,
		0,
		MAX(ag.DataLossSeconds)/60.0
	FROM #allGaps ag
	GROUP BY ag.DBName, ag.ServerName

	--Calculate RPO scores for each database
	UPDATE eddsdbo.QoS_RecoveryObjectiveSummary
	SET RPOScore = CASE
			WHEN ISNULL(PotentialDataLossMinutes, 0) <= 15 THEN 100
			WHEN PotentialDataLossMinutes <= 240
				THEN 100 - 0.0888 * (PotentialDataLossMinutes - 15) --100 to 80
			WHEN PotentialDataLossMinutes < 1440
				THEN 80 - 0.0666 * (PotentialDataLossMinutes - 240) --80 to 0
			ELSE 0
		END;

	IF @logging = 1
	BEGIN
		EXEC eddsdbo.QoS_LogAppend
			@AgentID = @AgentID,
			@module = @Module,
			@taskCompleted = 'Completed maximum data loss analysis',
			@nextTask = 'Analyzing time to recover'
	END

	--Calculate maximum TTR per database and RTO scores
	;WITH ttr AS
	(
	  SELECT
		adc.DBName,
		f.CompletedOn AS LastFull,
		d.CompletedOn AS LastDiff,
		(ISNULL(f.Duration, DATEDIFF(SECOND, adc.DateCreated, @localSummaryDayHour)) + ISNULL(d.Duration, 0) + ISNULL(l.CumulativeDuration, 0))/3600.0 AS TimeToRecoverHours
	  FROM eddsdbo.QoS_AllDatabasesChecked adc
	  OUTER APPLY (
		SELECT TOP 1
			CompletedOn,
			Duration
		FROM eddsdbo.QoS_BackupHistory bh WITH(NOLOCK)
		WHERE bh.DBName = adc.DBName
			AND bh.BackupType = 'D'
		ORDER BY bh.CompletedOn DESC
	  ) f
	  OUTER APPLY (
		SELECT TOP 1
			CompletedOn,
			Duration
		FROM eddsdbo.QoS_BackupHistory bh WITH(NOLOCK)
		WHERE bh.DBName = adc.DBName
			AND bh.CompletedOn > f.CompletedOn
			AND bh.BackupType = 'I'
		ORDER BY bh.CompletedOn DESC
	  ) d
	  OUTER APPLY (
		SELECT
			SUM(Duration) CumulativeDuration
		FROM eddsdbo.QoS_BackupHistory bh WITH(NOLOCK)
		WHERE bh.DBName = adc.DBName
			AND bh.CompletedOn > ISNULL(d.CompletedOn, f.CompletedOn)
			AND bh.BackupType = 'L'
	  ) l
	)
	UPDATE ros
	SET EstimatedTimeToRecoverHours = CASE
			WHEN ttr.TimeToRecoverHours > 168 THEN 168
			ELSE ttr.TimeToRecoverHours
		END,
		RTOScore = CASE
			WHEN ISNULL(ttr.TimeToRecoverHours, 0) <= 4 THEN 100
			WHEN ttr.TimeToRecoverHours > 168 THEN 0
			WHEN ttr.TimeToRecoverHours <= 24
				THEN 100 - (ttr.TimeToRecoverHours - 4) -- 100 to 80
			WHEN ttr.TimeToRecoverHours < 48
				THEN 80 - 3.3333 * (ttr.TimeToRecoverHours - 24) -- 80 to 0
			ELSE 0
		END
	FROM eddsdbo.QoS_RecoveryObjectiveSummary ros
	INNER JOIN ttr
		ON ros.DBName = ttr.DBName

	IF @logging = 1
	BEGIN
		EXEC eddsdbo.QoS_LogAppend
			@AgentID = @AgentID,
			@module = @Module,
			@taskCompleted = 'Completed time to recover analysis',
			@nextTask = 'Removing monitoring exclusions for RPO/RTO'
	END

	DELETE ros
	FROM eddsdbo.QoS_RecoveryObjectiveSummary ros
	INNER JOIN eddsdbo.QoS_MonitoringExclusions me
	ON ros.DBName = me.ExclusionName
		OR ros.ServerName = me.ExclusionName
	WHERE me.SkipBackups = 1

	IF @logging = 1
	BEGIN
		EXEC eddsdbo.QoS_LogAppend
			@AgentID = @AgentID,
			@module = @Module,
			@taskCompleted = 'Removed monitoring exclusions for RPO/RTO',
			@nextTask = 'Scoring recoverability and integrity'
	END

	SELECT @dbTotal = COUNT(*)
		FROM eddsdbo.QoS_AllDatabasesChecked WITH(NOLOCK)
		WHERE CaseArtifactID >= -1

		--IF (@dbTotal = 0)
		--BEGIN
		--	SELECT @dbTotal = COUNT(*)
		--	FROM EDDS.eddsdbo.[Case] WITH(NOLOCK)
		--END

		SELECT
			@backupCoverage = (CASE
			WHEN COUNT(DISTINCT CaseArtifactID)/@dbTotal < @coverageMinPct THEN 100
			WHEN COUNT(DISTINCT CaseArtifactID)/@dbTotal >= @coverageMaxPct THEN 0
			ELSE (@coverageMaxPct - COUNT(DISTINCT CaseArtifactID)/@dbTotal)/(@coverageMaxPct - @coverageMinPct)*100.0
		END),
		@backupFrequency = (CASE
			WHEN ISNULL(MAX(WindowExceededBy), 0) >= 100.0/@ptsLostPerDay THEN 0
			ELSE 100 - ISNULL(MAX(WindowExceededBy), 0) * @ptsLostPerDay
		END)
	FROM eddsdbo.QoS_BackSummary WITH(NOLOCK)
	WHERE CaseArtifactID >= -1
		AND (GapResolvedDate IS NULL OR GapResolvedDate > @summaryDayHourWeekRange)

	--If at least one DBCC monitoring method is enabled, score DBCCs
	IF (@useDbccLogs = 1 OR @useDbccDbinfo = 1)
	BEGIN
		SELECT
			@dbccCoverage = (CASE
				WHEN COUNT(DISTINCT CaseArtifactID)/@dbTotal < @coverageMinPct THEN 100
				WHEN COUNT(DISTINCT CaseArtifactID)/@dbTotal >= @coverageMaxPct THEN 0
				ELSE (@coverageMaxPct - COUNT(DISTINCT CaseArtifactID)/@dbTotal)/(@coverageMaxPct - @coverageMinPct)*100.0
			END),
			@dbccFrequency = (CASE
				WHEN ISNULL(MAX(WindowExceededBy), 0) >= 100.0/@ptsLostPerDay THEN 0
				ELSE 100 - ISNULL(MAX(WindowExceededBy), 0) * @ptsLostPerDay
			END)
		FROM eddsdbo.QoS_DBCCSummary WITH(NOLOCK)
		WHERE CaseArtifactID >= -1
			AND (GapResolvedDate IS NULL OR GapResolvedDate > @summaryDayHourWeekRange)
	END
	ELSE
	BEGIN
		SET @dbccCoverage = NULL;
		SET @dbccFrequency = NULL;
	END

	--Determine worst RPO and capture historic information about data loss
	SELECT TOP 1
		@rpoScore = ISNULL(RPOScore, 100),
		@worstRPO = DBName,
		@potentialDataLoss = PotentialDataLossMinutes
	FROM eddsdbo.QoS_RecoveryObjectiveSummary WITH(NOLOCK)
	WHERE DBName NOT LIKE 'INV%' --This isn't used for scoring
	ORDER BY PotentialDataLossMinutes DESC

	--Determine worst RTO and capture historic information about time to recover
	SELECT TOP 1
		@rtoScore = ISNULL(RTOScore, 100),
		@worstRTO = DBName,
		@estimatedTTR = EstimatedTimeToRecoverHours
	FROM eddsdbo.QoS_RecoveryObjectiveSummary WITH(NOLOCK)
	WHERE DBName NOT LIKE 'INV%' --This isn't used for scoring
	ORDER BY EstimatedTimeToRecoverHours DESC

	INSERT INTO eddsdbo.QoS_RecoverabilityIntegritySummary
		(SummaryDayHour, RecoverabilityIntegrityScore, BackupFrequencyScore, BackupCoverageScore, DbccFrequencyScore,
		 DbccCoverageScore, RPOScore, RTOScore, WorstRPODatabase, WorstRTODatabase, PotentialDataLossMinutes, EstimatedTimeToRecoverHours)
	SELECT
		@summaryDayHour,
		CASE WHEN @dbccFrequency IS NULL OR @dbccCoverage IS NULL
			THEN (@backupFrequency + @backupCoverage + @rpoScore + @rtoScore)/4.0
		ELSE
			(@backupFrequency + @backupCoverage + @dbccFrequency + @dbccCoverage + @rpoScore + @rtoScore)/6.0
		END,
		@backupFrequency,
		@backupCoverage,
		@dbccFrequency,
		@dbccCoverage,
		@rpoScore,
		@rtoScore,
		@worstRPO,
		@worstRTO,
		@potentialDataLoss,
		@estimatedTTR

	IF @logging = 1
	BEGIN
		EXEC eddsdbo.QoS_LogAppend
			@AgentID = @AgentID,
			@module = @Module,
			@taskCompleted = 'Scored recoverability and integrity'
	END
END
