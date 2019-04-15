-- Awaiting Discovery: hours have not been created for the timerange yet

DECLARE @nowDate DATETIME = getutcdate()
DECLARE @startDate DATETIME = DATEADD(dd, @backFillHours, @nowDate)
DECLARE @hoursInRange int = DATEDIFF(hour, @startDate, @nowDate)

-- If they exist in the Hours table, we're looking at them.
SELECT @hoursInRange - COUNT(*) FROM eddsdbo.[Hours] h WITH(NOLOCK) WHERE h.HourTimeStamp >= @startDate and h.Status != 4