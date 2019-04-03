IF NOT EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'DatabaseGaps' AND TABLE_SCHEMA = N'EDDSDBO') 
BEGIN
	CREATE TABLE [eddsdbo].[DatabaseGaps](
		[ID] [int] IDENTITY(1,1) NOT NULL,
		[DatabaseId] [int] NOT NULL,
		[GapStart] [datetime] NOT NULL,
		[GapEnd] [datetime] NULL,
		[Duration] [int] NULL,
		[ActivityType] [int] NOT NULL,
	 CONSTRAINT [PK_DatabaseGaps] PRIMARY KEY CLUSTERED 
	(
		[ID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

	ALTER TABLE [eddsdbo].[DatabaseGaps]  WITH CHECK ADD  CONSTRAINT [FK_DatabaseGaps_Databases] FOREIGN KEY([DatabaseId])
	REFERENCES [eddsdbo].[Databases] ([ID])

	ALTER TABLE [eddsdbo].[DatabaseGaps] CHECK CONSTRAINT [FK_DatabaseGaps_Databases]
END

