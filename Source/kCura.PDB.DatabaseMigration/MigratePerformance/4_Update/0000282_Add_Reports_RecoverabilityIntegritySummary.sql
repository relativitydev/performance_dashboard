-- EDDSPerformance 
IF NOT EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'Reports_RecoverabilityIntegritySummary' AND TABLE_SCHEMA = N'EDDSDBO') 
BEGIN
	CREATE TABLE eddsdbo.Reports_RecoverabilityIntegritySummary (
		Id INT IDENTITY(1,1),
		CONSTRAINT PK_Reports_RecoverabilityIntegritySummary PRIMARY KEY (Id),
		HourId INT,
		OverallScore DECIMAL (5,2),
		RpoScore DECIMAL (5,2),
		RtoScore DECIMAL (5,2),
		BackupFrequencyScore DECIMAL (5,2),
		BackupCoverageScore DECIMAL (5,2),
		DbccFrequencyScore DECIMAL (5,2),
		DbccCoverageScore DECIMAL (5,2)
	)
END
GO