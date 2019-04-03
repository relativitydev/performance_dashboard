USE EDDSPerformance
GO

IF NOT EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'QoS_RecoverabilityIntegritySummary' AND TABLE_SCHEMA = N'EDDSDBO') 
BEGIN
	CREATE TABLE eddsdbo.QoS_RecoverabilityIntegritySummary (
		RISID INT IDENTITY(1,1),
		CONSTRAINT PK_RecoverabilityIntegritySummary PRIMARY KEY (RISID),
		SummaryDayHour DATETIME NULL,
		RecoverabilityIntegrityScore DECIMAL (5,2) NULL,
		BackupFrequencyScore DECIMAL (5,2) NULL,
		BackupCoverageScore DECIMAL (5,2) NULL,
		DbccFrequencyScore DECIMAL (5,2) NULL,
		DbccCoverageScore DECIMAL (5,2) NULL,
		RPOScore DECIMAL (5,2) NULL,
		RTOScore DECIMAL (5,2) NULL,
		WorstRPODatabase NVARCHAR(255) NULL,
		WorstRTODatabase NVARCHAR(255) NULL,
		PotentialDataLossMinutes INT NULL,
		EstimatedTimeToRecoverHours DECIMAL (6,2) NULL,
		RowHash BINARY(20) NULL
	)
END
GO