USE EDDSPerformance 

-- Categories
IF NOT EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'eddsdbo' AND TABLE_NAME = 'Categories') 
BEGIN
	CREATE TABLE [eddsdbo].[Categories](
		[ID] [int] IDENTITY(1,1) NOT NULL,
		[CategoryTypeID] [int] NOT NULL,
		[HourID] [int] NOT NULL,
	 CONSTRAINT [PK_Categories] PRIMARY KEY CLUSTERED 
	(
		[ID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
END

-- Category Scores
IF NOT EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'eddsdbo' AND TABLE_NAME = 'CategoryScores') 
BEGIN
	CREATE TABLE [eddsdbo].[CategoryScores](
		[ID] [int] IDENTITY(1,1) NOT NULL,
		[CategoryID] [int] NOT NULL,
		[ServerID] [int] NULL,
		[Score] [decimal](9, 0) NULL,
	 CONSTRAINT [PK_CategoryScores] PRIMARY KEY CLUSTERED 
	(
		[ID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
END

-- Category Types
IF NOT EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'eddsdbo' AND TABLE_NAME = 'CategoryTypes') 
BEGIN
	CREATE TABLE [eddsdbo].[CategoryTypes](
		[ID] [int] NOT NULL,
		[Name] [varchar](50) NOT NULL,
		[Description] [varchar](max) NULL,
	 CONSTRAINT [PK_CategoryTypes] PRIMARY KEY CLUSTERED 
	(
		[ID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END

-- Hours
IF NOT EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'eddsdbo' AND TABLE_NAME = 'Hours') 
BEGIN
	CREATE TABLE [eddsdbo].[Hours](
		[ID] [int] IDENTITY(1,1) NOT NULL,
		[HourTimeStamp] [datetime] NOT NULL,
		[Score] [decimal](9, 0) NULL,
		[InSample] [bit] NOT NULL,
	 CONSTRAINT [PK_Hours] PRIMARY KEY CLUSTERED 
	(
		[ID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
END

-- Metric Data
IF NOT EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'eddsdbo' AND TABLE_NAME = 'MetricData') 
BEGIN
	CREATE TABLE [eddsdbo].[MetricData](
		[ID] [int] IDENTITY(1,1) NOT NULL,
		[MetricID] [int] NOT NULL,
		[ServerID] [int] NULL,
		[Data] [varchar](max) NULL,
		[Score] [decimal](9, 0) NULL,
	 CONSTRAINT [PK_MetricData] PRIMARY KEY CLUSTERED 
	(
		[ID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END

-- Metrics
IF NOT EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'eddsdbo' AND TABLE_NAME = 'Metrics') 
BEGIN
	CREATE TABLE [eddsdbo].[Metrics](
		[ID] [int] IDENTITY(1,1) NOT NULL,
		[MetricTypeID] [int] NOT NULL,
		[HourID] [int] NOT NULL,
	 CONSTRAINT [PK_Metrics] PRIMARY KEY CLUSTERED 
	(
		[ID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
END

-- Metric Types
IF NOT EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'eddsdbo' AND TABLE_NAME = 'MetricTypes') 
BEGIN
	CREATE TABLE [eddsdbo].[MetricTypes](
		[ID] [int] NOT NULL,
		[Name] [varchar](50) NOT NULL,
		[Description] [varchar](max) NULL,
		[SampleType] [int] NOT NULL,
	 CONSTRAINT [PK_MetricTypes] PRIMARY KEY CLUSTERED 
	(
		[ID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
-- Metric Types To Category Types
IF NOT EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'eddsdbo' AND TABLE_NAME = 'MetricTypesToCategoryTypes') 
BEGIN
	CREATE TABLE [eddsdbo].[MetricTypesToCategoryTypes](
		[MetricTypeID] [int] NOT NULL,
		[CategoryTypeID] [int] NOT NULL,
	 CONSTRAINT [PK_MetricTypesToCategoryTypes] PRIMARY KEY CLUSTERED 
	(
		[MetricTypeID] ASC,
		[CategoryTypeID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END

--Trust Weekly Scores
IF NOT EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'eddsdbo' AND TABLE_NAME = 'TrustWeeklyScores') 
BEGIN
	CREATE TABLE [eddsdbo].[TrustWeeklyScores](
		[ID] [int] IDENTITY(1,1) NOT NULL,
		[TrustWeekID] [int] NOT NULL,
		[Score] [decimal](9, 0) NULL,
	 CONSTRAINT [PK_TrustWeeklyScores] PRIMARY KEY CLUSTERED 
	(
		[ID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
END

-- Trust Weeks
IF NOT EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'eddsdbo' AND TABLE_NAME = 'TrustWeeks') 
	BEGIN
	CREATE TABLE [eddsdbo].[TrustWeeks](
		[ID] [int] IDENTITY(1,1) NOT NULL,
		[WeekTimeStamp] [datetime] NOT NULL,
	 CONSTRAINT [PK_TrustWeeks] PRIMARY KEY CLUSTERED 
	(
		[ID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
END

IF OBJECT_ID('eddsdbo.FK_Categories_CategoryTypes', 'F') IS NULL
BEGIN
ALTER TABLE [eddsdbo].[Categories]  WITH CHECK ADD  CONSTRAINT [FK_Categories_CategoryTypes] FOREIGN KEY([CategoryTypeID])
REFERENCES [eddsdbo].[CategoryTypes] ([ID])

ALTER TABLE [eddsdbo].[Categories] CHECK CONSTRAINT [FK_Categories_CategoryTypes]
END

IF OBJECT_ID('eddsdbo.FK_Categories_Hours', 'F') IS NULL
BEGIN
ALTER TABLE [eddsdbo].[Categories]  WITH CHECK ADD  CONSTRAINT [FK_Categories_Hours] FOREIGN KEY([HourID])
REFERENCES [eddsdbo].[Hours] ([ID])

ALTER TABLE [eddsdbo].[Categories] CHECK CONSTRAINT [FK_Categories_Hours]
END

IF OBJECT_ID('eddsdbo.FK_CategoryScores_Categories', 'F') IS NULL
BEGIN
ALTER TABLE [eddsdbo].[CategoryScores]  WITH CHECK ADD  CONSTRAINT [FK_CategoryScores_Categories] FOREIGN KEY([CategoryID])
REFERENCES [eddsdbo].[Categories] ([ID])

ALTER TABLE [eddsdbo].[CategoryScores] CHECK CONSTRAINT [FK_CategoryScores_Categories]
END

IF OBJECT_ID('eddsdbo.FK_CategoryScores_Server', 'F') IS NULL
BEGIN
ALTER TABLE [eddsdbo].[CategoryScores]  WITH CHECK ADD  CONSTRAINT [FK_CategoryScores_Server] FOREIGN KEY([ServerID])
REFERENCES [eddsdbo].[Server] ([ServerID])

ALTER TABLE [eddsdbo].[CategoryScores] CHECK CONSTRAINT [FK_CategoryScores_Server]
END

IF OBJECT_ID('eddsdbo.FK_MetricData_Metrics', 'F') IS NULL
BEGIN
ALTER TABLE [eddsdbo].[MetricData]  WITH CHECK ADD  CONSTRAINT [FK_MetricData_Metrics] FOREIGN KEY([MetricID])
REFERENCES [eddsdbo].[Metrics] ([ID])

ALTER TABLE [eddsdbo].[MetricData] CHECK CONSTRAINT [FK_MetricData_Metrics]
END

IF OBJECT_ID('eddsdbo.FK_MetricData_Server', 'F') IS NULL
BEGIN
ALTER TABLE [eddsdbo].[MetricData]  WITH CHECK ADD  CONSTRAINT [FK_MetricData_Server] FOREIGN KEY([ServerID])
REFERENCES [eddsdbo].[Server] ([ServerID])

ALTER TABLE [eddsdbo].[MetricData] CHECK CONSTRAINT [FK_MetricData_Server]
END

IF OBJECT_ID('eddsdbo.FK_Metrics_Hours', 'F') IS NULL
BEGIN
ALTER TABLE [eddsdbo].[Metrics]  WITH CHECK ADD  CONSTRAINT [FK_Metrics_Hours] FOREIGN KEY([HourID])
REFERENCES [eddsdbo].[Hours] ([ID])

ALTER TABLE [eddsdbo].[Metrics] CHECK CONSTRAINT [FK_Metrics_Hours]
END

IF OBJECT_ID('eddsdbo.FK_Metrics_MetricTypes', 'F') IS NULL
BEGIN
ALTER TABLE [eddsdbo].[Metrics]  WITH CHECK ADD  CONSTRAINT [FK_Metrics_MetricTypes] FOREIGN KEY([MetricTypeID])
REFERENCES [eddsdbo].[MetricTypes] ([ID])

ALTER TABLE [eddsdbo].[Metrics] CHECK CONSTRAINT [FK_Metrics_MetricTypes]
END

IF OBJECT_ID('eddsdbo.FK_MetricTypesToCategoryTypes_CategoryTypes', 'F') IS NULL
BEGIN
ALTER TABLE [eddsdbo].[MetricTypesToCategoryTypes]  WITH CHECK ADD  CONSTRAINT [FK_MetricTypesToCategoryTypes_CategoryTypes] FOREIGN KEY([CategoryTypeID])
REFERENCES [eddsdbo].[CategoryTypes] ([ID])

ALTER TABLE [eddsdbo].[MetricTypesToCategoryTypes] CHECK CONSTRAINT [FK_MetricTypesToCategoryTypes_CategoryTypes]
END

IF OBJECT_ID('eddsdbo.FK_MetricTypesToCategoryTypes_MetricTypes', 'F') IS NULL
BEGIN
ALTER TABLE [eddsdbo].[MetricTypesToCategoryTypes]  WITH CHECK ADD  CONSTRAINT [FK_MetricTypesToCategoryTypes_MetricTypes] FOREIGN KEY([MetricTypeID])
REFERENCES [eddsdbo].[MetricTypes] ([ID])

ALTER TABLE [eddsdbo].[MetricTypesToCategoryTypes] CHECK CONSTRAINT [FK_MetricTypesToCategoryTypes_MetricTypes]
END

IF OBJECT_ID('eddsdbo.FK_TrustWeeklyScores_TrustWeeks', 'F') IS NULL
BEGIN
ALTER TABLE [eddsdbo].[TrustWeeklyScores]  WITH CHECK ADD  CONSTRAINT [FK_TrustWeeklyScores_TrustWeeks] FOREIGN KEY([TrustWeekID])
REFERENCES [eddsdbo].[TrustWeeks] ([ID])

ALTER TABLE [eddsdbo].[TrustWeeklyScores] CHECK CONSTRAINT [FK_TrustWeeklyScores_TrustWeeks]
END