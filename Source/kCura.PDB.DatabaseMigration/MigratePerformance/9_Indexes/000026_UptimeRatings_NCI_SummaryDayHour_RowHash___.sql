USE [EDDSPerformance]
GO

DECLARE @enterprise bit;
DECLARE @SQL nvarchar(max);

IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'QoS_UptimeRatings') 
BEGIN
	IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_UptimeRatings]') AND name = N'NCI_SummaryDayHour_RowHash')
	BEGIN
		CREATE NONCLUSTERED INDEX [NCI_SummaryDayHour_RowHash] ON [eddsdbo].[QoS_UptimeRatings] 
		(
			[SummaryDayHour] ASC
		)
		INCLUDE ([HoursDown],[RowHash])
	END
END