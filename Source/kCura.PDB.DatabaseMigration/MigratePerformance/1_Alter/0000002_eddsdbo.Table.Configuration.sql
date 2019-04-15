USE [EDDSPerformance]
GO

/****** Object:  Table [eddsdbo].[Configuration]    Script Date: 03/14/2014 10:48:37 ******/
IF NOT EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'Configuration' AND TABLE_SCHEMA = N'EDDSDBO')
BEGIN
	CREATE TABLE [eddsdbo].[Configuration](
		[Section] [nvarchar](200) NOT NULL,
		[Name] [nvarchar](200) NOT NULL,
		[Value] [nvarchar](max) NOT NULL,
		[MachineName] [varchar](100) NOT NULL,
		[Description] [nvarchar](max) NULL,
	 CONSTRAINT [PK_Configuration] PRIMARY KEY CLUSTERED 
	(
		[Section] ASC,
		[Name] ASC,
		[MachineName] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO