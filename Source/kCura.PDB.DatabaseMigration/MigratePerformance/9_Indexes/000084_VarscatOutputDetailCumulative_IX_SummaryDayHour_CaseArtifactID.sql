USE [EDDSPerformance]
GO

IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'eddsdbo' AND TABLE_NAME = 'QoS_VarscatOutputDetailCumulative') 
BEGIN
	IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_VarscatOutputDetailCumulative]') AND name = N'IX_SummaryDayHour_CaseArtifactID')
	BEGIN
		CREATE NONCLUSTERED INDEX IX_SummaryDayHour_CaseArtifactID ON [eddsdbo].[QoS_VarscatOutputDetailCumulative] 
		(
			[SummaryDayHour] ASC,
			[CaseArtifactID] ASC
		)
		INCLUDE ( ExecutionTime, IsLongRunning )
	END
END