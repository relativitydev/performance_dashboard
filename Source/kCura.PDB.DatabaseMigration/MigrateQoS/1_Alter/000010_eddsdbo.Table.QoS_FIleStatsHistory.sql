USE EDDSQoS
GO

IF NOT EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'eddsdbo' AND TABLE_NAME = 'QoS_FileStatsHistory') 
BEGIN
	CREATE TABLE eddsdbo.QoS_FileStatsHistory
	(
		FileStatsHistoryID INT IDENTITY(1,1),
		SummaryDayHour DATETIME,
		DatabaseID SMALLINT,
		FileID SMALLINT,
		DBName NVARCHAR(255),
		CumulativeReads BIGINT,
		CumulativeWrites BIGINT,
		CumulativeIOStallReadMs BIGINT,
		CumulativeIOStallWriteMs BIGINT,
		DifferentialReads BIGINT,
		DifferentialWrites BIGINT,
		DifferentialIOStallReadMs BIGINT,
		DifferentialIOStallWriteMs BIGINT,
		IsDataFile BIT
		CONSTRAINT [PK_FileStatsHistoryID] PRIMARY KEY CLUSTERED 
		(
			[FileStatsHistoryID] ASC
		)
	)
END