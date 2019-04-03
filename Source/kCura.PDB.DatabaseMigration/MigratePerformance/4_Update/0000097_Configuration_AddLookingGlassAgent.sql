USE EDDSPerformance
GO

IF NOT EXISTS(SELECT TOP 1 Value FROM eddsdbo.Configuration WHERE Section = 'kCura.PDB' AND Name = 'LookingGlassAgent')
BEGIN
	INSERT INTO eddsdbo.Configuration
	(Section, Name, Value, MachineName, [Description]) VALUES
	('kCura.PDB', 'LookingGlassAgent', '', '', '')
END