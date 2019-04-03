-- EDDSPerformance
DECLARE @expectedVersion nvarchar(max) = (SELECT TOP 1 Value FROM [eddsdbo].[Configuration] WHERE Section = 'kCura.PDB' AND Name = 'AdminScriptsVersion')

select case when count(*) = 0 then 1 else 0 end as AllServersDeployed
from eddsdbo.[Server] as s
where
(DeletedOn is null and (IgnoreServer is null or IgnoreServer = 0))
and ServerTypeID = 3
and (s.AdminScriptsVersion <> @expectedVersion or s.AdminScriptsVersion is null)