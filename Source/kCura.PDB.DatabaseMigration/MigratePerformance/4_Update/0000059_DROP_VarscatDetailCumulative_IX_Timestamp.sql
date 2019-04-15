USE EDDSPerformance
GO

IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_VarscatOutputDetailCumulative]') AND name = N'IX_Timestamp')
DROP INDEX [IX_Timestamp] ON [eddsdbo].[QoS_VarscatOutputDetailCumulative] WITH ( ONLINE = OFF )