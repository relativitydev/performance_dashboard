USE EDDSPerformance;

DECLARE @scriptsVersion nvarchar(max) = (SELECT TOP 1 Value FROM [EDDSPerformance].[eddsdbo].[Configuration] WHERE Section = 'kCura.PDB' AND Name = 'AdminScriptsVersion')

update eddsdbo.[Server]
set AdminScriptsVersion = @scriptsVersion
where 
DeletedOn is null 
and (IgnoreServer is null or IgnoreServer = 0)
and ServerTypeID = 3