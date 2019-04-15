IF NOT EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'MockBackupSet' AND TABLE_SCHEMA = N'EDDSDBO') 
BEGIN

	CREATE TABLE [eddsdbo].[MockBackupSet](
		[Server] [nvarchar](150) NOT NULL,
		[Database] [nvarchar](100) NOT NULL,
		[BackupStartDate] [datetime] NOT NULL,
		[BackupEndDate] [datetime] NULL,
		[BackupType] [char](1) NOT NULL,
	 CONSTRAINT [PK_MockBackupSet] PRIMARY KEY CLUSTERED 
	(
		[Server] ASC,
		[Database] ASC,
		[BackupStartDate] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

END