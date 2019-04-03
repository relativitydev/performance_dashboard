

SELECT b.*, hsab.HourId, hsab.ServerId
FROM eddsdbo.[SearchAuditBatch] b with(nolock)
inner join eddsdbo.HourSearchAuditBatches hsab on b.HourSearchAuditBatchId = hsab.Id
WHERE b.[Id] = @batchId