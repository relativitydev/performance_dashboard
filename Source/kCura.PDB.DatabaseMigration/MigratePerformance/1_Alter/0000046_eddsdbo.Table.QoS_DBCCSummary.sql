USE EDDSPerformance
GO

IF NOT EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'QoS_DBCCSummary' AND TABLE_SCHEMA = N'EDDSDBO') 
BEGIN
	CREATE TABLE [eddsdbo].[QoS_DBCCSummary]
	(
		kdbbcsID INT IDENTITY (1,1), PRIMARY KEY (kdbbcsID),
		[DBName] [nvarchar](255) NULL,
		CaseArtifactID int,
		[LastDBCCDate] [datetime] NULL,
		EntryDate DATETIME,
		WindowExceededBy INT,
		GapResolvedDate DATETIME NULL
	)
END
GO