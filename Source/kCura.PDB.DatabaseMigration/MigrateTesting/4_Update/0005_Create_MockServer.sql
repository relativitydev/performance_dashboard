IF NOT EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'MockServer' AND TABLE_SCHEMA = N'EDDSDBO') 
BEGIN

	CREATE TABLE [eddsdbo].[MockServer](
		[ServerID] [int] IDENTITY(1,1) NOT NULL,
		[ServerName] [nvarchar](150) NULL,
		[CreatedOn] [datetime] NOT NULL,
		[DeletedOn] [datetime] NULL,
		[ServerTypeID] [int] NOT NULL,
		[IgnoreServer] [bit] NULL,
		[ArtifactID] [int] NULL,
		[LastServerBackup] [datetime] NULL,
	 CONSTRAINT [PK_MockServer] PRIMARY KEY CLUSTERED 
	(
		[ServerID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

	ALTER TABLE [eddsdbo].[MockServer] ADD  CONSTRAINT [DF_MockServer_CreatedOn]  DEFAULT (getutcdate()) FOR [CreatedOn]

END