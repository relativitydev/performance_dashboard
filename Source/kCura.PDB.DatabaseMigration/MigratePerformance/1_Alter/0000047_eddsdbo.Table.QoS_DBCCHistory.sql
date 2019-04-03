USE EDDSPerformance
GO

IF NOT EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'QoS_DBCCHistory' AND TABLE_SCHEMA = N'EDDSDBO') 
BEGIN
	CREATE TABLE [eddsdbo].[QoS_DBCCHistory]
	(
		[DbccHistoryID] INT IDENTITY ( 1 , 1 ),PRIMARY KEY (DbccHistoryID),
		[DBName] [nvarchar](255),
		[CompletedOn] [datetime],
		[IsCommandBased] [BIT]
	)
END
GO