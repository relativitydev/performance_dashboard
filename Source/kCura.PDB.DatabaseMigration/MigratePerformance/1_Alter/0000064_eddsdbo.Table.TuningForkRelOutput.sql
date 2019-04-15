USE EDDSPerformance
GO

IF NOT EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'TuningForkRelOutput' AND TABLE_SCHEMA = N'EDDSDBO') 
BEGIN
	CREATE TABLE [eddsdbo].[TuningForkRelOutput](
		[ID] [int] IDENTITY(1,1) NOT NULL,
		[Section] [varchar](200) NOT NULL,
		[Name] [varchar](200) NOT NULL,
		[ActualValue] [varchar](max) NOT NULL,
		[DefaultValue] [varchar](max) NULL,
		[kIE_Status] [varchar](100) NOT NULL,
		[Recommendation] [varchar](max) NULL,
		[MachineName] [varchar](100) NOT NULL,
		[Description] [varchar](max) NULL,
	 CONSTRAINT [PK_TuningForkRelOutput] PRIMARY KEY CLUSTERED 
	(
		[ID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO