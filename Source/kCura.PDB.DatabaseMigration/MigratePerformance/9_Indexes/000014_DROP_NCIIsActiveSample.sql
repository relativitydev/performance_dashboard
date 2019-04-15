USE EDDSPerformance
GO

IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_SampleHistory]') AND name = N'NCIIsActiveSample_SummaryDayHour')
BEGIN
	DROP INDEX NCIIsActiveSample_SummaryDayHour ON eddsdbo.QoS_SampleHistory
END