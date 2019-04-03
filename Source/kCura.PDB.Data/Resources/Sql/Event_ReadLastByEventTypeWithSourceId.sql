

select top(1) *
from eddsdbo.[Events] with(nolock)
where SourceTypeID = @eventType
and SourceID = @sourceId
order by Id desc