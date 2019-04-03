USE EDDSPerformance
GO

IF NOT EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'QoS_WaitSummary' AND TABLE_SCHEMA = N'EDDSDBO') 
BEGIN
	CREATE TABLE eddsdbo.QoS_WaitSummary
	(
		WaitSummaryID INT IDENTITY (1,1) PRIMARY KEY (WaitSummaryID)
		,QoSHourID BIGINT
		,SummaryDayHour DATETIME
		,ServerArtifactID INT
		,ServerName nvarchar(150)
		,LastSqlRestart datetime
		,SignalWaitsRatio decimal(4,3)
		,RunCondition INT
	)
END
GO