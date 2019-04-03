

select *
from eddsdbo.EventWorkers with(nolock)
where Id = @id