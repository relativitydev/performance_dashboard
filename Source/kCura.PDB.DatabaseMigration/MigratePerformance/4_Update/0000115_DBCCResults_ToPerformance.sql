USE EDDSPerformance
GO

IF EXISTS(SELECT * FROM EDDSResource.INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'kIE_DBCCResults' AND TABLE_SCHEMA = 'dbo')
BEGIN
	IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_DBCCResults]') AND type in (N'U'))
		CREATE TABLE [eddsdbo].[QoS_DBCCResults](
			kdbccbID  INT    IDENTITY ( 1 , 1 ),Primary Key (kdbccbID),
			[ServerName] [nvarchar](255) NULL,
			[DBName] [nvarchar](255) NULL,
			CaseArtifactID int,
			dateDBCreated datetime,
			[LastCleanDBCCDate] [datetime] NULL,
			[DBCCStatus] [int] NULL,
			DaysSinceLast INT,
			[LoggedDate] datetime
		)
		
	INSERT INTO eddsdbo.QoS_DBCCResults
	(ServerName, DBName, CaseArtifactID, dateDBCreated, LastCleanDBCCDate, DBCCStatus, DaysSinceLast, LoggedDate)
	SELECT
	ServerName, DBName, CaseArtifactID, dateDBCreated, LastCleanDBCCDate, DBCCStatus, DaysSinceLast, LoggedDate
	FROM EDDSResource.dbo.kIE_DBCCResults WITH(NOLOCK)
	
	DROP TABLE EDDSResource.dbo.kIE_DBCCResults
END