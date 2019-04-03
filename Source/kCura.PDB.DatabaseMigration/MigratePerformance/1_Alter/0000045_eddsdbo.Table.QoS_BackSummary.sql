USE EDDSPerformance
GO

IF NOT EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'QoS_BackSummary' AND TABLE_SCHEMA = N'EDDSDBO') 
BEGIN
	CREATE TABLE [eddsdbo].[QoS_BackSummary]
	(
		kdbsID INT IDENTITY (1,1), PRIMARY KEY (kdbsID),
		[DBName] [nvarchar](255) NULL,
		CaseArtifactID int,
		[LastBackupDate] [datetime] NULL,
		EntryDate DATETIME,
		WindowExceededBy INT,
		GapResolvedDate DATETIME NULL
	)
END
GO