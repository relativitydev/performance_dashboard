IF (EXISTS (SELECT name 
FROM master.dbo.sysdatabases 
WHERE ('[' + name + ']' = 'EDDSPerformance' OR name = 'EDDSPerformance')))
BEGIN
	PRINT N'Updating EDDSPerformance.eddsdbo.[Server] AdminScriptsVersion';
	UPDATE EDDSPerformance.eddsdbo.[Server]
	SET AdminScriptsVersion = (SELECT TOP 1 Name FROM EDDSPerformance.eddsdbo.Configuration WHERE Section = 'kCura.PDB' AND Name = 'AdminScriptsVersion')
	OUTPUT inserted.*
	WHERE ServerTypeID = 3 AND (DeletedOn IS NULL OR IgnoreServer <> 1)
END