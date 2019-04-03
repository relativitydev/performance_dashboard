USE [EDDSPerformance]

IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'eddsdbo' AND TABLE_NAME = 'SearchAuditBatchResult') 
	AND NOT EXISTS(SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[SearchAuditBatchResult]') AND name = N'IX_SearchAuditBatchResult_BatchId_UserId')
BEGIN
	
	CREATE NONCLUSTERED INDEX [IX_SearchAuditBatchResult_BatchId_UserId] ON [eddsdbo].[SearchAuditBatchResult]
	(
		[BatchId] ASC,
		[UserId] ASC
	)
	INCLUDE ([Id])
	WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

END
