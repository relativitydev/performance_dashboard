SELECT count(*)
FROM [eddsdbo].[Databases] with(nolock)
where [ServerId] = @serverId and [DeletedOn] is null and [Ignore] = 0