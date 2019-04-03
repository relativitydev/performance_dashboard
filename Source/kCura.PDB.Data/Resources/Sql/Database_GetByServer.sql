select
	sd.Name
from sys.databases sd with(nolock)
where name = 'EDDS'
	OR name LIKE 'EDDS[0-9]%'
	OR name LIKE 'INV%'