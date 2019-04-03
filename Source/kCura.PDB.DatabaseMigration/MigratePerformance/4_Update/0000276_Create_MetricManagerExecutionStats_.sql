USE [EDDSPerformance]

IF NOT EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'MetricManagerExecutionStats' AND TABLE_SCHEMA = N'EDDSDBO') 
BEGIN
	CREATE TABLE [eddsdbo].[MetricManagerExecutionStats](
		[ExecutionId] [uniqueidentifier] NOT NULL,
		[Start] [datetime] NOT NULL,
		[End] [datetime] NOT NULL,
		[Name] [varchar](50) NOT NULL,
		[TotalTime] [decimal](18, 4) NOT NULL,
		[MaxTime] [decimal](18, 4) NOT NULL,
		[Count] [int] NOT NULL
	) ON [PRIMARY]
END