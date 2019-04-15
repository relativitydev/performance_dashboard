USE EDDSPerformance
GO

IF NOT EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'QoS_BackupHistory' AND TABLE_SCHEMA = N'EDDSDBO') 
BEGIN
	CREATE TABLE [eddsdbo].[QoS_BackupHistory] (
		[BackupHistoryID] [int] IDENTITY(1,1) NOT NULL,
		[DBName] [nvarchar](255) NULL,
		[CompletedOn] [datetime] NULL,
		[Duration] INT NULL,
		[BackupType] CHAR(1) NULL,
		[LoggedDate] DATETIME NULL,
		CONSTRAINT [PK_BackupHistory] PRIMARY KEY CLUSTERED 
		(
			[BackupHistoryID] ASC
		)
	)
END
GO