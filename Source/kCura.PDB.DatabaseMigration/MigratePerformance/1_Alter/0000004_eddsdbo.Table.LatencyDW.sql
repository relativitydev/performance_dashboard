USE [EDDSPerformance]
GO

/****** Object:  Table [eddsdbo].[LatencyDW]    Script Date: 03/14/2014 10:49:41 ******/
IF NOT EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'LatencyDW' AND TABLE_SCHEMA = N'EDDSDBO')
BEGIN
	CREATE TABLE [eddsdbo].[LatencyDW](
		[LatencyDWID] [int] IDENTITY(1,1) NOT NULL,
		[MeasureDate] [date] NOT NULL,
		[MeasureHour] [int] NOT NULL,
		[CaseArtifactID] [int] NOT NULL,
		[AverageLatency] [int] NULL,
		[CreatedOn] [datetime] NOT NULL,
	 CONSTRAINT [PK_LatencyDW] PRIMARY KEY CLUSTERED 
	(
		[LatencyDWID] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]
END
GO