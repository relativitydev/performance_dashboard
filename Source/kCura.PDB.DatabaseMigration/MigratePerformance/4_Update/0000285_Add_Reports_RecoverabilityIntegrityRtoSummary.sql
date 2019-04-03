-- EDDSPerformance 
IF NOT EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'Reports_RecoverabilityIntegrityRtoSummary' AND TABLE_SCHEMA = N'EDDSDBO') 
BEGIN
	CREATE TABLE eddsdbo.Reports_RecoverabilityIntegrityRtoSummary (
		Id INT IDENTITY(1,1),
		CONSTRAINT PK_Reports_RecoverabilityIntegrityRtoSummary PRIMARY KEY (Id),
		HourId INT,
		WorstRtoDatabase NVARCHAR(255),
		RtoTimeToRecover DECIMAL (6,2)
	)
END
GO