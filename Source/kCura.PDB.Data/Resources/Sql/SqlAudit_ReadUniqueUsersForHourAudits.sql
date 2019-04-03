-- Run against workspace database
-- Arguments required (@beginDate, @endDate, @action)
SELECT DISTINCT(UserId)
FROM [eddsdbo].[AuditRecord] a WITH ( NOLOCK )
WHERE a.[Action] in @action
	AND [TimeStamp] >= @beginDate
	AND [Timestamp] < @endDate