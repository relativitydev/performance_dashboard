USE [EDDSPerformance]
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[ServerDiskDW]') AND name = N'IX_CreatedOn')
BEGIN
CREATE NONCLUSTERED INDEX [IX_CreatedOn] ON [eddsdbo].[ServerDiskDW] 
(
	[CreatedOn] ASC
)
END