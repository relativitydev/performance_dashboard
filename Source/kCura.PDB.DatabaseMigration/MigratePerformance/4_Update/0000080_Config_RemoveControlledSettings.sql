USE EDDSPerformance
GO

IF EXISTS(SELECT * FROM eddsdbo.Configuration WHERE Section = 'kCura.PDB' AND Name = 'OffloadBackups' AND Value = 'False')
	DELETE FROM eddsdbo.Configuration WHERE Section = 'kCura.PDB' AND Name = 'OffloadBackups'

IF EXISTS(SELECT * FROM eddsdbo.Configuration WHERE Section = 'kCura.PDB' AND Name = 'OffloadDBCC' AND Value = 'False')
	DELETE FROM eddsdbo.Configuration WHERE Section = 'kCura.PDB' AND Name = 'OffloadDBCC'
	
IF EXISTS(SELECT * FROM eddsdbo.Configuration WHERE Section = 'kCura.PDB' AND Name = 'UpgradeWillPurgeQoSTables' AND Value = 'False')
	DELETE FROM eddsdbo.Configuration WHERE Section = 'kCura.PDB' AND Name = 'UpgradeWillPurgeQoSTables'