USE [EDDSPerformance]
GO

/****** Object:  Table [eddsdbo].[PerformanceSummary]    Script Date: 03/14/2014 10:51:36 ******/
IF NOT EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'PerformanceSummary' AND TABLE_SCHEMA = N'EDDSDBO')
BEGIN
	CREATE TABLE [eddsdbo].[PerformanceSummary](
		[PerformanceSummaryID] [int] IDENTITY(1,1) NOT NULL,
		[CreatedOn] [datetime] NOT NULL,
		[CaseArtifactID] [int] NOT NULL,
		[MeasureDate] [datetime] NOT NULL,
		[UserCount] [int] NULL,
		[ErrorCount] [int] NULL,
		[LRQCount] [int] NULL,
		[AverageLatency] [int] NULL,
		[NRLRQCount] [int] NULL,
		[TotalQCount] [int] NULL,
		[TotalNRQCount] [int] NULL,
	 CONSTRAINT [PK_PerformanceSummary] PRIMARY KEY CLUSTERED 
	(
		[PerformanceSummaryID] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]
END
GO