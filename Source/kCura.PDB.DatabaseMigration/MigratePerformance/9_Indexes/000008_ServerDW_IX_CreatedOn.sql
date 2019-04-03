USE EDDSPerformance

GO

IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[ServerDW]') AND name = N'IX_CreatedOn')
BEGIN
	DROP INDEX IX_CreatedOn ON eddsdbo.ServerDW
END

GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[ServerDW]') AND name = N'IX_CreatedOn')
BEGIN
	CREATE NONCLUSTERED INDEX [IX_CreatedOn] ON [eddsdbo].[ServerDW]
	(
		[CreatedOn] ASC
	)
	INCLUDE ( [ServerID], [AvailableMemory] )
END