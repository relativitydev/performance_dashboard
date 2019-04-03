update eddsdbo.[Server]
set IsQosDeployed = 0
where DeletedOn is null and (IgnoreServer is null or IgnoreServer = 0)
and ServerTypeID = 3