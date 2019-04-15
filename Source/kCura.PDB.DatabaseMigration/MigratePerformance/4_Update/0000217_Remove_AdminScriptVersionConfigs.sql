USE [EDDSPerformance]
GO

delete from eddsdbo.Configuration
where section = 'kCura.PDB' and (Name = 'AdminScriptsLatestVersion')

