

select count(*)
from eddsdbo.[Events] e with(nolock)
where e.SourceTypeID = @sourceTypeId
	AND e.SourceID IS NULL
	AND e.StatusId != 100 -- No duplicates