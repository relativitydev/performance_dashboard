USE EDDSPerformance
GO

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[PerformanceSummary]') AND name = N'IX_CaseArtifactID')
DROP INDEX [IX_CaseArtifactID] ON [eddsdbo].[PerformanceSummary] WITH ( ONLINE = OFF )

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[PerformanceSummary]') AND name = N'kIE_MeasureDate')
DROP INDEX [kIE_MeasureDate] ON [eddsdbo].[PerformanceSummary] WITH ( ONLINE = OFF )
GO

ALTER TABLE eddsdbo.PerformanceSummary
DROP COLUMN AverageLatency;

GO

ALTER TABLE eddsdbo.PerformanceSummary
DROP COLUMN NRLRQCount;

GO

ALTER TABLE eddsdbo.PerformanceSummary
DROP COLUMN TotalQCount;

GO

ALTER TABLE eddsdbo.PerformanceSummary
DROP COLUMN TotalNRQCount;