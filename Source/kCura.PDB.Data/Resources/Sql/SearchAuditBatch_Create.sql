

INSERT INTO [eddsdbo].[SearchAuditBatch](
		WorkspaceId
		,BatchStart
		,BatchSize
		,Completed
		,HourSearchAuditBatchId)
	VALUES(
		@workspaceId
		,@batchStart
		,@batchSize
		,0
		,@hourSearchAuditBatchId)