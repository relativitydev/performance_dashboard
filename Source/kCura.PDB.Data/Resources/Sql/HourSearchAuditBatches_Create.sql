USE EDDSPerformance

INSERT INTO [eddsdbo].[HourSearchAuditBatches]
		([HourId]
		,[ServerId]
		,[BatchesCreated])
	VALUES
		(@hourId
		,@serverId
		,@batchesCreated)
		
select ID
from [eddsdbo].[HourSearchAuditBatches]
where [HourId] = @hourId and [ServerId] = @serverId