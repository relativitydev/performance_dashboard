USE EDDSPerformance
GO

IF NOT EXISTS(SELECT TOP 1 Value FROM eddsdbo.Configuration WHERE Section = 'kCura.PDB' AND Name = 'EnableInstantFileInitializationCheck')
BEGIN
	INSERT INTO eddsdbo.Configuration
	(Section, Name, Value, MachineName, [Description]) VALUES
	('kCura.PDB', 'EnableInstantFileInitializationCheck', 'True', '', 'Indicates whether Tuning Fork scripts will check for instant file initialization is enabled or not.')
END