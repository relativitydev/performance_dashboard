
select *
	FROM [eddsdbo].[Databases] with(nolock)
	where ServerId = @serverId
		and datediff(day, LastDbccDate, @windowExceededDate) > 0
		and [DeletedOn] is null and [Ignore] = 0