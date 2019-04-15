USE EDDSPerformance
GO

IF NOT EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'QoS_DBCCResults' AND TABLE_SCHEMA = N'EDDSDBO') 
BEGIN
	CREATE TABLE [eddsdbo].[QoS_DBCCResults]
	(
		kdbccbID INT IDENTITY ( 1 , 1 ),Primary Key (kdbccbID),
		[ServerName] [nvarchar](255) NULL,
		[DBName] [nvarchar](255) NULL,
		CaseArtifactID int,
		dateDBCreated datetime,
		[LastCleanDBCCDate] [datetime] NULL,
		[DBCCStatus] [int] NULL,
		DaysSinceLast INT,
		[LoggedDate] datetime
	)
END
GO