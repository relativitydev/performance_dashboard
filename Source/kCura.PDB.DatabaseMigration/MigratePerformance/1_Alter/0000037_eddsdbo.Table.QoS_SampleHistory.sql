USE EDDSPerformance
GO

IF NOT EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'QoS_SampleHistory' AND TABLE_SCHEMA = N'EDDSDBO') 
BEGIN
	CREATE TABLE EDDSDBO.QoS_SampleHistory
	(
		QSampleID INT IDENTITY ( 1 , 1 ),PRIMARY KEY (QSampleID)
		,QoSHourID bigint
		,SummaryDayHour datetime
		,ServerArtifactID int
		,ArrivalRate DECIMAL (10, 3)
		,AVGConcurrency DECIMAL (10, 3)
		,IsActiveSample bit
		,IsActive4Sample bit
		,IsActiveWeeklySample bit
		,IsActiveWeekly4Sample bit
		,SampleDate datetime  ---if an hour is no longer an active sample, this will contain the date that it was last a sample.
		,RowHash binary(20)
	)
END
GO