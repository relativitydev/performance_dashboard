USE EDDSPerformance
GO

IF EXISTS(SELECT * FROM EDDSResource.INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'kIE_BackResults' AND TABLE_SCHEMA = 'dbo')
BEGIN
	IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_BackResults]') AND type in (N'U'))
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
		
	INSERT INTO eddsdbo.QoS_BackResults
	(ServerName, DBName, CaseArtifactID, dateDBCreated, LastBackupDate, BackupStatus, DaysSinceLast, LoggedDate)
	SELECT
	ServerName, DBName, CaseArtifactID, dateDBCreated, lastBackupDate, BackupStatus, DaysSinceLast, LoggedDate
	FROM EDDSResource.dbo.kIE_BackResults WITH(NOLOCK)
	
	DROP TABLE EDDSResource.dbo.kIE_BackResults
END