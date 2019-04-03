DECLARE @latestVersion nvarchar(max) = '2.7.16.24';

IF NOT EXISTS (SELECT TOP 1 Name FROM EDDSPerformance.eddsdbo.Configuration WHERE Section = 'kCura.PDB' AND Name = 'AdminScriptsVersion')
BEGIN
	INSERT INTO EDDSPerformance.eddsdbo.Configuration
		(Section, Name, Value, MachineName, [Description])
	VALUES
		('kCura.PDB', 'AdminScriptsVersion', @latestVersion, '', 'Indicates the current version of scripts that must be installed via PDB with admin privileges. If PDB has been completely installed, this and AdminScriptsVersion should be identical.')
END
ELSE
BEGIN
	UPDATE EDDSPerformance.eddsdbo.Configuration
	SET Value = @latestVersion, [Description] = 'Indicates the current version of scripts that must be installed via PDB with admin privileges. If PDB has been completely installed, this and AdminScriptsVersion should be identical.'
	WHERE Section = 'kCura.PDB' AND Name = 'AdminScriptsVersion'
END