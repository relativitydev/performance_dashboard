

select b.*, hsab.HourId, hsab.ServerId
from eddsdbo.SearchAuditBatch as b with(nolock)
inner join eddsdbo.HourSearchAuditBatches hsab on b.HourSearchAuditBatchId = hsab.Id
where hsab.ServerId = @serverId and hsab.HourId = @hourId