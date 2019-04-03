UPDATE [EDDSPerformance].[eddsdbo].[DBCCTarget]
SET DatabaseName = @dbName,
	Active = @active
WHERE DbccTargetId = @targetId