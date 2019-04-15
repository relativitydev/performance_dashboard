USE EDDSPerformance;
GO

IF EXISTS(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_CasesToAudit]') AND type in (N'U'))
	TRUNCATE TABLE eddsdbo.QoS_CasesToAudit;

IF EXISTS(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_GlassRunHistory]') AND type in (N'U'))
	TRUNCATE TABLE eddsdbo.QoS_GlassRunHistory;

IF EXISTS(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_GlassRunLog]') AND type in (N'U'))
	TRUNCATE TABLE eddsdbo.QoS_GlassRunLog;

IF EXISTS(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_Ratings]') AND type in (N'U'))
	TRUNCATE TABLE eddsdbo.QoS_Ratings;

IF EXISTS(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_SampleHistory]') AND type in (N'U'))
	TRUNCATE TABLE eddsdbo.QoS_SampleHistory;
	
IF EXISTS(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_ActiveHours]') AND type in (N'U'))
	TRUNCATE TABLE eddsdbo.QoS_ActiveHours;
	
IF EXISTS(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_Servers]') AND type in (N'U'))
	TRUNCATE TABLE eddsdbo.QoS_Servers;
	
IF EXISTS(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_SourceDatetime]') AND type in (N'U'))
	TRUNCATE TABLE eddsdbo.QoS_SourceDatetime;
	
IF EXISTS(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_SystemLoadSummary]') AND type in (N'U'))
	TRUNCATE TABLE eddsdbo.QoS_SystemLoadSummary;
	
IF EXISTS(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_UptimeRatings]') AND type in (N'U'))
	TRUNCATE TABLE eddsdbo.QoS_UptimeRatings;
	
IF EXISTS(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_UserExperienceServerSummary]') AND type in (N'U'))
	TRUNCATE TABLE eddsdbo.QoS_UserExperienceServerSummary;
	
IF EXISTS(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_UserExperienceWorkspaceSummary]') AND type in (N'U'))
	TRUNCATE TABLE eddsdbo.QoS_UserExperienceWorkspaceSummary;

IF EXISTS(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_UserExperienceSearchSummary]') AND type in (N'U'))
	TRUNCATE TABLE eddsdbo.QoS_UserExperienceSearchSummary;

IF EXISTS(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_UserXInstanceSummary]') AND type in (N'U'))
	TRUNCATE TABLE eddsdbo.QoS_UserXInstanceSummary;
	
IF EXISTS(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_VarscatOutputCumulative]') AND type in (N'U'))
	TRUNCATE TABLE eddsdbo.QoS_VarscatOutputCumulative;
	
IF EXISTS(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_VarscatOutputDetailCumulative]') AND type in (N'U'))
	TRUNCATE TABLE eddsdbo.QoS_VarscatOutputDetailCumulative;

--Force QoS_CasesToAudit task to run ASAP after the purge
UPDATE EDDSperformance.eddsdbo.ProcessControl
SET LastProcessExecDateTime = DATEADD(hh, DATEDIFF(hh, 0, getutcdate()) - 1, 0)
WHERE ProcessControlID = 17