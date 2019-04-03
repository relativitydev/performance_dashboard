USE EDDSPerformance
GO

IF NOT EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'QoS_UserXInstanceSummary' AND TABLE_SCHEMA = N'EDDSDBO') 
BEGIN
	CREATE TABLE [EDDSDBO].[QoS_UserXInstanceSummary](
		[UserXID] [int] IDENTITY(1,1) NOT NULL,
		[ServerArtifactID] [int] NULL,
		[QoSHourID] [bigint] NULL,
		GlassRunID INT,
		[SummaryDayHour] [datetime] NULL,
		[BusiestWorkspace] INT NULL,
		BusiestUser INT,
		[qtyActiveUsers] [int] NULL,
		[TotalExecutionTime] [int] NULL,
		[QConcurrency] [DECIMAL](10, 3) NULL,
		[AVGConcurrency] [DECIMAL](10, 3) NULL,
		[MaxConcurrency] [int] NULL,
		[ArrivalRate] [DECIMAL](10, 3) NULL,
		[SimpleQueryConcurrency] [DECIMAL](10, 3) NULL,
		[ComplexQueryConcurrency] [DECIMAL](10, 3) NULL,
		[TotalSimpleQuery] [int] NULL,
		[TotalComplexQuery] [int] NULL,
		[TotalViews] [int] NULL,
		[TotalEdits] [int] NULL,
		[TotalMassOps] [int] NULL,
		[TotalOtherActions] [int] NULL,
		[TotalSLRQ] [int] NULL,
		[TotalCLRQ] [int] NULL,
		[MaxConcurrenctUsers] [int] NULL,
		[maxArrivalRateUsers] [DECIMAL](10, 3) NULL,
		[ProductivityPercent] [DECIMAL](10, 3) NULL,
		[TotalUniqueUsers] [int] NULL,
		[TotalSQUsers] [int] NULL,
		[TotalCQUsers] [int] NULL,
		[TotalSQExecutionTime] [int] NULL,
		[TotalCQViewExecutionTime] [int] NULL,
		[TotalTOPQueryExecutionTime] [int] NULL,
		[TotalCOUNTQueryExecutionTime] [int] NULL,
		[TotalLongRunningExecutionTime] [int] NULL,
		[TotalSLRQExecutionTime] [int] NULL,
		[TotalCLRQExecutionTime] [int] NULL,
		[AverageSQPerUser] [DECIMAL](10, 3) NULL,
		[AverageCQPerUser] [DECIMAL](10, 3) NULL,
		[AverageViewsPerUser] [DECIMAL](10, 3) NULL,
		[AverageEditsPerUser] [DECIMAL](10, 3)  NULL,
		[AverageUserSLRQ] [DECIMAL](10, 3) NULL,
		[AverageUserCLRQ] [DECIMAL](10, 3) NULL,
		[AverageLVEPerUser] [DECIMAL](10, 3) NULL,
		[AverageLEPerUser] [DECIMAL](10, 3) NULL,
		[AvgSQScorePerUser] [DECIMAL](10, 3) NULL,
		[AvgCQScorePerUser] [DECIMAL](10, 3) NULL,
		[SLRQLimit] [int] NULL,
		[CLRQLimit] [int] NULL,
		[AverageUserSLRQWaits] [DECIMAL](10, 3) NULL,
		[AverageUserCLRQWaits] [DECIMAL](10, 3) NULL,
		[AVGSLRQOverage] [DECIMAL](10, 3)  NULL,
		[AVGCLRQOverage] [DECIMAL](10, 3)  NULL,
		RowHash binary(20),
	PRIMARY KEY CLUSTERED 
	(
		[UserXID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
END
GO