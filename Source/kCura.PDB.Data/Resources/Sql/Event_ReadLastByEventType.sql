

select top(1) *
from eddsdbo.[Events] with(nolock)
where SourceTypeID = @eventType
and SourceID is null
order by Id desc