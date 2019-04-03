USE [EDDSPerformance]
GO

IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'eddsdbo' AND TABLE_NAME = 'QoS_DBCCResults') 
BEGIN
	IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_DBCCResults]') AND name = N'IX_LoggedDate_DBCCStatus_DaysSinceLast')
	BEGIN
		CREATE INDEX IX_LoggedDate_DBCCStatus_DaysSinceLast ON [EDDSPerformance].[eddsdbo].[QoS_DBCCResults] 
		(
			[LoggedDate]
		) 
		INCLUDE ([kdbccbID], 
		[DBCCStatus], 
		[DaysSinceLast])
	END
END