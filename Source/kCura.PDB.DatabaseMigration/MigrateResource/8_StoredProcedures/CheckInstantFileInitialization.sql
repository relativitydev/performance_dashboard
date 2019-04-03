USE [{{resourcedbname}}]
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
		FILENAME = ''' + Replace(@mdfPath, '''', '') + 'EDDSPerformance_DummyDB' + convert(nvarchar(50), newid()) + '.mdf'',  
		MAXSIZE = UNLIMITED,  
		FILEGROWTH = 5 )  
	LOG ON  
	( NAME = EDDSPerformance_DummyDB_log,  
	   FILENAME = ''' + Replace(@ldfPath, '''', '') + 'EDDSPerformance_DummyDBlog' + convert(nvarchar(50), newid()) + '.ldf'',
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