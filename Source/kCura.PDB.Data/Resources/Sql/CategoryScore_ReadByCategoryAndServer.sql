

select top(1) *
from [eddsdbo].[CategoryScores] with(nolock)
where
	[CategoryID] = @categoryID
	and ([ServerID] = @serverID OR ([ServerID] is null and @serverID is null))