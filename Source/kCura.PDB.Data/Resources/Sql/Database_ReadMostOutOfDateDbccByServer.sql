
SELECT TOP (1) LastDbccDate
	FROM [eddsdbo].[Databases] with(nolock)
	where serverId = @serverId and LastDbccDate is not null
		and [DeletedOn] is null and [Ignore] = 0
	order by LastDbccDate asc