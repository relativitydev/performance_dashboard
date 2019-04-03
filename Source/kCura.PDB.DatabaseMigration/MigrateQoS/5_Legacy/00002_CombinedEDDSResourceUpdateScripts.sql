DECLARE @dbname nvarchar(128)
SET @dbname = N'EDDSResource'

declare @IsReadOnly bit
SELECT @IsReadOnly = case when DATABASEPROPERTYEX('EDDSResource', 'Updateability') = 'READ_ONLY' then  1 else 0 end

IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE '[' + name + ']' = @dbname OR name = @dbname) and @IsReadOnly = 0
Begin

declare @sql nvarchar(max) = N'
USE EDDSResource

IF EXISTS(SELECT TOP 1 * FROM sys.procedures pr
	INNER JOIN sys.schemas s
	ON pr.schema_id = s.schema_id
	WHERE s.name = ''dbo'' AND pr.name = ''kIE_BackupAndDBCCCheck'')
BEGIN
	DROP PROCEDURE dbo.kIE_BackupAndDBCCCheck;
END


IF EXISTS(SELECT TOP 1 * FROM sys.procedures pr
	INNER JOIN sys.schemas s
	ON pr.schema_id = s.schema_id
	WHERE s.name = ''dbo'' AND pr.name = ''kIE_BackupAndDBCCCheckServer'')
BEGIN
	DROP PROCEDURE dbo.kIE_BackupAndDBCCCheckServer;
END


IF EXISTS(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N''[dbo].[AllDatabasesChecked]'') AND type in (N''U''))
	DROP TABLE dbo.AllDatabasesChecked;
	
IF EXISTS(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N''[dbo].[DBCCBACKKEY]'') AND type in (N''U''))
	DROP TABLE dbo.DBCCBACKKEY;
	
IF EXISTS(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N''[dbo].[kIE_BackResults]'') AND type in (N''U''))
	DROP TABLE dbo.kIE_BackResults;
	
IF EXISTS(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N''[dbo].[kIE_BackSummary]'') AND type in (N''U''))
	DROP TABLE dbo.kIE_BackSummary;
	
IF EXISTS(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N''[dbo].[kIE_DBCCResults]'') AND type in (N''U''))
	DROP TABLE dbo.kIE_DBCCResults;
	
IF EXISTS(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N''[dbo].[kIE_DBCCSummary]'') AND type in (N''U''))
	DROP TABLE dbo.kIE_DBCCSummary;
	
IF EXISTS(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N''[eddsdbo].[QoS_BackResults]'') AND type in (N''U''))
	DROP TABLE eddsdbo.QoS_BackResults;
	
IF EXISTS(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N''[eddsdbo].[QoS_databases]'') AND type in (N''U''))
	DROP TABLE eddsdbo.QoS_databases;
	
IF EXISTS(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N''[eddsdbo].[QoS_DBCCHistory]'') AND type in (N''U''))
	DROP TABLE eddsdbo.QoS_DBCCHistory;
	
IF EXISTS(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N''[eddsdbo].[QoS_DBCCResults]'') AND type in (N''U''))
	DROP TABLE eddsdbo.QoS_DBCCResults;
	
IF EXISTS(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N''[eddsdbo].[QoS_DBInfo]'') AND type in (N''U''))
	DROP TABLE eddsdbo.QoS_DBInfo;
	
IF EXISTS(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N''[eddsdbo].[QoS_tempDBCCResults]'') AND type in (N''U''))
	DROP TABLE eddsdbo.QoS_tempDBCCResults;
	
IF EXISTS(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N''[eddsdbo].[QoS_tempServers]'') AND type in (N''U''))
	DROP TABLE eddsdbo.QoS_tempServers;
	
IF EXISTS(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N''[eddsdbo].[QoS_MonitoringExclusions]'') AND type in (N''U''))
	DROP TABLE eddsdbo.QoS_MonitoringExclusions;
	
IF EXISTS(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N''[eddsdbo].[QoS_FailedDatabases]'') AND type in (N''U''))
	DROP TABLE eddsdbo.QoS_FailedDatabases;
	
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N''[dbo].[kIE_BackupAndDBCCCheckServerMon]'') AND type in (N''P'', N''PC''))
	DROP PROCEDURE [dbo].[kIE_BackupAndDBCCCheckServerMon]

'
EXEC sp_executesql @SQL

End
