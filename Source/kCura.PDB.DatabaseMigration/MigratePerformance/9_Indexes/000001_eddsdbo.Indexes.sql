USE [EDDSPerformance]
GO

IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[PerformanceSummary]') AND name = N'IX_CaseArtifactID')
DROP INDEX [IX_CaseArtifactID] ON [eddsdbo].[PerformanceSummary] WITH ( ONLINE = OFF )

CREATE NONCLUSTERED INDEX [IX_CaseArtifactID] ON [eddsdbo].[PerformanceSummary]
(
  [CaseArtifactID] ASC
)
INCLUDE ( [MeasureDate],
 [UserCount],
 [ErrorCount],
 [LRQCount]) WITH (SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY];
	
IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[PerformanceSummary]') AND name = N'kIE_MeasureDate')
DROP INDEX [kIE_MeasureDate] ON [eddsdbo].[PerformanceSummary] WITH ( ONLINE = OFF )

CREATE NONCLUSTERED INDEX [kIE_MeasureDate] ON [eddsdbo].[PerformanceSummary] 
(
	[MeasureDate] ASC
)
INCLUDE ([CaseArtifactID],
[UserCount],
[ErrorCount],
[LRQCount]) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
