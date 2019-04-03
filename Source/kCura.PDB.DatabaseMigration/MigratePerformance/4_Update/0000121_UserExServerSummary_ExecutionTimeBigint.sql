USE EDDSPerformance
GO

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'QoS_UserExperienceServerSummary' AND TABLE_SCHEMA = 'eddsdbo')
BEGIN
	ALTER TABLE eddsdbo.QoS_UserExperienceServerSummary
	ALTER COLUMN TotalExecutionTime BIGINT
END