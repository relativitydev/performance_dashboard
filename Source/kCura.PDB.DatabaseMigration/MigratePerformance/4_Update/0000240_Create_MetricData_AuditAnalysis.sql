USE [EDDSPerformance]

IF NOT EXISTS (select 1 from sysobjects where [name] = 'MetricData_AuditAnalysis' and type = 'U')  
BEGIN
	CREATE TABLE [eddsdbo].[MetricData_AuditAnalysis](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[MetricDataId] [int] NOT NULL,
		[UserId] [int] NOT NULL,
		[TotalComplexQueries] [bigint] NOT NULL,
		[TotalLongRunningQueries] [bigint] NOT NULL,
		[TotalSimpleLongRunningQueries] [bigint] NOT NULL,
		[TotalQueries] [bigint] NOT NULL,
	 CONSTRAINT [PK_MetricData_AuditAnalysis] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

	ALTER TABLE [eddsdbo].[MetricData_AuditAnalysis]  WITH CHECK ADD  CONSTRAINT [FK_MetricData_AuditAnalysis_MetricData] FOREIGN KEY([MetricDataId])
	REFERENCES [eddsdbo].[MetricData] ([ID])

	ALTER TABLE [eddsdbo].[MetricData_AuditAnalysis] CHECK CONSTRAINT [FK_MetricData_AuditAnalysis_MetricData]
END