USE [EDDSPerformance]

IF NOT EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'HourSearchAuditBatches' AND TABLE_SCHEMA = N'EDDSDBO') 
BEGIN
	CREATE TABLE [eddsdbo].[HourSearchAuditBatches](
		[HourId] [int] NOT NULL,
		[ServerId] [int] NOT NULL,
		[BatchesCreated] [int] NOT NULL,
		[BatchesCompleted] [int] NOT NULL,
	 CONSTRAINT [PK_HourSearchAuditBatches] PRIMARY KEY CLUSTERED 
	(
		[HourId] ASC,
		[ServerId] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

	ALTER TABLE [eddsdbo].[HourSearchAuditBatches]  WITH CHECK ADD  CONSTRAINT [FK_HourSearchAuditBatches_Hours] FOREIGN KEY([HourId])
	REFERENCES [eddsdbo].[Hours] ([ID])

	ALTER TABLE [eddsdbo].[HourSearchAuditBatches] CHECK CONSTRAINT [FK_HourSearchAuditBatches_Hours]

	ALTER TABLE [eddsdbo].[HourSearchAuditBatches]  WITH CHECK ADD  CONSTRAINT [FK_HourSearchAuditBatches_Server] FOREIGN KEY([ServerId])
	REFERENCES [eddsdbo].[Server] ([ServerID])

	ALTER TABLE [eddsdbo].[HourSearchAuditBatches] CHECK CONSTRAINT [FK_HourSearchAuditBatches_Server]
END