SELECT TOP (1) dg.Id, DatabaseId, GapStart as [Start], GapEnd as [End], Duration, ActivityType
  FROM [eddsdbo].[DatabaseGaps] dg with(nolock)
  inner join [eddsdbo].[Databases] d with(nolock) on d.id = dg.databaseid and d.serverid = @serverId
  where [GapEnd] >= @hourTimeStampStart and [GapEnd] <= @hourTimeStampEnd
	and ActivityType = @activityType
  order by [Duration] desc