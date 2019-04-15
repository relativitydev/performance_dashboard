USE EDDSPerformance
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[UserCountDW]') AND name = N'IX_MeasureDate_CaseArtifactID')
CREATE NONCLUSTERED INDEX IX_MeasureDate_CaseArtifactID
ON [eddsdbo].[UserCountDW] ([MeasureDate],[CaseArtifactID])
INCLUDE ([UserCount])