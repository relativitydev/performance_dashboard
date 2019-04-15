

-- Count 1 or greater = true
select
	case when
	count(b.id) >= max(hb.BatchesCreated)
	then 1 else 0 end
from eddsdbo.HourSearchAuditBatches as hb WITH(NOLOCK)
	left outer join eddsdbo.SearchAuditBatch b with(nolock) on hb.id = b.hoursearchauditbatchid and b.Completed = 1
where
		hb.ServerId = @serverId and hb.HourId =  @hourId -- for the given server and hour
group by hb.HourId, hb.ServerId