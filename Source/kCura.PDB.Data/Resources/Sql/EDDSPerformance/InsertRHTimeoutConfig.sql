USE EDDSPerformance

IF NOT EXISTS (SELECT TOP 1 Name FROM eddsdbo.Configuration WHERE Section = 'kCura.PDB' AND Name = 'RHTimeoutSeconds')
BEGIN
	INSERT INTO eddsdbo.Configuration
		(Section, Name, Value, MachineName, [Description])
	VALUES
		('kCura.PDB', 'RHTimeoutSeconds', '3600', '', 'Commands executed by RoundhousE will time out after this interval (in seconds). If you experience timeout errors while importing PDB or during agent deployment tasks, increase this accordingly.')
END