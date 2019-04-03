
--@serverCleanupId

UPDATE eddsdbo.ServerCleanups
SET ServerId = @serverId, HourId = @hourId, Success = @success
WHERE Id = @id