USE EDDSPerformance

IF NOT EXISTS(SELECT TOP 1 Value FROM eddsdbo.Configuration WHERE Section = 'kCura.PDB' AND Name = 'LogLevel')
BEGIN
	INSERT INTO eddsdbo.Configuration
	(Section, Name, Value, MachineName, [Description]) VALUES
	('kCura.PDB', 'LogLevel', 'Warning', '', 'The level of information logged. Only Error, Warning, and Verbose Currently supported.')
END
