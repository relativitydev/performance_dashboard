USE [EDDSPerformance]
GO

/****** Object:  Table [eddsdbo].[ServerDiskDW]    Script Date: 03/14/2014 10:53:13 ******/
IF NOT EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'ServerDiskDW' AND TABLE_SCHEMA = N'EDDSDBO')
BEGIN
	CREATE TABLE [eddsdbo].[ServerDiskDW](
		[ServerDiskDWID] [int] IDENTITY(1,1) NOT NULL,
		[CreatedOn] [datetime] NOT NULL,
		[ServerID] [int] NOT NULL,
		[DiskNumber] [int] NOT NULL,
		[DiskAvgSecPerRead] [decimal](10, 2) NULL,
		[DiskAvgSecPerWrite] [decimal](10, 2) NULL,
		[DriveLetter] [nvarchar](300) NULL,
	 CONSTRAINT [PK_ServerDiskDW] PRIMARY KEY CLUSTERED 
	(
		[ServerDiskDWID] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]
END
GO
