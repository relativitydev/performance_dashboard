use EDDSQoS

declare @sqlversion varchar(250)
declare @tempDbFileCount int
declare @lastSqlRestart datetime
declare @adhocWorkloads int
declare @maxDegreeOfParallelism int
declare @maxServerMemory int

declare @sql nvarchar(max)

--sql server version
set @sqlversion = (SELECT CAST( SERVERPROPERTY ('edition') AS VARCHAR(MAX) ) + ' ' + CAST( SERVERPROPERTY('productversion') AS VARCHAR(MAX) ) + ' ' +CAST( SERVERPROPERTY ('productlevel') AS VARCHAR(MAX) ) )

--temp db file count
set @tempDbFileCount = (SELECT COUNT(file_id) as filecount FROM tempdb.sys.database_files WHERE [type] = 0)

--last sql restart
set @lastSqlRestart = (SELECT Convert(datetime, CrDate) as DateServerRebooted from sys.sysdatabases WHERE dbid = 2)

--ad hoc
set @adhocWorkloads = (SELECT convert(int, value) FROM sys.configurations WHERE name = 'optimize for ad hoc workloads')

--max degree of parallelism
set @maxDegreeOfParallelism = (SELECT convert(int, value) FROM sys.configurations WHERE name = 'max degree of parallelism')

--max server memory
set @maxServerMemory = (SELECT convert(int, value) FROM sys.configurations WHERE name = 'max server memory (MB)')

select  @sqlversion as [SQLVersion]
		,@adhocWorkloads as [AdHocWorkLoad]
		,@maxServerMemory as [MaxServerMemory]
		,@maxDegreeOfParallelism as [MaxDegreeOfParallelism]
		,@tempDbFileCount as [TempDBDataFiles]
		,@lastSqlRestart as [LastSQLRestart]