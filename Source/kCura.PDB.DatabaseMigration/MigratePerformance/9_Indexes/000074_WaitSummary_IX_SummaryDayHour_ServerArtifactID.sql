USE EDDSPerformance
GO

IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_WaitSummary]') AND name = N'IX_SummaryDayHour_ServerArtifactID')
BEGIN
	DROP INDEX IX_SummaryDayHour_ServerArtifactID ON eddsdbo.QoS_WaitSummary
END

GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_WaitSummary]') AND name = N'IX_SummaryDayHour_ServerArtifactID')
BEGIN
	CREATE NONCLUSTERED INDEX IX_SummaryDayHour_ServerArtifactID ON eddsdbo.QoS_WaitSummary
	(
		[SummaryDayHour] ASC,
		[ServerArtifactID] ASC
	) INCLUDE ([SignalWaitsRatio])
END