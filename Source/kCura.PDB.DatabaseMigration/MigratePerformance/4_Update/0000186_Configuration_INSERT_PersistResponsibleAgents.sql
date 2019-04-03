USE EDDSPerformance
GO

IF NOT EXISTS(SELECT TOP 1 Value FROM eddsdbo.Configuration WHERE Section = 'kCura.PDB' AND Name = 'PersistResponsibleAgents')
BEGIN
	INSERT INTO eddsdbo.Configuration
	(Section, Name, Value, MachineName, [Description]) VALUES
	('kCura.PDB', 'PersistResponsibleAgents', 'False', '', 'Indicates whether the ResponsibleAgent column on the eddsdbo.Server table should be preserved. When this is true, QoS Workers will persist server-level claims.')
END