--EDDSPerformance
IF NOT EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'Reports_RecoveryGaps' AND TABLE_SCHEMA = N'EDDSDBO') 
BEGIN
	CREATE TABLE eddsdbo.Reports_RecoveryGaps
	(
		Id INT IDENTITY (1,1),
		CONSTRAINT PK_Reports_RecoveryGaps PRIMARY KEY (Id),
		DatabaseId INT, --ServerArtifactID INT NULL, ServerName NVARCHAR(50) NULL, DatabaseName NVARCHAR(50) NOT NULL, WorkspaceName VARCHAR(50) NULL, CaseArtifactID INT NOT NULL,
		ActivityType INT, --IsBackup BIT NOT NULL,
		LastActivity DATETIME,
		GapResolutionDate DATETIME NULL,
		GapSize INT
	)
END
GO