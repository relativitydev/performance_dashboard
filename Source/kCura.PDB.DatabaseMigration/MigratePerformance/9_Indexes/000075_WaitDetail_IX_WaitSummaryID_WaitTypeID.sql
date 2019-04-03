USE [EDDSPerformance]
GO

IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_WaitDetail]') AND name = N'IX_WaitSummaryID_WaitTypeID')
BEGIN
	DROP INDEX IX_WaitSummaryID_WaitTypeID ON eddsdbo.QoS_WaitDetail
END

GO

IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'eddsdbo' AND TABLE_NAME = 'QoS_WaitDetail') 
BEGIN
	IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_WaitDetail]') AND name = N'IX_WaitSummaryID_WaitTypeID')
	BEGIN
		CREATE NONCLUSTERED INDEX [IX_WaitSummaryID_WaitTypeID] ON [eddsdbo].[QoS_WaitDetail]
		(
			[WaitSummaryID] ASC,
			[WaitTypeID] ASC
		)
		INCLUDE ( [DifferentialWaitMs] )
	END
END