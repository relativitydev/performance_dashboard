USE EDDSPerformance
GO

IF EXISTS(SELECT * FROM EDDSResource.INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'kIE_DBCCSummary' AND TABLE_SCHEMA = 'dbo')
BEGIN
	IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_DBCCSummary]') AND type in (N'U'))
		CREATE TABLE [eddsdbo].[QoS_DBCCSummary]
		(
			kdbbcsID  INT    IDENTITY ( 1 , 1 ),Primary Key (kdbbcsID),
			[DBName] [nvarchar](255) NULL,
			CaseArtifactID int,
			[LastDBCCDate] [datetime] NULL,
			EntryDate DATETIME,
			WindowExceededBy INT,
			GapResolvedDate DATETIME NULL
		)
		
	INSERT INTO eddsdbo.QoS_DBCCSummary
	(DBName, CaseArtifactID, LastDBCCDate, EntryDate, WindowExceededBy, GapResolvedDate)
	SELECT
	DBName, CaseArtifactID, LastDBCCDate, EntryDate, WindowExceededBy, GapResolvedDate
	FROM EDDSResource.dbo.kIE_DBCCSummary WITH(NOLOCK)
	
	DROP TABLE EDDSResource.dbo.kIE_DBCCSummary
END