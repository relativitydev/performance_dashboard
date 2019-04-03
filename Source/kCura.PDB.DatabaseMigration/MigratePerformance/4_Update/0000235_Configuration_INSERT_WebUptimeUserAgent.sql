USE EDDSPerformance
GO

IF NOT EXISTS(SELECT TOP 1 Value FROM eddsdbo.Configuration WHERE Section = 'kCura.PDB' AND Name = 'WebUptimeUserAgent')
BEGIN
	INSERT INTO eddsdbo.Configuration
	(Section, Name, Value, MachineName, [Description]) VALUES
	('kCura.PDB', 'WebUptimeUserAgent', 'Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2228.0 Safari/537.36', '', 'The User-Agent value used when requesting the Relativity login page during web uptime scoring.')
END