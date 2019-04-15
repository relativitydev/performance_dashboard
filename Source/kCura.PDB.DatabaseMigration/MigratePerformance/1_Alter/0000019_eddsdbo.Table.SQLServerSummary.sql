USE [EDDSPerformance]
GO

/****** Object:  Table [eddsdbo].[SQLServerSummary]    Script Date: 03/14/2014 11:07:16 ******/
IF NOT EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'SQLServerSummary' AND TABLE_SCHEMA = N'EDDSDBO')
BEGIN
	CREATE TABLE [eddsdbo].[SQLServerSummary](
		[SQLServerSummaryID] [int] IDENTITY(1,1) NOT NULL,
		[CreatedOn] [datetime] NOT NULL,
		[MeasureDate] [datetime] NOT NULL,
		[ServerID] [int] NOT NULL,
		[SQLPageLifeExpectancy] [decimal](10, 2) NULL,
	 CONSTRAINT [PK_SQLServerSummary] PRIMARY KEY CLUSTERED 
	(
		[SQLServerSummaryID] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]
END
GO