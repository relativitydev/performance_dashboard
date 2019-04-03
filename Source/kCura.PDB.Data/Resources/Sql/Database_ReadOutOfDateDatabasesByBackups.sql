
select *
	FROM [eddsdbo].[Databases] with(nolock)
	where ServerId = @serverId
	and (datediff(day, LastBackupFullDate, @windowExceededDate) > 0
		OR datediff(day, LastBackupDiffDate, @windowExceededDate) > 0)
	and [DeletedOn] is null and [Ignore] = 0