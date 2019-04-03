IF NOT EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'MockDbccServerResults' AND TABLE_SCHEMA = N'EDDSDBO') 
BEGIN

	CREATE TABLE [eddsdbo].[MockDbccServerResults](
		[Server] [nvarchar](150) NOT NULL,
		[Database] [nvarchar](100) NOT NULL,
		[CaseArtifactId] [int] NULL,
		[LastCleanDbccDate] [datetime] NOT NULL,
	 CONSTRAINT [PK_MockDbccServerResults] PRIMARY KEY CLUSTERED 
	(
		[Server] ASC,
		[Database] ASC,
		[LastCleanDbccDate] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

END