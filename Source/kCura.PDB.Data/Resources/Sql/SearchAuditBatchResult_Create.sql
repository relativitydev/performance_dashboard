

INSERT INTO [eddsdbo].[SearchAuditBatchResult](
		BatchId
		,UserId
		,TotalComplexQueries
		,TotalLongRunningQueries
		,TotalSimpleLongRunningQueries
		,TotalQueries
		,TotalExecutionTime)
	VALUES(
		@batchId
		,@userId
		,@totalComplexQueries
		,@totalLongRunningQueries
		,@totalSimpleLongRunningQueries
		,@totalQueries
		,@totalExecutionTime)
