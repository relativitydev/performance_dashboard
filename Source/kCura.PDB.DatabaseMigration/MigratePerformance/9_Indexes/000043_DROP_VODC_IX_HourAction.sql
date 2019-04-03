USE EDDSPerformance
GO

IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_VarscatOutputDetailCumulative]') AND name = N'IX_HourAction')
BEGIN
	DROP INDEX IX_HourAction ON eddsdbo.QoS_VarscatOutputDetailCumulative
END