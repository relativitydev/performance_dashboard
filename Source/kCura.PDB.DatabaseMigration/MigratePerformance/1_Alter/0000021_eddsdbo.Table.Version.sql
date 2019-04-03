USE [EDDSPerformance]
GO

/****** Object:  Table [eddsdbo].[Version]    Script Date: 03/14/2014 11:08:24 ******/
IF NOT EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'Version' AND TABLE_SCHEMA = N'EDDSDBO')
BEGIN
	CREATE TABLE [eddsdbo].[Version](
		[ApplicationVersion] [nchar](10) NULL
	) ON [PRIMARY]
END
GO