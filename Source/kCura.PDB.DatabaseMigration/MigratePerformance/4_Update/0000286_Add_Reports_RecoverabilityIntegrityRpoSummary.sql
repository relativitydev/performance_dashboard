-- EDDSPerformance 
IF NOT EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'Reports_RecoverabilityIntegrityRpoSummary' AND TABLE_SCHEMA = N'EDDSDBO') 
BEGIN
	CREATE TABLE eddsdbo.Reports_RecoverabilityIntegrityRpoSummary (
		Id INT IDENTITY(1,1),
		CONSTRAINT PK_Reports_RecoverabilityIntegrityRpoSummary PRIMARY KEY (Id),
		HourId INT,
		WorstRpoDatabase NVARCHAR(255),
		RpoMaxDataLoss INT
	)
END
GO