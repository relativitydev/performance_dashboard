USE [EDDSPerformance]
GO

/****** Object:  Table [eddsdbo].[Server]    Script Date: 03/14/2014 10:52:18 ******/
IF NOT EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'Server' AND TABLE_SCHEMA = N'EDDSDBO')
BEGIN
	CREATE TABLE [eddsdbo].[Server](
		[ServerID] [int] IDENTITY(1,1) NOT NULL,
		[ServerName] [varchar](100) NOT NULL,
		[CreatedOn] [datetime] NOT NULL,
		[DeletedOn] [datetime] NULL,
		[ServerTypeID] [int] NOT NULL,
		[ServerIPAddress] [varchar](100) NULL,
		[IgnoreServer] [bit] NULL,
		[ResponsibleAgent] [nvarchar](max) NULL,
	 CONSTRAINT [PK_Server] PRIMARY KEY CLUSTERED 
	(
		[ServerID] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO