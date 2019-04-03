USE [EDDSPerformance]
GO

IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'QoS_BackupHistory' AND TABLE_SCHEMA = 'eddsdbo') 
BEGIN
	IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_BackupHistory]') AND name = N'IX_DBName_BackupType')
	BEGIN
		CREATE NONCLUSTERED INDEX [IX_DBName_BackupType] ON [eddsdbo].[QoS_BackupHistory]
		(
			[DBName] ASC,
			[BackupType] ASC
		)
		INCLUDE ([CompletedOn])
	END
END