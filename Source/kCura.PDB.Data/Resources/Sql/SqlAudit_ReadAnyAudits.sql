-- Run against workspace database
-- Arguments required (@beginDate, @endDate, @action)
IF EXISTS(SELECT TOP 1 * -- TODO Paging
	FROM [eddsdbo].[AuditRecord] a WITH ( NOLOCK )
	WHERE a.[Action] in @action
		AND [TimeStamp] >= @beginDate
		AND [Timestamp] < @endDate)
	SELECT 1 
ELSE SELECT 0