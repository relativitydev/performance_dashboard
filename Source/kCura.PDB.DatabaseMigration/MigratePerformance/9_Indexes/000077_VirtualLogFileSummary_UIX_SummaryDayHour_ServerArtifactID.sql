USE [EDDSPerformance]
GO

IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'eddsdbo' AND TABLE_NAME = 'VirtualLogFileSummary') 
BEGIN
	IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[VirtualLogFileSummary]') AND name = N'UIX_SummaryDayHour_ServerArtifactID')
	BEGIN
		DROP INDEX UIX_SummaryDayHour_ServerArtifactID ON eddsdbo.VirtualLogFileSummary
	END

	IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[VirtualLogFileSummary]') AND name = N'UIX_SummaryDayHour_ServerArtifactID')
	BEGIN
		CREATE UNIQUE NONCLUSTERED INDEX [UIX_SummaryDayHour_ServerArtifactID] ON [eddsdbo].[VirtualLogFileSummary]
		(
			[SummaryDayHour] ASC,
			[ServerArtifactID] ASC
		)
		INCLUDE ([QoSHourID], [VirtualLogFiles])
		WITH (IGNORE_DUP_KEY = ON)
	END
END