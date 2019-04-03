USE [PDBResource] 
 begin transaction


--CheckInstantFileInitialization.sql


GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[CheckInstantFileInitialization]') AND type in (N'P', N'PC'))
	DROP PROCEDURE eddsdbo.CheckInstantFileInitialization

GO

CREATE PROCEDURE eddsdbo.CheckInstantFileInitialization
	@mdfPath nvarchar(max),
	@ldfPath nvarchar(max),
	@enabled [bit] OUTPUT
WITH EXEC AS SELF
AS
BEGIN
	
	-- FROM: http://sqlblog.com/blogs/tibor_karaszi/archive/2013/10/30/check-for-instant-file-initialization.aspx
	
	-- *** WARNING: Undocumented commands used in this script !!! *** --

	--Exit if a database named DummyTestDB exists
	IF DB_ID('EDDSPerformance_DummyDB') IS NOT NULL
	BEGIN
	  --RAISERROR('A database named EDDSPerformance_DummyDB already exists, exiting script', 20, 1) WITH LOG
	  DROP DATABASE EDDSPerformance_DummyDB;
	END

	--Temptable to hold output from sp_readerrorlog
	IF OBJECT_ID('tempdb..#SqlLogs') IS NOT NULL DROP TABLE #SqlLogs
	
	CREATE TABLE #SqlLogs(LogDate datetime2(0), ProcessInfo VARCHAR(20), TEXT VARCHAR(MAX))

	--Turn on trace flags 3004 and 3605
	DBCC TRACEON(3004, 3605, -1) WITH NO_INFOMSGS

	--Create a dummy database to see the output in the SQL Server Errorlog
	declare @SQL nvarchar(max)

	set @SQL = N'
	CREATE DATABASE EDDSPerformance_DummyDB
	ON   
	( NAME = EDDSPerformance_DummyDB_dat,  
		FILENAME = ''' + @mdfPath + 'EDDSPerformance_DummyDB' + convert(nvarchar(50), newid()) + '.mdf'',  
		MAXSIZE = UNLIMITED,  
		FILEGROWTH = 5 )  
	LOG ON  
	( NAME = EDDSPerformance_DummyDB_log,  
	   FILENAME = ''' + @ldfPath + 'EDDSPerformance_DummyDBlog' + convert(nvarchar(50), newid()) + '.ldf'',
		MAXSIZE = UNLIMITED,  
		FILEGROWTH = 5MB ) ; 
		ALTER DATABASE EDDSPerformance_DummyDB SET RECOVERY SIMPLE; '

	exec sp_executesql @SQL
	
	--Turn off trace flags 3004 and 3605
	DBCC TRACEOFF(3004, 3605, -1) WITH NO_INFOMSGS

	--Remove the DummyDB
	DROP DATABASE EDDSPerformance_DummyDB;

	--Now check the output in the SQL Server Error Log File
	--This can take a while if you have a large errorlog file
	INSERT INTO #SqlLogs(LogDate, ProcessInfo, TEXT)
	EXEC sp_readerrorlog 0, 1, 'Zeroing'

	IF EXISTS(
			SELECT * FROM #SqlLogs
			WHERE TEXT LIKE 'Zeroing completed%'
			AND TEXT LIKE '%EDDSPerformance_DummyDB.mdf%'
			AND LogDate > DATEADD(HOUR, -1, LogDate)
		)
	BEGIN
		set @enabled = 0 --Instant file initialization is NOT set.
	END
	ELSE
	BEGIN
		set @enabled = 1 --Instant file initialization is set.
	END
END

--DatabaseJoinedToGroup.sql


GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[DatabaseJoinedToGroup]') AND type in (N'P', N'PC'))
	DROP PROCEDURE eddsdbo.DatabaseJoinedToGroup

GO

CREATE PROCEDURE [eddsdbo].[DatabaseJoinedToGroup]
	@databaseName SYSNAME,
	@availabilityGroup nvarchar(250)
WITH EXECUTE AS SELF AS
BEGIN	
	/*
	 * Determines if a given database is joined to an availabilty group or not
	 */
	DECLARE @databaseServerIsJoined bit = 

	CASE WHEN 
	EXISTS(
		SELECT database_name
		FROM sys.availability_groups
		INNER JOIN sys.availability_databases_cluster ON availability_databases_cluster.group_id = availability_groups.group_id
		WHERE sys.availability_groups.name = @availabilityGroup AND sys.availability_databases_cluster.database_name = @databaseName) 
	THEN 1 ELSE 0 END;

	SELECT @databaseServerIsJoined
END

--QoS_CycleErrorLog.sql


GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_CycleErrorLog]') AND type in (N'P', N'PC'))
	DROP PROCEDURE eddsdbo.QoS_CycleErrorLog

GO

CREATE PROCEDURE eddsdbo.QoS_CycleErrorLog
WITH EXEC AS SELF
AS
BEGIN
	EXEC sp_cycle_errorlog;
END

--QoS_DatabaseDBCCMonitor.sql


GO

/****** Object:  StoredProcedure [dbo].[kIE_BackupAndDBCCCheckServerMon]    Script Date: 05/05/2014 12:56:37 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_DatabaseDBCCMonitor]') AND type in (N'P', N'PC'))
DROP PROCEDURE [eddsdbo].[QoS_DatabaseDBCCMonitor]
GO

CREATE PROCEDURE [eddsdbo].[QoS_DatabaseDBCCMonitor]
	@tempDBName NVARCHAR(255),
	@currentServerName NVARCHAR(255)
WITH EXEC AS SELF
AS
BEGIN
	INSERT INTO EDDSQoS.EDDSDBO.QoS_DBInfo
	EXECUTE('DBCC DBINFO([' + @tempDBName + ']) WITH TABLERESULTS, NO_INFOMSGS') 
	
	-- Pull out object for last known good dbcc checkDB from EDDSDBO.QoS_DBInfo temp table
	-- Insert the values into the EDDSDBO.QoS_DBCCResults table
	INSERT INTO EDDSQoS.EDDSDBO.QoS_DBCCServerResults (ServerName, DBName, CaseArtifactID, LastCleanDBCCDate)
	SELECT TOP 1
		@currentServerName,
		@tempDBName,
		CASE WHEN ISNUMERIC(REPLACE(@tempDBName, 'EDDS', '')) = 1 THEN CAST(REPLACE(@tempDBName, 'EDDS', '') AS int)
			WHEN @tempDBName = 'EDDS' THEN -1
			ELSE -2
		END,
		Value
	FROM EDDSQoS.EDDSDBO.QoS_DBInfo WHERE Field = 'dbi_dbccLastKnownGood'
END

--QoS_ReadErrorLog.sql


GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_ReadErrorLog]') AND type in (N'P', N'PC'))
	DROP PROCEDURE eddsdbo.QoS_ReadErrorLog

GO

CREATE PROCEDURE eddsdbo.QoS_ReadErrorLog
	@start DATETIME,
	@end DATETIME,
	@searchTerm NVARCHAR(4000)
WITH EXEC AS SELF
AS
BEGIN
	EXEC xp_readerrorlog 0, 1, @searchTerm, NULL, @start, @end
END

--QoS_VirtualLogFileCounter.sql


GO
IF EXISTS (select 1 from sysobjects where [name] = 'QoS_VirtualLogFileCounter' and type = 'P')  
BEGIN
	DROP PROCEDURE eddsdbo.QoS_VirtualLogFileCounter
END
GO
CREATE PROCEDURE eddsdbo.QoS_VirtualLogFileCounter
	@DatabaseName nvarchar(150)
WITH EXEC AS SELF
AS
BEGIN

DECLARE @SQL nvarchar(max) = N'
	USE [' + @DatabaseName + ']

	DBCC LOGINFO

	INSERT INTO EDDSQoS.eddsdbo.VirtualLogFileDW
		(DatabaseName, VirtualLogFiles)
	VALUES
		(@DatabaseName, @@ROWCOUNT)
	',
	@parmDefinition nvarchar(max) = N'@DatabaseName NVARCHAR(150)';

EXEC sp_executesql @SQL, @parmDefinition, @DatabaseName;

END

--ReadAvailabilityGroupName.sql


GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[ReadAvailabilityGroupName]') AND type in (N'P', N'PC'))
	DROP PROCEDURE eddsdbo.ReadAvailabilityGroupName

GO

CREATE PROCEDURE [eddsdbo].[ReadAvailabilityGroupName]
WITH EXECUTE AS SELF AS
BEGIN
	SELECT TOP 1 name FROM sys.availability_groups
END

--RemoveDatabaseFromAvailabilityGroup.sql


GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[RemoveDatabaseFromAvailabilityGroup]') AND type in (N'P', N'PC'))
	DROP PROCEDURE eddsdbo.RemoveDatabaseFromAvailabilityGroup

GO

CREATE PROCEDURE [eddsdbo].[RemoveDatabaseFromAvailabilityGroup]
	@databaseName SYSNAME,
	@availabilityGroup nvarchar(250)
WITH EXECUTE AS SELF AS
BEGIN
	/* 
	 * Removes a database from the given availability group
	 */	 
	--Suspend database mirroring
	DECLARE @sql nvarchar(1000) = 'USE [master] ALTER DATABASE '+ QUOTENAME(@databaseName)+ ' SET HADR SUSPEND'
	EXEC sp_executesql @sql

	--Remove database from availability group
	SET @sql = 'USE [master] ALTER AVAILABILITY GROUP '+ QUOTENAME(@availabilityGroup) + ' REMOVE DATABASE ' + QUOTENAME(@databaseName)
	EXEC sp_executesql @sql
END
 commit transaction
