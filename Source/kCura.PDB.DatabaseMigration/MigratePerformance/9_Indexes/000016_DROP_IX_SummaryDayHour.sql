USE EDDSPerformance
GO

IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_FCM]') AND name = N'IX_SummaryDayHour')
BEGIN
	DROP INDEX IX_SummaryDayHour ON eddsdbo.QoS_FCM
END