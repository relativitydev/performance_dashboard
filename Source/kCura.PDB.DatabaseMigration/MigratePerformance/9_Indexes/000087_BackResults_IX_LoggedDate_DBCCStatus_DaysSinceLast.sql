USE [EDDSPerformance]
GO

IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'eddsdbo' AND TABLE_NAME = 'QoS_BackResults') 
BEGIN
	IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_BackResults]') AND name = N'IX_LoggedDate_BackupStatus_DaysSinceLast')
	BEGIN
		CREATE INDEX IX_LoggedDate_BackupStatus_DaysSinceLast ON [EDDSPerformance].[eddsdbo].[QoS_BackResults] 
		(
			[LoggedDate]
		) 
		INCLUDE ([kdbbuID], 
		[BackupStatus], 
		[DaysSinceLast])
	END
END