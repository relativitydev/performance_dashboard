USE EDDSPerformance
GO

IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_UptimeRatings]') AND name = N'NCQoS_SummaryDayHour_HoursDown')
BEGIN
	DROP INDEX NCQoS_SummaryDayHour_HoursDown ON eddsdbo.QoS_UptimeRatings
END