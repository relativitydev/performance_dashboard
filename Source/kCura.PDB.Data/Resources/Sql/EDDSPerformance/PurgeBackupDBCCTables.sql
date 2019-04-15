USE EDDSPerformance;
GO

IF EXISTS(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_AllDatabasesChecked]') AND type in (N'U'))
	TRUNCATE TABLE eddsdbo.QoS_AllDatabasesChecked;
	
IF EXISTS(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_DBCCBACKKEY]') AND type in (N'U'))
	TRUNCATE TABLE eddsdbo.QoS_DBCCBACKKEY;
	
IF EXISTS(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_BackResults]') AND type in (N'U'))
	TRUNCATE TABLE eddsdbo.QoS_BackResults;
	
IF EXISTS(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_BackSummary]') AND type in (N'U'))
	TRUNCATE TABLE eddsdbo.QoS_BackSummary;
	
IF EXISTS(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_DBCCResults]') AND type in (N'U'))
	TRUNCATE TABLE eddsdbo.QoS_DBCCResults;
	
IF EXISTS(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_DBCCSummary]') AND type in (N'U'))
	TRUNCATE TABLE eddsdbo.QoS_DBCCSummary;
	
IF EXISTS(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_DBInfo]') AND type in (N'U'))
	TRUNCATE TABLE eddsdbo.QoS_DBInfo;
	
IF EXISTS(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_tempDBCCResults]') AND type in (N'U'))
	TRUNCATE TABLE eddsdbo.QoS_tempDBCCResults;
	
IF EXISTS(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_tempServers]') AND type in (N'U'))
	TRUNCATE TABLE eddsdbo.QoS_tempServers;
	
IF EXISTS(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_MonitoringExclusions]') AND type IN (N'U'))
	TRUNCATE TABLE eddsdbo.QoS_MonitoringExclusions;

IF EXISTS(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_RecoverabilityIntegritySummary]') AND type IN (N'U'))
	TRUNCATE TABLE eddsdbo.QoS_RecoverabilityIntegritySummary;

IF EXISTS(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_GapSummary]') AND type IN (N'U'))
	TRUNCATE TABLE eddsdbo.QoS_GapSummary;

IF EXISTS(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_RecoverabilityIntegritySummary]') AND type IN (N'U'))
	TRUNCATE TABLE eddsdbo.QoS_RecoveryObjectiveSummary;

--Force Backup/DBCC monitoring task to run ASAP after the purge
UPDATE EDDSperformance.eddsdbo.ProcessControl
SET LastProcessExecDateTime = DATEADD(dd, -1, LastProcessExecDateTime)
WHERE ProcessControlID = 10