USE EDDSPerformance
GO

IF NOT EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'QoS_FileLatencySummary' AND TABLE_SCHEMA = N'EDDSDBO') 
BEGIN
	CREATE TABLE eddsdbo.QoS_FileLatencySummary
	(
		FileLatencyID INT IDENTITY(1,1),
		ServerArtifactID INT,
		QoSHourID BIGINT,
		SummaryDayHour DATETIME,
		HighestLatencyDatabase NVARCHAR(255),
		ReadLatencyMs BIGINT,
		WriteLatencyMs BIGINT,
		LatencyScore DECIMAL(5,2),
		IOWaitsHigh BIT,
		IsDataFile BIT,
		CONSTRAINT [PK_FileLatencyID] PRIMARY KEY CLUSTERED 
		(
			FileLatencyID ASC
		)
	)
END
GO