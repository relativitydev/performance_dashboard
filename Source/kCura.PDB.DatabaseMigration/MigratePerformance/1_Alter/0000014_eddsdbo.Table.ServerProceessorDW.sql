USE [EDDSPerformance]
GO

/****** Object:  Table [eddsdbo].[ServerProcessorDW]    Script Date: 03/14/2014 11:04:50 ******/
IF NOT EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'ServerProcessorDW' AND TABLE_SCHEMA = N'EDDSDBO')
BEGIN
	CREATE TABLE [eddsdbo].[ServerProcessorDW](
		[ServerProcessorDWID] [int] IDENTITY(1,1) NOT NULL,
		[CreatedOn] [datetime] NOT NULL,
		[ServerID] [int] NOT NULL,
		[CoreNumber] [int] NOT NULL,
		[CPUProcessorTimePct] [decimal](10, 0) NULL,
	 CONSTRAINT [PK_ServerProcessorDW] PRIMARY KEY CLUSTERED 
	(
		[ServerProcessorDWID] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]
END
GO