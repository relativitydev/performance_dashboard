USE [EDDSQoS]
GO

IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'QoS_FileStatsHistory' AND TABLE_SCHEMA = 'eddsdbo') 
BEGIN
	IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_FileStatsHistory]') AND name = N'IX_SummaryDayHour_FileID_DBName')
	BEGIN
		CREATE NONCLUSTERED INDEX [IX_SummaryDayHour_FileID_DBName] ON [eddsdbo].[QoS_FileStatsHistory]
		(
			[SummaryDayHour] ASC,
			[FileID] ASC,
			[DBName] ASC
		)
	END
END