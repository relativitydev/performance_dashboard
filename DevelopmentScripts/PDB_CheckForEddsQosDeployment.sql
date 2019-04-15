USE EDDSPerformance;

select count(*) DatabasesNotDeployed
from EDDSPerformance.eddsdbo.[Server]
where IsQosDeployed = 0
and DeletedOn is null and (IgnoreServer is null or IgnoreServer = 0)
and ServerTypeId = 3