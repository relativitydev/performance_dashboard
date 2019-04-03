USE EDDSPerformance
GO

IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_VarscatOutputDetailCumulative]') AND name = N'NIDX_QOSHRSID')
BEGIN
	DROP INDEX NIDX_QOSHRSID ON eddsdbo.QoS_VarscatOutputDetailCumulative
END