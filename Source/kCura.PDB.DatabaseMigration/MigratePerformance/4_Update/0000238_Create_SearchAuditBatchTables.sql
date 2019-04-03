USE [EDDSPerformance]
GO

IF NOT EXISTS (select 1 from sysobjects where [name] = 'SearchAuditBatch' and type = 'U')  
BEGIN
	CREATE TABLE [eddsdbo].[SearchAuditBatch](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[HourId] [int] NOT NULL,
	[WorkspaceId] [int] NOT NULL,
	[ServerId] [int] NOT NULL,
	[BatchStart] [bigint] NOT NULL,
	[BatchSize] [int] NOT NULL,
	 CONSTRAINT [PK_SearchAuditBatch] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
END

IF NOT EXISTS (select 1 from sysobjects where [name] = 'SearchAuditBatchResult' and type = 'U')  
BEGIN
	CREATE TABLE [eddsdbo].[SearchAuditBatchResult](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BatchId] [int] NOT NULL,
	[UserId] [int] NOT NULL,
	[TotalComplexQueries] [bigint] NOT NULL,
	[TotalLongRunningQueries] [bigint] NOT NULL,
	[TotalSimpleLongRunningQueries] [bigint] NOT NULL,
	[TotalQueries] [bigint] NOT NULL,
	 CONSTRAINT [PK_SearchAuditBatchResult] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
	
	ALTER TABLE [eddsdbo].[SearchAuditBatchResult]  WITH CHECK ADD  CONSTRAINT [FK_SearchAuditBatchResult_SearchAuditBatch] FOREIGN KEY([BatchId])
	REFERENCES [eddsdbo].[SearchAuditBatch] ([Id])
	
	ALTER TABLE [eddsdbo].[SearchAuditBatchResult] CHECK CONSTRAINT [FK_SearchAuditBatchResult_SearchAuditBatch]
END