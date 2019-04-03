select distinct obj.name
from sys.objects as obj
inner join sys.columns c on c.object_id = obj.object_id
where 
[type] = 'U'
and obj.name like 'zca_pop_[0-9]%' and c.name = 'ID'