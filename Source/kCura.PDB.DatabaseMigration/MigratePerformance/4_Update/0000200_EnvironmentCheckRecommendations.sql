USE [EDDSPerformance]


IF NOT EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'eddsdbo' AND TABLE_NAME = 'EnvironmentCheckRecommendations') 
BEGIN
	CREATE TABLE [eddsdbo].[EnvironmentCheckRecommendations](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Scope] [varchar](200) NOT NULL,
	[Name] [varchar](200) NOT NULL,
	[Description] [varchar](max) NULL,
	[Status] [varchar](100) NOT NULL,
	[Recommendation] [varchar](max) NULL,
	[Value] [varchar](max) NULL,
	[Section] [varchar](200) NOT NULL,
	 CONSTRAINT [PK_EnvironmentCheckRecommendations] PRIMARY KEY CLUSTERED 
	(
		[ID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END




IF EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'eddsdbo' AND TABLE_NAME = 'TuningForkRelOutput') 
BEGIN
	drop table eddsdbo.TuningForkRelOutput
END

IF EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'eddsdbo' AND TABLE_NAME = 'TuningForkSysOutput') 
BEGIN
	drop table eddsdbo.TuningForkSysOutput
END