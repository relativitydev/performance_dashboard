USE [EDDSPerformance]


IF COL_LENGTH ('eddsdbo.QoS_GlassRunLog' ,'LogLevel') IS NULL
BEGIN
	ALTER TABLE eddsdbo.[QoS_GlassRunLog]
    ADD [LogLevel] int null
END