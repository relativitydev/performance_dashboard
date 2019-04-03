USE EDDSPerformance
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_Ratings]') AND type in (N'U'))
BEGIN
	IF COL_LENGTH('eddsdbo.QoS_Ratings', 'RowHash') IS NULL
	BEGIN
		ALTER TABLE eddsdbo.QoS_Ratings ADD RowHash binary(20)
	END
END

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_CasesToAudit]') AND type in (N'U'))
BEGIN
	IF COL_LENGTH('eddsdbo.QoS_CasesToAudit', 'RowHash') IS NULL
	BEGIN
		ALTER TABLE eddsdbo.QoS_CasesToAudit ADD RowHash binary(20)
	END
END

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_SampleHistory]') AND type in (N'U'))
BEGIN
	IF COL_LENGTH('eddsdbo.QoS_SampleHistory', 'RowHash') IS NULL
	BEGIN
		ALTER TABLE eddsdbo.QoS_SampleHistory ADD RowHash binary(20)
	END
END

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_SystemLoadSummary]') AND type in (N'U'))
BEGIN
	IF COL_LENGTH('eddsdbo.QoS_SystemLoadSummary', 'RowHash') IS NULL
	BEGIN
		ALTER TABLE eddsdbo.QoS_SystemLoadSummary ADD RowHash binary(20)
	END
END

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_UptimeRatings]') AND type in (N'U'))
BEGIN
	IF COL_LENGTH('eddsdbo.QoS_UptimeRatings', 'RowHash') IS NULL
	BEGIN
		ALTER TABLE eddsdbo.QoS_UptimeRatings ADD RowHash binary(20)
	END
END

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_UserXInstanceSummary]') AND type in (N'U'))
BEGIN
	IF COL_LENGTH('eddsdbo.QoS_UserXInstanceSummary', 'RowHash') IS NULL
	BEGIN
		ALTER TABLE eddsdbo.QoS_UserXInstanceSummary ADD RowHash binary(20)
	END
END

GO

