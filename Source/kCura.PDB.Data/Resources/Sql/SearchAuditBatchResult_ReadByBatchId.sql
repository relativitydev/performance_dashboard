

SELECT *
FROM eddsdbo.[SearchAuditBatchResult] with(nolock)
WHERE [BatchId] = @batchId