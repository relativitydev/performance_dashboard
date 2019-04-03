USE [EDDSPerformance]
GO

/****** Object:  Table [eddsdbo].[ServerProcessorSummary]    Script Date: 03/14/2014 11:05:38 ******/
IF NOT EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'ServerProcessorSummary' AND TABLE_SCHEMA = N'EDDSDBO')
BEGIN
	CREATE TABLE [eddsdbo].[ServerProcessorSummary](
		[ServerProcessorSummaryID] [int] IDENTITY(1,1) NOT NULL,
		[CreatedOn] [datetime] NOT NULL,
		[MeasureDate] [datetime] NOT NULL,
		[ServerID] [int] NOT NULL,
		[CoreNumber] [int] NOT NULL,
		[CPUProcessorTimePct] [decimal](10, 0) NULL,
	 CONSTRAINT [PK_ServerProcessorSummary] PRIMARY KEY CLUSTERED 
	(
		[ServerProcessorSummaryID] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]
END
GO