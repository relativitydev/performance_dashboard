USE [EDDSPerformance];

IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_GlassRunLog]') AND name = N'NCQoS_RunID')
BEGIN
	DROP INDEX NCQoS_RunID ON eddsdbo.QoS_GlassRunLog
END

IF COL_LENGTH ('eddsdbo.QoS_GlassRunLog' ,'GlassRunID') IS NOT NULL
BEGIN
	ALTER TABLE eddsdbo.[QoS_GlassRunLog]
	drop column [GlassRunID]
END

IF COL_LENGTH ('eddsdbo.QoS_GlassRunLog' ,'RunTimeUTC') IS NOT NULL
BEGIN
	ALTER TABLE eddsdbo.[QoS_GlassRunLog]
	drop column [RunTimeUTC]
END

IF COL_LENGTH ('eddsdbo.QoS_GlassRunLog' ,'LogIntervalDurationMS') IS NOT NULL
BEGIN
	ALTER TABLE eddsdbo.[QoS_GlassRunLog]
	drop column [LogIntervalDurationMS]
END

IF COL_LENGTH ('eddsdbo.QoS_GlassRunLog' ,'RunDurationMS') IS NOT NULL
BEGIN
	ALTER TABLE eddsdbo.[QoS_GlassRunLog]
	drop column [RunDurationMS]
END