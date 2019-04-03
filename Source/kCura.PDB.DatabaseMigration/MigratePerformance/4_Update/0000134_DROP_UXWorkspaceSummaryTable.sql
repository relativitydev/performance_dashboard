USE EDDSPerformance
GO

IF EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'QoS_UserExperienceWorkspaceSummary' AND TABLE_SCHEMA = 'eddsdbo') 
BEGIN
	DROP TABLE eddsdbo.QoS_UserExperienceWorkspaceSummary
END