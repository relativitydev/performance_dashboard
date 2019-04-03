USE EDDSPerformance
GO

IF EXISTS(SELECT * FROM EDDSResource.INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'kIE_BackSummary' AND TABLE_SCHEMA = 'dbo')
BEGIN
	IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_BackSummary]') AND type in (N'U'))
		CREATE TABLE [eddsdbo].[QoS_BackSummary]
		(
			kdbsID  INT    IDENTITY ( 1 , 1 ),Primary Key (kdbsID),
			[DBName] [nvarchar](255) NULL,
			CaseArtifactID int,
			[LastBackupDate] [datetime] NULL,
			EntryDate DATETIME,
			WindowExceededBy INT,
			GapResolvedDate DATETIME NULL
		)
		
	INSERT INTO eddsdbo.QoS_BackSummary
	(DBName, CaseArtifactID, LastBackupDate, EntryDate, WindowExceededBy, GapResolvedDate)
	SELECT
	DBName, CaseArtifactID, LastBackupDate, EntryDate, WindowExceededBy, GapResolvedDate
	FROM EDDSResource.dbo.kIE_BackSummary WITH(NOLOCK)
	
	DROP TABLE EDDSResource.dbo.kIE_BackSummary
END