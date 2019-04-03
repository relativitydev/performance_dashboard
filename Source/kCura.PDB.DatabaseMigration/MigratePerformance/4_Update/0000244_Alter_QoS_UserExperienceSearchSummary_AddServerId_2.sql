USE [EDDSPerformance]
GO

IF EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'QoS_UserExperienceSearchSummary' AND TABLE_SCHEMA = N'EDDSDBO') AND COL_LENGTH ('eddsdbo.QoS_UserExperienceSearchSummary' ,'ServerID') IS NULL
BEGIN
	ALTER TABLE eddsdbo.QoS_UserExperienceSearchSummary
    ADD ServerID int
END