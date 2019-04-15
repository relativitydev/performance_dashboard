USE [EDDSPerformance]
GO

IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'QoS_RecoverabilityIntegritySummary' AND TABLE_SCHEMA = 'eddsdbo') 
BEGIN
	IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_RecoverabilityIntegritySummary]') AND name = N'IX_SummaryDayHour')
	BEGIN
		CREATE NONCLUSTERED INDEX [IX_SummaryDayHour] ON [eddsdbo].[QoS_RecoverabilityIntegritySummary]
		(
			[SummaryDayHour] ASC
		)
		INCLUDE ([RecoverabilityIntegrityScore], [RowHash])
	END
END