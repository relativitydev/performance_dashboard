USE EDDSPerformance
GO

IF NOT EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'QoS_WaitDetail' AND TABLE_SCHEMA = N'EDDSDBO') 
BEGIN
	CREATE TABLE eddsdbo.QoS_WaitDetail
	(
		WaitDetailID INT IDENTITY (1,1) PRIMARY KEY (WaitDetailID)
		,WaitSummaryID INT
		,WaitTypeID INT
		,CumulativeWaitMs BIGINT
		,CumulativeSignalWaitMs BIGINT
		,DifferentialWaitMs BIGINT
		,DifferentialSignalWaitMs BIGINT
	)
END
GO