-- EDDSPerformance
IF NOT EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'Reports_RecoveryObjectives' AND TABLE_SCHEMA = N'EDDSDBO') 
BEGIN
	CREATE TABLE eddsdbo.Reports_RecoveryObjectives (
		Id INT IDENTITY (1,1),
		CONSTRAINT PK_Reports_RecoveryObjectives PRIMARY KEY (Id),
		DatabaseId INT, --DBName NVARCHAR (255) NULL, ServerName NVARCHAR (255) NULL,
		RpoScore DECIMAL (5, 2) NULL,
		RpoMaxDataLoss INT NULL, --PotentialDataLossMinutes INT NULL,
		RtoScore DECIMAL (5, 2) NULL,		
		RtoTimeToRecover DECIMAL (6,2) NULL --EstimatedTimeToRecoverHours DECIMAL (6,2) NULL,
	)
END
GO