USE [EDDSPerformance]
GO

IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'QoS_BackupHistory' AND TABLE_SCHEMA = 'eddsdbo') 
BEGIN
	IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_BackupHistory]') AND name = N'IX_CompletedOn')
	BEGIN
		CREATE NONCLUSTERED INDEX [IX_CompletedOn] ON [eddsdbo].[QoS_BackupHistory]
		(
			[CompletedOn] ASC
		)
		INCLUDE ([DBName])
	END
END