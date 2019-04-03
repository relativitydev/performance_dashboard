USE EDDSPerformance
GO

IF NOT EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'QoS_BackResults' AND TABLE_SCHEMA = N'EDDSDBO') 
BEGIN
	CREATE TABLE [eddsdbo].[QoS_BackResults]
	(
		kdbbuID  INT    IDENTITY ( 1 , 1 ),Primary Key (kdbbuID),
		[ServerName] [nvarchar](255) NULL,
		[DBName] [nvarchar](255) NULL,
		CaseArtifactID int,
		dateDBCreated datetime,
		[LastBackupDate] [datetime] NULL,
		[BackupStatus] [int] NULL,
		DaysSinceLast INT,
		[LoggedDate] datetime
	)
END
GO