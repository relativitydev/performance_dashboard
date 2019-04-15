USE EDDSQoS;
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_ConcurrencyItems]') AND type in (N'U'))
	TRUNCATE TABLE eddsdbo.QoS_ConcurrencyItems;
	
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_ConcurrencyItemsSeconds]') AND type in (N'U'))
	TRUNCATE TABLE eddsdbo.QoS_ConcurrencyItemsSeconds;
	
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_HourlyWaitStats]') AND type in (N'U'))
	TRUNCATE TABLE eddsdbo.QoS_HourlyWaitStats;
	
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_HourlyWaitStatsMaster]') AND type in (N'U'))
	TRUNCATE TABLE eddsdbo.QoS_HourlyWaitStatsMaster;
	
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_UserXServerSummary]') AND type in (N'U'))
	TRUNCATE TABLE eddsdbo.QoS_UserXServerSummary;

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_VarscatOutput]') AND type in (N'U'))
	TRUNCATE TABLE eddsdbo.QoS_VarscatOutput;

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_VarscatOutputDetail]') AND type in (N'U'))
	TRUNCATE TABLE eddsdbo.QoS_VarscatOutputDetail;
	
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_Waits]') AND type in (N'U'))
	TRUNCATE TABLE eddsdbo.QoS_Waits;