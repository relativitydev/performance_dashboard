

SELECT *
FROM eddsdbo.[UserExperience] with(nolock)
WHERE [HourId] = @hourId AND [ServerId] = @serverId