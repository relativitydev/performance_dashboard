SELECT *
FROM [eddsdbo].[Databases] with(nolock)
WHERE [Name] = @name
	and [ServerId] = @serverId