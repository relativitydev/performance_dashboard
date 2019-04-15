
--@serverCleanupId

SELECT *
FROM eddsdbo.ServerCleanups with(nolock)
WHERE Id = @serverCleanupId