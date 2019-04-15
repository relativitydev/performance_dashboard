-- @serverId int, @start datetime, @end datetime



SELECT *
FROM eddsdbo.[UserExperience] ux with(nolock)
INNER JOIN eddsdbo.[Hours] h with(nolock)
	ON ux.HourId = h.Id
WHERE ux.ServerId = @serverId
	AND h.HourTimeStamp >= @start
	AND h.HourTimeStamp <= @end
	and h.Status != 4