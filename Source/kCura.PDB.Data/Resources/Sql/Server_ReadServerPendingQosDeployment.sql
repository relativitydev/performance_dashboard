select *
from eddsdbo.[Server] with(nolock)
where IsQosDeployed = 0
and DeletedOn is null and (IgnoreServer is null or IgnoreServer = 0)
and ServerTypeId = @databaseServerTypeId