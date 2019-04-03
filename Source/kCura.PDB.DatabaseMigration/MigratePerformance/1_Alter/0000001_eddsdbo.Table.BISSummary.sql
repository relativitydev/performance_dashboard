USE [EDDSPerformance]
GO

/****** Object:  Table [eddsdbo].[BISSummary]    Script Date: 03/14/2014 10:47:42 ******/
IF NOT EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'BISSummary' AND TABLE_SCHEMA = N'EDDSDBO')
BEGIN
	CREATE TABLE [eddsdbo].[BISSummary](
		[BISSummaryID] [int] IDENTITY(1,1) NOT NULL,
		[CreatedOn] [datetime] NOT NULL,
		[CaseArtifactID] [int] NOT NULL,
		[MeasureDate] [date] NOT NULL,
		[TQCount] [int] NULL,
		[TotalNRQCount] [int] NULL,
		[NRLRQCount] [int] NULL,
		[StatusDay] [int] NULL,
		[StatusPercentageNRLRQDay] [int] NULL,
		[DocumentCount] [bigint] NULL,
	 CONSTRAINT [PK_BISSummary] PRIMARY KEY CLUSTERED 
	(
		[BISSummaryID] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]
END
GO