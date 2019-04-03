USE EDDSPerformance
GO

IF NOT EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'QoS_UptimeRatings' AND TABLE_SCHEMA = N'EDDSDBO') 
BEGIN
	CREATE TABLE EDDSDBO.QoS_UptimeRatings
	(
		UpRatingID int IDENTITY ( 1 , 1 ),PRIMARY KEY (UpRatingID)
		,HoursDown INT CONSTRAINT DF_UptimeRatings_HoursDown DEFAULT 0
		,UptimeScore DECIMAL (5, 2) CONSTRAINT DF_UptimeRatings_UptimeScore DEFAULT 100 --This is the uptime score over the last 2160 hours.
		,SummaryDayHour datetime
		,RowHash binary(20)
	)
END
GO