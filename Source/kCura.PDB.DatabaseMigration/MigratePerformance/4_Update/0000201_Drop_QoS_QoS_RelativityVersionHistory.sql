USE [EDDSPerformance]
GO

if exists(select 1 from sysobjects where [name] = 'QoS_RelativityVersionHistory' and type = 'U')
begin
	DROP TABLE [eddsdbo].[QoS_RelativityVersionHistory]
end


