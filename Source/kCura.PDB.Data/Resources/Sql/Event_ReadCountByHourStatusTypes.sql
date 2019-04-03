

select count(id)
from eddsdbo.Events with(nolock)
where Hourid = @hourId
	and StatusID in @statusIds
	and SourceTypeID != @eventTypeToExclude

