-- Awaiting Analysis: Batch jobs have been created and are currently gathering audit data

select isnull(sum(c), 0)
from (
	select 
		1 c
	from eddsdbo.[Hours] as h WITH(NOLOCK)
		left outer join eddsdbo.HourSearchAuditBatches as hb WITH(NOLOCK) on h.id = hb.HourId
		left outer join eddsdbo.SearchAuditBatch sab on hb.id = sab.hoursearchauditbatchid and sab.Completed = 0
		where h.HourTimeStamp >= DATEADD(DAY, @backFillHours, getutcdate()) and h.Status != 4 and h.Score is null
		and (hb.BatchesCreated > 0 or hb.ID is null)
		group by h.id
) a
--notes:
--	where "hb.ID is null" is for the current hour. Since we don't create the batches till we start analysing the hour when the hour has past