USE [EDDSPerformance]
GO

IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_SourceDatetime]') AND name = N'CiQoS_quotidian')
DROP INDEX [CiQoS_quotidian] ON [eddsdbo].[QoS_SourceDatetime] WITH ( ONLINE = OFF )
GO

IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_SourceDatetime]') AND name = N'NCQoS_Quotidian')
DROP INDEX [NCQoS_Quotidian] ON [eddsdbo].[QoS_SourceDatetime] WITH ( ONLINE = OFF )
GO