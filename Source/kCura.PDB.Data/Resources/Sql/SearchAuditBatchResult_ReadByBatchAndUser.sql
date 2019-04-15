

SELECT Id FROM [eddsdbo].[SearchAuditBatchResult] with(nolock)
	where [BatchId] = @batchId and [UserId] = @userId