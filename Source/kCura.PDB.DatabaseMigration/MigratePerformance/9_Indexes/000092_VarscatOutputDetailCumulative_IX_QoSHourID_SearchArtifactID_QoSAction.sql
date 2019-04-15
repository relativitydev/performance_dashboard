USE [EDDSPerformance]
GO

IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'eddsdbo' AND TABLE_NAME = 'QoS_VarscatOutputDetailCumulative') 
	AND NOT EXISTS(SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_VarscatOutputDetailCumulative]') AND name = N'IX_QoSHourID_SearchArtifactID_QoSAction')
BEGIN
	CREATE NONCLUSTERED INDEX [IX_QoSHourID_SearchArtifactID_QoSAction] ON [eddsdbo].[QoS_VarscatOutputDetailCumulative]
	(
		[QoSHourID] ASC,
		[SearchArtifactID] ASC,
		[QoSAction] ASC
	)
	INCLUDE ( [CaseArtifactID], [IsComplex] )
END