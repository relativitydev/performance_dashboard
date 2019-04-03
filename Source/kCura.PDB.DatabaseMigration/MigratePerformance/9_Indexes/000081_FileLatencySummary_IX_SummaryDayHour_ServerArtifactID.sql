USE [EDDSPerformance]
GO

DECLARE @enterprise bit;
DECLARE @SQL nvarchar(max);

IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'eddsdbo' AND TABLE_NAME = 'QoS_FileLatencySummary') 
BEGIN
	IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_FileLatencySummary]') AND name = N'IX_SummaryDayHour_ServerArtifactID')
	BEGIN			
		SET @SQL = 'CREATE UNIQUE NONCLUSTERED INDEX [IX_SummaryDayHour_ServerArtifactID] ON [eddsdbo].[QoS_FileLatencySummary]
		(
			[SummaryDayHour] ASC,
			[ServerArtifactID] ASC
		) INCLUDE (
			HighestLatencyDatabase,
			ReadLatencyMs,
			WriteLatencyMs,
			LatencyScore,
			IOWaitsHigh,
			IsDataFile
		) WITH (IGNORE_DUP_KEY = ON)'
			
		EXEC sp_executesql @SQL
	END
END