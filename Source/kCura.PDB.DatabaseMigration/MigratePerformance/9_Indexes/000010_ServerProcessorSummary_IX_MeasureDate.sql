USE EDDSPerformance
GO

IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[ServerProcessorSummary]') AND name = N'IX_MeasureDate')
BEGIN
	DROP INDEX IX_MeasureDate ON eddsdbo.ServerProcessorSummary
END

GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[ServerProcessorSummary]') AND name = N'IX_MeasureDate')
BEGIN
	CREATE NONCLUSTERED INDEX [IX_MeasureDate] ON [eddsdbo].[ServerProcessorSummary]
	(
		[MeasureDate] ASC
	) INCLUDE ([ServerID], [CPUProcessorTimePct])
END