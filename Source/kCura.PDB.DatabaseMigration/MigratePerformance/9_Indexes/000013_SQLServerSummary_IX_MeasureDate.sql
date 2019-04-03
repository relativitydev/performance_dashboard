USE EDDSPerformance
GO

IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[SQLServerSummary]') AND name = N'IX_MeasureDate')
BEGIN
	DROP INDEX IX_MeasureDate ON eddsdbo.SQLServerSummary
END

GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[SQLServerSummary]') AND name = N'IX_MeasureDate')
BEGIN
	CREATE NONCLUSTERED INDEX [IX_MeasureDate] ON [eddsdbo].[SQLServerSummary]
	(
		[MeasureDate] ASC
	) INCLUDE ([ServerID], [LowMemorySignalStateRatio])
END