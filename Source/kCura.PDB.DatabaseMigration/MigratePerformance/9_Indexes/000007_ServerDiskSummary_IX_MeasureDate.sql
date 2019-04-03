USE EDDSPerformance
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[ServerDiskSummary]') AND name = N'IX_MeasureDate')
BEGIN
CREATE NONCLUSTERED INDEX [IX_MeasureDate] ON [eddsdbo].[ServerDiskSummary]
(
	[MeasureDate] ASC
)
END