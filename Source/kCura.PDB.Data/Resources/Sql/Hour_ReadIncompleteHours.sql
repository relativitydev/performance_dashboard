

select h.Id
from eddsdbo.Hours h with(nolock)
where h.Status in (1, 2)