USE [EDDSPerformance]


UPDATE [eddsdbo].[HourSearchAuditBatches]
set
	[BatchesCreated] = @batchesCreated
WHERE HourId = @hourId and ServerId = @serverId


