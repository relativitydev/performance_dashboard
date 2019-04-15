USE EDDSPerformance
GO

IF NOT EXISTS (SELECT TOP 1 Name FROM eddsdbo.Configuration WHERE Section = 'kCura.PDB' AND Name = 'UseDbccViewMonitoring')
BEGIN
	INSERT INTO EDDSPerformance.eddsdbo.Configuration
		(Section, Name, Value, MachineName, [Description])
	VALUES
		('kCura.PDB', 'UseDbccViewMonitoring', 'False', '', 'Indicates whether to include DBCC history views in monitoring. A value of True indicates that this form of monitoring is enabled.')
END

IF NOT EXISTS (SELECT TOP 1 Name FROM eddsdbo.Configuration WHERE Section = 'kCura.PDB' AND Name = 'UseDbccCommandMonitoring')
BEGIN
	INSERT INTO EDDSPerformance.eddsdbo.Configuration
		(Section, Name, Value, MachineName, [Description])
	VALUES
		('kCura.PDB', 'UseDbccCommandMonitoring', 'True', '', 'Indicates whether to run DBCC DBINFO commands to obtain DBCC history. A value of True indicates that this form of monitoring is enabled. This requires sysadmin credentials for deployment.')
END