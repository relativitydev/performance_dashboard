Select top(1) *
from [eddsdbo].[DatabaseGaps] with(nolock)
where [DatabaseId] = @databaseId
	and [GapStart] = @start
	and [GapEnd] = @end
	and [ActivityType] = @activityType