USE [EDDSPerformance]
GO

/****** Object:  Table [eddsdbo].[ServerType]    Script Date: 03/14/2014 11:06:26 ******/
IF NOT EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'ServerType' AND TABLE_SCHEMA = N'EDDSDBO')
BEGIN
	CREATE TABLE [eddsdbo].[ServerType](
		[ServerTypeID] [int] IDENTITY(1,1) NOT NULL,
		[ServerTypeName] [nvarchar](50) NOT NULL,
		[CreatedOn] [datetime] NOT NULL,
		[ArtifactID] [int] NULL,
	 CONSTRAINT [PK_ServerType] PRIMARY KEY CLUSTERED 
	(
		[ServerTypeID] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]
END
GO