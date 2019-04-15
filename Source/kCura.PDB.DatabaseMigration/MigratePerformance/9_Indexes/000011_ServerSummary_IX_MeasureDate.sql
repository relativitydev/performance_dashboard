USE EDDSPerformance
GO

IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[ServerSummary]') AND name = N'IX_MeasureDate')
BEGIN
	DROP INDEX IX_MeasureDate ON eddsdbo.ServerSummary
END

GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[ServerSummary]') AND name = N'IX_MeasureDate')
BEGIN
	CREATE NONCLUSTERED INDEX [IX_MeasureDate] ON [eddsdbo].[ServerSummary]
	(
		[MeasureDate] ASC
	)
	INCLUDE ([ServerID], [RAMPagesPerSec], [AvailableMemory], [RAMPct])
END