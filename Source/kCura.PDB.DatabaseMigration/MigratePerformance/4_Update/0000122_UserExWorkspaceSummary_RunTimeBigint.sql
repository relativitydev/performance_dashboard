USE EDDSPerformance
GO

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'QoS_UserExperienceWorkspaceSummary' AND TABLE_SCHEMA = 'eddsdbo')
BEGIN
	ALTER TABLE eddsdbo.QoS_UserExperienceWorkspaceSummary
	ALTER COLUMN TotalRunTime BIGINT
END