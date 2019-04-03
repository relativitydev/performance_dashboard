-- Awaiting scoring: Data has been collected, waiting for metric system to run and calculate the category score for UX
USE [EddsPerformance]

select isnull(sum(c), 0)
from (
	select 
		1 c
	from eddsdbo.[Hours] as h WITH(NOLOCK)
		left outer join eddsdbo.HourSearchAuditBatches as hb WITH(NOLOCK) on h.id = hb.HourId
		left outer join eddsdbo.SearchAuditBatch sab on hb.id = sab.hoursearchauditbatchid and sab.Completed = 1
		where h.HourTimeStamp >= DATEADD(DAY, @backFillHours, getutcdate()) and h.Status != 4
		group by h.id
		having count(sab.Id) > 0
) a