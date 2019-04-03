
--@serverCleanupId

INSERT INTO eddsdbo.ServerCleanups (ServerId, HourId, Success)
VALUES(@serverId, @hourId, @success)

SELECT * FROM eddsdbo.ServerCleanups
WHERE Id = @@IDENTITY