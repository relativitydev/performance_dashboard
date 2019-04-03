

update [eddsdbo].[SearchAuditBatchResult]
	set TotalComplexQueries = @totalComplexQueries
		,TotalLongRunningQueries = @totalLongRunningQueries
		,TotalSimpleLongRunningQueries = @totalSimpleLongRunningQueries
		,TotalQueries = @totalQueries
		,TotalExecutionTime = @totalExecutionTime
	where [BatchId] = @batchId and [UserId] = @userId
