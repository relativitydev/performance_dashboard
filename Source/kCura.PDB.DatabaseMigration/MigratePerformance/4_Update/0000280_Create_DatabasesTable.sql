IF NOT EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'Databases' AND TABLE_SCHEMA = N'EDDSDBO') 
BEGIN
	CREATE TABLE [eddsdbo].[Databases](
		[ID] [int] IDENTITY(1,1) NOT NULL,
		[Name] [varchar](250) NOT NULL,
		[ServerId] [int] NOT NULL,
		[WorkspaceId] [int] NULL,
		[Type] [int] NOT NULL,
		[DeletedOn] [datetime] NULL,
		[Ignore] [bit] NOT NULL,
		[LastDbccDate] [datetime] NULL,
		[LastBackupLogDate] [datetime] NULL,
		[LastBackupDiffDate] [datetime] NULL,
		[LastBackupFullDate] [datetime] NULL,
		[LastBackupFullDuration] [int] NULL,
		[LastBackupDiffDuration] [int] NULL,
		[LogBackupsDuration] [int] NULL,
	 CONSTRAINT [PK_Databases] PRIMARY KEY CLUSTERED 
	(
		[ID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

	ALTER TABLE [eddsdbo].[Databases] ADD  CONSTRAINT [DF_Databases_Ignore]  DEFAULT ((0)) FOR [Ignore]

	ALTER TABLE [eddsdbo].[Databases]  WITH CHECK ADD  CONSTRAINT [FK_Databases_Server] FOREIGN KEY([ServerId])
	REFERENCES [eddsdbo].[Server] ([ServerID])

	ALTER TABLE [eddsdbo].[Databases] CHECK CONSTRAINT [FK_Databases_Server]
END