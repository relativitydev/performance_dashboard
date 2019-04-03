

SELECT *
FROM [eddsdbo].[AgentHistory] as ah with(nolock)
inner join eddsdbo.[Hours] as h with(nolock) on h.ID = @hourId
WHERE ah.[TimeStamp] > h.HourTimeStamp and ah.TimeStamp < dateadd(HOUR, 1,  h.HourTimeStamp) and h.Status != 4