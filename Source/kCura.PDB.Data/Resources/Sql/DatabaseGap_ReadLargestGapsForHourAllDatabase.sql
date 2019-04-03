;WITH CTE AS
(
SELECT 
dg.Id, 
DatabaseId,
GapStart as [Start],
GapEnd as [End],
Duration,
ActivityType,
ROW_NUMBER() OVER(PARTITION BY [DatabaseId] ORDER BY [Duration] DESC) as [RowNum]
  FROM [eddsdbo].[DatabaseGaps] dg with(nolock)
  inner join [eddsdbo].[Databases] d with(nolock) on d.id = dg.DatabaseId and d.serverid = @serverId

  where [GapEnd] >= @hourTimeStampStart and [GapEnd] <= @hourTimeStampEnd
	and ActivityType = @activityType
)
SELECT CTE.Id, 
DatabaseId,
[Start],
[End],
Duration,
ActivityType
FROM CTE 
WHERE [RowNum] = 1