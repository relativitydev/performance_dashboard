SELECT distinct e.SourceTypeID
from eddsdbo.[Events] e with(nolock)
where e.SourceTypeID in @eventTypes and e.StatusID in @eventStatuses