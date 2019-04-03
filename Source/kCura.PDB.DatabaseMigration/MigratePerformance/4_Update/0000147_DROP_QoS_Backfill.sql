USE EDDSPerformance
GO

IF EXISTS (select 1 from sysobjects where [name] = 'QoS_Backfill' and type = 'P')  
BEGIN
	DROP PROCEDURE EDDSDBO.QoS_Backfill
END