USE EDDSPerformance

DELETE result
FROM eddsdbo.SearchAuditBatchResult result
INNER JOIN eddsdbo.SearchAuditBatch batch ON result.BatchId = batch.Id
WHERE batch.HourSearchAuditBatchId = @hourSearchAuditBatchId