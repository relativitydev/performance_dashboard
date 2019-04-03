USE EDDSPerformance

UPDATE [eddsdbo].[SearchAuditBatch]
set		BatchStart = @batchStart
		,BatchSize = @batchSize
		,Completed = @completed
Where ID = @id