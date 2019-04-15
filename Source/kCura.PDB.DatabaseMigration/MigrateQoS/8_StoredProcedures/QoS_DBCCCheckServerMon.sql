USE [EDDSQoS]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_BackupAndDBCCCheckServerMon]') AND type in (N'P', N'PC'))
DROP PROCEDURE [eddsdbo].[QoS_BackupAndDBCCCheckServerMon]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_DBCCCheckServerMon]') AND type in (N'P', N'PC'))
DROP PROCEDURE [eddsdbo].[QoS_DBCCCheckServerMon]
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_DBCCServerResults]') AND type in (N'U'))
	DROP TABLE EDDSDBO.QoS_DBCCServerResults
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_BackServerResults]') AND type in (N'U'))
	DROP TABLE EDDSDBO.QoS_BackServerResults
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_databases]') AND type in (N'U'))
	DROP TABLE EDDSDBO.QoS_databases
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [eddsdbo].[QoS_DBCCCheckServerMon]
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
		@Module NVARCHAR(100) = 'QoS_DBCCCheckServerMon',
		@params NVARCHAR (500),
		@key int,
		@loggingVars nvarchar(max);
		
	IF @logging = 1 
	BEGIN
		SET @loggingVars = '@ServerName = ' + QUOTENAME(@currentServerName)
		EXEC eddsdbo.QoS_LogAppend
				@AgentID = @AgentID,
				@module = @Module,
				@taskCompleted = 'Launched QoS_DBCCCheckServerMon',
				@otherVars = @loggingVars,
				@nextTask = 'Creating temp tables and locating databases for monitoring'
				,@eddsPerformanceServerName = @eddsPerformanceServerName
	END	

	SET @params = '@retValOut int OUTPUT'				
	SET @USESQL = 'SELECT top 1 @retvalout = KID FROM ' + QUOTENAME(@eddsPerformanceServerName) + '.EDDSPerformance.eddsdbo.QoS_DBCCBACKKEY ORDER by KID DESC'

	EXEC sp_executeSQL @usesql, @params, @retValOut = @key OUTPUT

	IF @key = 1 
	BEGIN
		--------------------------------------------------
		--
		--	Create temp tables
		--
		--------------------------------------------------
		
		-- Create temp table to hold results from DBINFO for each database
		IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID('[eddsdbo].[QoS_DBInfo]') AND type in (N'U'))
		DROP TABLE EDDSDBO.QoS_DBInfo
		CREATE TABLE EDDSDBO.QoS_DBInfo
		(
			TID  INT  IDENTITY ( 1 , 1 ),Primary Key (TID),
			ParentObject nVARCHAR(255),
			[Object] nVARCHAR(255),
			Field nVARCHAR(255) ,
			[Value] VARCHAR(255)
		)   
		
		-- Create temp table to hold a list of all databases in the instance
		IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_databases]') AND type in (N'U'))
		DROP TABLE EDDSDBO.QoS_databases
		CREATE TABLE EDDSDBO.QoS_databases
		(
			DatabaseID INT IDENTITY (1, 1), Primary Key(DatabaseID),
			DBName nVARCHAR(255) NOT NULL,
			IsCompleted BIT,
			Errors varchar(max)
		)

		-- Create temp table to hold the values of the checkDB lookup
		IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_DBCCServerResults]') AND type in (N'U'))
		DROP TABLE EDDSDBO.QoS_DBCCServerResults
		CREATE TABLE EDDSDBO.QoS_DBCCServerResults
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
		
		SET @USESQL = 
		'
			INSERT INTO EDDSDBO.QoS_Databases (DBName)
			SELECT d.Name
			FROM sys.databases as d
			inner join ' + QUOTENAME(@eddsServerName) + '.edds.eddsdbo.[Case] as c 
			on ''EDDS''+convert(varchar,c.ArtifactID) = d.name 
			or ''INV''+convert(varchar,c.ArtifactID) = d.name 
			or (d.name = ''EDDS'' and c.ArtifactID = -1)
		'
		
		EXEC sp_executeSQL @USESQL;
		
		------------------------------------------------------------------
		--
		--	Update Database names in the ALLDatabasesChecked table of 
		--	the EDDSPerformance database in the primary SQL instance.
		--
		------------------------------------------------------------------
		
		SET @USESQL = 
		'
			INSERT INTO ' + QUOTENAME(@eddsPerformanceServerName) + '.EDDSPerformance.eddsdbo.QoS_ALLDatabasesChecked 
			(
				servername
				,dbname
				,CaseArtifactID
				,datecreated
			) 
			SELECT @currentServerName
			, d.Name
				, CASE WHEN ISNUMERIC(REPLACE(d.Name, ''EDDS'', '''')) = 1 THEN CAST(REPLACE(d.Name, ''EDDS'', '''') AS int)
					WHEN d.Name = ''EDDS'' THEN -1
					ELSE -2
				END
				, create_date 
			FROM sys.databases as d
			inner join ' + QUOTENAME(@eddsServerName) + '.edds.eddsdbo.[Case] as c 
			on ''EDDS''+convert(varchar,c.ArtifactID) = d.name 
			or ''INV''+convert(varchar,c.ArtifactID) = d.name 
			or (d.name = ''EDDS'' and c.ArtifactID = -1)
		'
				
		IF @logging = 1 
		BEGIN
			SET @loggingVars = @USESQL;
			EXEC eddsdbo.QoS_LogAppend
					@AgentID = @AgentID,
					@module = @Module,
					@taskCompleted = 'Created temp tables and located databases',
					@otherVars = @loggingVars,
					@nextTask = 'Updating database names'
					,@eddsPerformanceServerName = @eddsPerformanceServerName
		END	
		
		EXEC sp_executeSQL @USESQL, N'@currentServerName nvarchar(255)', @currentServerName;

		IF @logging = 1 
		BEGIN
			EXEC eddsdbo.QoS_LogAppend
					@AgentID = @AgentID,
					@module = @Module,
					@taskCompleted = 'Updated database names',
					@nextTask = 'Retrieving DBCC CHECKDB results'
					,@eddsPerformanceServerName = @eddsPerformanceServerName
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
			WHILE EXISTS (SELECT TOP 1 1 FROM EDDSDBO.QoS_Databases WHERE IsCompleted IS NULL)
			BEGIN
				SELECT TOP 1 @Id = DatabaseID, @tempDBName = DBName
				FROM EDDSDBO.QoS_databases
				WHERE IsCompleted IS NULL
		
				-- Execute DBINFO for select database and insert into EDDSDBO.QoS_DBInfo temp table
				BEGIN TRY
					EXEC [{{resourcedbname}}].eddsdbo.QoS_DatabaseDBCCMonitor @tempDBName, @currentServerName;

					UPDATE EDDSDBO.QoS_databases
					SET IsCompleted = 1
					WHERE DatabaseID = @Id
				END TRY
				BEGIN CATCH
					UPDATE eddsdbo.QoS_databases
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
								,@eddsPerformanceServerName = @eddsPerformanceServerName
					END
				END CATCH
				
				TRUNCATE TABLE EDDSDBO.QoS_DBInfo
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
						,@eddsPerformanceServerName = @eddsPerformanceServerName
			END
		END
		
		--Indicate failed databases
		SET @USESQL = '
			INSERT INTO ' + QUOTENAME(@eddsPerformanceServerName) + '.EDDSPerformance.EDDSDBO.QoS_FailedDatabases
			(
				DBName,
				ServerName,
				Errors
			) 
			SELECT
				DBName,
				@currentServerName,
				Errors
			FROM EDDSDBO.QoS_Databases
			WHERE IsCompleted = 0
			ORDER BY DBName'
			
		IF @logging = 1 
		BEGIN
			SET @loggingVars = @USESQL;
			EXEC eddsdbo.QoS_LogAppend
					@AgentID = @AgentID,
					@module = @Module,
					@taskCompleted = 'Retrieved DBCC CHECKDB results',
					@otherVars = @loggingVars,
					@nextTask = 'Gathering DBCC monitoring status information to primary server'
					,@eddsPerformanceServerName = @eddsPerformanceServerName
		END	
			
		EXEC sp_executeSQL @USESQL, N'@currentServerName nvarchar(255)', @currentServerName;
		
		------------------------------------------------------------------
		--
		--	Get the date created of every database
		--	and update the ALLDatabasesChecked table on the primary
		--	SQL instance
		--
		------------------------------------------------------------------
		
		SET @USESQL = '
			UPDATE adc 
			SET adc.dateCreated = sd.create_date
			FROM sys.databases sd
			INNER JOIN ' + QUOTENAME(@eddsPerformanceServerName) + '.EDDSPerformance.eddsdbo.QoS_AllDatabasesChecked adc 
			on adc.DBName COLLATE DATABASE_DEFAULT = sd.[name]'
			
		IF @logging = 1
		BEGIN
			SET @loggingVars = @USESQL;
			EXEC eddsdbo.QoS_LogAppend
					@AgentID = @AgentID,
					@module = @Module,
					@taskCompleted = 'Gathered DBCC monitoring status information to primary server',
					@otherVars = @loggingVars,
					@nextTask = 'Checking database creation dates'
					,@eddsPerformanceServerName = @eddsPerformanceServerName
		END
			
		EXEC sp_executeSQL @USESQL;
		
		IF @logging = 1
		BEGIN
			EXEC eddsdbo.QoS_LogAppend
					@AgentID = @AgentID,
					@module = @Module,
					@taskCompleted = 'Checked database creation dates',
					@nextTask = 'Pushing backup/DBCC results to primary SQL server'
					,@eddsPerformanceServerName = @eddsPerformanceServerName
		END

		--Push the DBCC results from the temp table to the QoS_DBCCHistory table on the primary SQL instance
		SET @USESQL = 'INSERT INTO ' + QUOTENAME(@eddsPerformanceServerName) + '.EDDSPerformance.EDDSDBO.QoS_DBCCHistory
					(DBName, CompletedOn, IsCommandBased)
				SELECT
					DbName,
					LastCleanDBCCDate,
					1
				FROM EDDSDBO.QoS_DBCCServerResults R
				WHERE LastCleanDBCCDate NOT IN
					(SELECT CompletedOn FROM ' + QUOTENAME(@eddsPerformanceServerName) + '.EDDSPerformance.eddsdbo.QoS_DBCCHistory WHERE DBName = R.DBName)'

		BEGIN TRY
			EXEC sp_executeSQL @USESQL;
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
						,@eddsPerformanceServerName = @eddsPerformanceServerName
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
				,@eddsPerformanceServerName = @eddsPerformanceServerName
	END
	
	END
END
