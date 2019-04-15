

select br.*
from eddsdbo.SearchAuditBatch as b with(nolock)
inner join eddsdbo.SearchAuditBatchResult as br with(nolock) on b.Id = br.BatchId
inner join eddsdbo.HourSearchAuditBatches hsab on b.HourSearchAuditBatchId = hsab.Id
where hsab.ServerId = @serverId and hsab.HourId = @hourId