-- EDDSPerformance

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_DBCCCheckServerMonTest]') AND type in (N'P', N'PC'))
DROP PROCEDURE [eddsdbo].[QoS_DBCCCheckServerMonTest]
GO

CREATE PROCEDURE [eddsdbo].[QoS_DBCCCheckServerMonTest]
	@beginDate DATETIME 
	,@endDate DATETIME
	,@summaryDayHour DATETIME
	,@AgentID INT = -1
	,@lastDate datetime
	,@useDbccDbinfo bit = 0
	,@logging bit = 0
	,@currentServerName nvarchar(255)
	,@eddsPerformanceServerName nvarchar(255)
	,@eddsServerName nvarchar(255)
AS
BEGIN

	DECLARE
		@USESQL nvarchar(max),
		@ParmDefinition NVARCHAR(500),
		@DatabaseName NVARCHAR(100),
		@Module NVARCHAR(100) = 'QoS_DBCCCheckServerMonTest',
		@params NVARCHAR (500),
		@key int,
		@loggingVars nvarchar(max);
		
	IF @logging = 1 
	BEGIN
		SET @loggingVars = '@ServerName = ' + QUOTENAME(@currentServerName)
		EXEC eddsdbo.QoS_LogAppend
				@AgentID = @AgentID,
				@module = @Module,
				@taskCompleted = 'Launched QoS_DBCCCheckServerMonTest',
				@otherVars = @loggingVars,
				@nextTask = 'Creating temp tables and locating databases for monitoring'
	END	

	SELECT top 1 @key = KID FROM eddsdbo.QoS_DBCCBACKKEY ORDER by KID DESC

	IF @key = 1 
	BEGIN
		--------------------------------------------------
		--
		--	Create temp tables
		--
		--------------------------------------------------
		-- Create temp table to hold a list of all databases in the instance
		IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_databasesTest]') AND type in (N'U'))
		DROP TABLE EDDSDBO.QoS_databasesTest
		CREATE TABLE EDDSDBO.QoS_databasesTest
		(
			DatabaseID INT IDENTITY (1, 1), Primary Key(DatabaseID),
			DBName nVARCHAR(255) NOT NULL,
			IsCompleted BIT,
			Errors varchar(max)
		)

		-- Create temp table to hold the values of the checkDB lookup
		IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_DBCCServerResultsTest]') AND type in (N'U'))
		DROP TABLE EDDSDBO.QoS_DBCCServerResultsTest
		CREATE TABLE EDDSDBO.QoS_DBCCServerResultsTest
		(
			kdbccbID  INT    IDENTITY ( 1 , 1 ),Primary Key (kdbccbID),
			ServerName nVARCHAR(255),
			DBName nVARCHAR(255),
			CaseArtifactID int,
			LastCleanDBCCDate DATETIME
		)
		
		-----------------------------------------------------------------------------------
		--
		--	Insert all databases that start with "EDDS" into the EDDSDBO.QoS_databases temp table
		--
		-----------------------------------------------------------------------------------
		
		INSERT INTO EDDSDBO.QoS_DatabasesTest (DBName)
		SELECT d.[Database]
		FROM eddsdbo.MockDatabasesChecked as d
		WHERE d.[Server] = @currentServerName
		
		
	IF @logging = 1 
		BEGIN
			SET @loggingVars = @USESQL;
			EXEC eddsdbo.QoS_LogAppend
					@AgentID = @AgentID,
					@module = @Module,
					@taskCompleted = 'Created temp tables and located databases',
					@otherVars = @loggingVars,
					@nextTask = 'Updating database names'
		END	
		------------------------------------------------------------------
		--
		--	Update Database names in the ALLDatabasesChecked table of 
		--	the EDDSPerformance database in the primary SQL instance.
		--
		------------------------------------------------------------------
		-- QoS_ALLDatabasesChecked Initialization 
		INSERT INTO eddsdbo.QoS_ALLDatabasesChecked 
			(
				servername
				,dbname
				,CaseArtifactID
				,datecreated
			) 
			SELECT @currentServerName
			, d.[Database]
				, CASE WHEN ISNUMERIC(REPLACE(d.[Database], 'EDDS', '')) = 1 THEN CAST(REPLACE(d.[Database], 'EDDS', '') AS int)
					WHEN d.[Database] = 'EDDS' THEN -1
					ELSE -2
				END
				, d.[CreatedOn]
			FROM eddsdbo.MockDatabasesChecked as d
			WHERE d.Server = @currentServerName
		
		IF @logging = 1 
		BEGIN
			EXEC eddsdbo.QoS_LogAppend
					@AgentID = @AgentID,
					@module = @Module,
					@taskCompleted = 'Updated database names',
					@nextTask = 'Retrieving DBCC CHECKDB results'
		END	
		
		------------------------------------------------------------------
		--
		--	Retrieve DBCC CheckDB results and
		--	update the EDDSDBO.QoS_DBCCResults table
		--
		------------------------------------------------------------------
		
		DECLARE @Id INT
		DECLARE @tempDBName NVARCHAR(255)
		
		IF (@useDbccDbinfo = 1)
		BEGIN
			WHILE EXISTS (SELECT TOP 1 1 FROM EDDSDBO.QoS_DatabasesTest WHERE IsCompleted IS NULL)
			BEGIN
				SELECT TOP 1 @Id = DatabaseID, @tempDBName = DBName
				FROM EDDSDBO.QoS_databasesTest
				WHERE IsCompleted IS NULL
		
				BEGIN TRY
				-- Insert the mock values into the EDDSDBO.QoS_DBCCResults table
					INSERT INTO EDDSDBO.QoS_DBCCServerResultsTest (ServerName, DBName, CaseArtifactID, LastCleanDBCCDate)
					SELECT
						@currentServerName,
						@tempDBName,
						CASE WHEN ISNUMERIC(REPLACE(@tempDBName, 'EDDS', '')) = 1 THEN CAST(REPLACE(@tempDBName, 'EDDS', '') AS int)
							WHEN @tempDBName = 'EDDS' THEN -1
							ELSE -2
						END,
						LastCleanDBCCDate
					FROM EDDSDBO.MockDbccServerResults msr
					WHERE msr.[Server] = @currentServerName and msr.[Database] = @tempDBName
					and LastCleanDBCCDate > ISNULL((select top 1 LastCleanDBCCDate from EDDSDBO.QoS_DBCCServerResultsTest order by LastCleanDBCCDate desc), cast('17530101' as datetime))
					order by LastCleanDBCCDate desc
					
					UPDATE EDDSDBO.QoS_databasesTest
					SET IsCompleted = 1
					WHERE DatabaseID = @Id
				END TRY
				BEGIN CATCH
					UPDATE eddsdbo.QoS_databasesTest
					SET IsCompleted = 0, Errors = ERROR_MESSAGE()
					WHERE DatabaseID = @Id
								
					IF @logging = 1
					BEGIN
						SET @loggingVars = ERROR_MESSAGE();
						EXEC eddsdbo.QoS_LogAppend
								@AgentID = @AgentID,
								@module = @Module,
								@taskCompleted = 'Encountered an error running DBCC DBINFO',
								@otherVars = @loggingVars,
								@nextTask = 'Attempting to retrieve remaining DBCC CHECKDB results'
					END
				END CATCH
			END
		END
		ELSE
		BEGIN
			IF @logging = 1
			BEGIN
				SET @loggingVars = '@useDbccDbinfo = ' + CONVERT(nvarchar, @useDbccDbinfo, 121);
				EXEC eddsdbo.QoS_LogAppend
						@AgentID = @AgentID,
						@module = @Module,
						@taskCompleted = 'Skipping DBCC DBINFO calls in QoS_DatabaseDBCCMonitor',
						@otherVars = @loggingVars,
						@nextTask = 'Finalizing DBCC results for this server'
			END
		END
		
		--Indicate failed databases
		IF @logging = 1 
		BEGIN
			SET @loggingVars = @USESQL;
			EXEC eddsdbo.QoS_LogAppend
					@AgentID = @AgentID,
					@module = @Module,
					@taskCompleted = 'Retrieved DBCC CHECKDB results',
					@otherVars = @loggingVars,
					@nextTask = 'Gathering DBCC monitoring status information to primary server'
		END	
			
		INSERT INTO EDDSDBO.QoS_FailedDatabases
		(
			DBName,
			ServerName,
			Errors
		) 
		SELECT
			DBName,
			@currentServerName,
			Errors
		FROM EDDSDBO.QoS_DatabasesTest
		WHERE IsCompleted = 0
		ORDER BY DBName
		
		------------------------------------------------------------------
		--
		--	Get the date created of every database
		--	and update the ALLDatabasesChecked table on the primary
		--	SQL instance
		--
		------------------------------------------------------------------
		IF @logging = 1
		BEGIN
			SET @loggingVars = @USESQL;
			EXEC eddsdbo.QoS_LogAppend
					@AgentID = @AgentID,
					@module = @Module,
					@taskCompleted = 'Gathered DBCC monitoring status information to primary server',
					@otherVars = @loggingVars,
					@nextTask = 'Checking database creation dates'
		END
			
		UPDATE adc 
		SET adc.dateCreated = sd.create_date
		FROM sys.databases sd
		INNER JOIN eddsdbo.QoS_AllDatabasesChecked adc 
		on adc.DBName COLLATE DATABASE_DEFAULT = sd.[name]		
		
		IF @logging = 1
		BEGIN
			EXEC eddsdbo.QoS_LogAppend
					@AgentID = @AgentID,
					@module = @Module,
					@taskCompleted = 'Checked database creation dates',
					@nextTask = 'Pushing backup/DBCC results to primary SQL server'
		END

		--Push the DBCC results from the temp table to the QoS_DBCCHistory table on the primary SQL instance
		BEGIN TRY
			INSERT INTO EDDSDBO.QoS_DBCCHistory
					(DBName, CompletedOn, IsCommandBased)
				SELECT
					DbName,
					LastCleanDBCCDate,
					1
				FROM EDDSDBO.QoS_DBCCServerResultsTest R
				WHERE LastCleanDBCCDate NOT IN
					(SELECT CompletedOn FROM eddsdbo.QoS_DBCCHistory WHERE DBName = R.DBName)
		END TRY
		BEGIN CATCH
			IF @logging = 1 
			BEGIN
				SET @loggingVars = ERROR_MESSAGE();
				EXEC eddsdbo.QoS_LogAppend
						@AgentID = @AgentID,
						@module = @Module,
						@taskCompleted = 'Failed to push DBCC results to primary server',
						@otherVars = @loggingVars,
						@nextTask = 'Pushing incremental backup data to primary SQL server'
			END	
		END CATCH		
	
	IF @logging = 1 
	BEGIN
		SET @loggingVars = 'Server: ' + QUOTENAME(@currentServerName)
		EXEC eddsdbo.QoS_LogAppend
				@AgentID = @AgentID,
				@module = @Module,
				@taskCompleted = 'Completed backup/DBCC data collection',
				@otherVars = @loggingVars,
				@nextTask = 'Returning control to QoS_BackupAndDBCCCheckMonLauncher'
	END
	
	END
END
