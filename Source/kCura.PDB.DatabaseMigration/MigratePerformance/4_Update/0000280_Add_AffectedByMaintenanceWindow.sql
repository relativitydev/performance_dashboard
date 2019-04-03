USE [EDDSPerformance];

IF COL_LENGTH ('eddsdbo.QoS_UptimeRatings' ,'AffectedByMaintenanceWindow') IS NULL
BEGIN
	ALTER TABLE eddsdbo.[QoS_UptimeRatings]
	ADD [AffectedByMaintenanceWindow] bit NOT NULL DEFAULT(0)
END