USE [EDDSPerformance]
GO

IF COL_LENGTH ('eddsdbo.EnvironmentCheckRecommendations' ,'Severity') IS NULL
BEGIN
    ALTER TABLE eddsdbo.EnvironmentCheckRecommendations
    ADD Severity int null
END