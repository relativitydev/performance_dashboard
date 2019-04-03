USE EDDSPerformance
GO

IF NOT EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'QoS_AllDatabasesChecked' AND TABLE_SCHEMA = N'EDDSDBO') 
BEGIN
	CREATE TABLE [eddsdbo].[QoS_AllDatabasesChecked]
	(
		DatabaseID INT IDENTITY (1, 1), PRIMARY KEY (DatabaseID),
		ServerName nvarchar (150),
		DBName nVARCHAR(255),
		CaseArtifactID INT,
		DateCreated datetime,
		IsCompleted BIT
	)
END
GO