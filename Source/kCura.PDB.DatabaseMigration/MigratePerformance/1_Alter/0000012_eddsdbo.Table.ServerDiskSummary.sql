USE [EDDSPerformance]
GO

/****** Object:  Table [eddsdbo].[ServerDiskSummary]    Script Date: 03/14/2014 10:53:28 ******/
IF NOT EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'ServerDiskSummary' AND TABLE_SCHEMA = N'EDDSDBO')
BEGIN
	CREATE TABLE [eddsdbo].[ServerDiskSummary](
		[ServerDiskSummaryID] [int] IDENTITY(1,1) NOT NULL,
		[CreatedOn] [datetime] NOT NULL,
		[MeasureDate] [datetime] NOT NULL,
		[ServerID] [int] NOT NULL,
		[DiskNumber] [int] NOT NULL,
		[DiskAvgSecPerRead] [decimal](10, 2) NULL,
		[DiskAvgSecPerWrite] [decimal](10, 2) NULL,
		[DriveLetter] [nvarchar](300) NULL,
	 CONSTRAINT [PK_ServerDiskSummary] PRIMARY KEY CLUSTERED 
	(
		[ServerDiskSummaryID] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]
END
GO