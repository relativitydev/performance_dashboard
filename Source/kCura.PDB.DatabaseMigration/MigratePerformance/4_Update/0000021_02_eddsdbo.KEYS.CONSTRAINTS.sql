USE EDDSPerformance;
GO


/****** Object:  Default [DF_Server_CreatedOn]    Script Date: 10/11/2011 13:32:09 ******/
IF EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'Server' AND TABLE_SCHEMA = N'EDDSDBO') and OBJECT_ID('eddsdbo.DF_Server_CreatedOn', 'D') IS NULL
	ALTER TABLE [eddsdbo].[Server] ADD  CONSTRAINT [DF_Server_CreatedOn]  DEFAULT (getutcdate()) FOR [CreatedOn]
GO

/****** Object:  Default [DF_LRQCountDW_CreatedOn]    Script Date: 10/11/2011 13:32:10 ******/
IF EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'LRQCountDW' AND TABLE_SCHEMA = N'EDDSDBO') and OBJECT_ID('eddsdbo.DF_LRQCountDW_CreatedOn', 'D') IS NULL
	ALTER TABLE [eddsdbo].[LRQCountDW] ADD  CONSTRAINT [DF_LRQCountDW_CreatedOn]  DEFAULT (getutcdate()) FOR [CreatedOn]
GO

/****** Object:  Default [DF_MeasureType_IsActive]    Script Date: 10/11/2011 13:32:10 ******/
IF EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'MeasureType' AND TABLE_SCHEMA = N'EDDSDBO') and OBJECT_ID('eddsdbo.DF_MeasureType_IsActive', 'D') IS NULL
	ALTER TABLE [eddsdbo].[MeasureType] ADD  CONSTRAINT [DF_MeasureType_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO

/****** Object:  Default [DF_MeasureType_IsDeleted]    Script Date: 10/11/2011 13:32:10 ******/
IF EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'MeasureType' AND TABLE_SCHEMA = N'EDDSDBO') and OBJECT_ID('eddsdbo.DF_MeasureType_IsDeleted', 'D') IS NULL
	ALTER TABLE [eddsdbo].[MeasureType] ADD  CONSTRAINT [DF_MeasureType_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO

/****** Object:  Default [DF_MeasureType_CreatedOn]    Script Date: 10/11/2011 13:32:10 ******/
IF EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'MeasureType' AND TABLE_SCHEMA = N'EDDSDBO') and OBJECT_ID('eddsdbo.DF_MeasureType_CreatedOn', 'D') IS NULL
	ALTER TABLE [eddsdbo].[MeasureType] ADD  CONSTRAINT [DF_MeasureType_CreatedOn]  DEFAULT (getutcdate()) FOR [CreatedOn]
GO

/****** Object:  Default [DF_LatencyDW_CreatedOn]    Script Date: 10/11/2011 13:32:10 ******/
IF EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'LatencyDW' AND TABLE_SCHEMA = N'EDDSDBO') and OBJECT_ID('eddsdbo.DF_LatencyDW_CreatedOn', 'D') IS NULL
	ALTER TABLE [eddsdbo].[LatencyDW] ADD  CONSTRAINT [DF_LatencyDW_CreatedOn]  DEFAULT (getutcdate()) FOR [CreatedOn]
GO

/****** Object:  Default [DF_ErrorCountDW_CreatedOn]    Script Date: 10/11/2011 13:32:10 ******/
IF EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'ErrorCountDW' AND TABLE_SCHEMA = N'EDDSDBO') and OBJECT_ID('eddsdbo.DF_ErrorCountDW_CreatedOn', 'D') IS NULL
	ALTER TABLE [eddsdbo].[ErrorCountDW] ADD  CONSTRAINT [DF_ErrorCountDW_CreatedOn]  DEFAULT (getutcdate()) FOR [CreatedOn]
GO

/****** Object:  Default [DF_ServerType_CreatedOn]    Script Date: 10/11/2011 13:32:10 ******/
IF EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'ServerType' AND TABLE_SCHEMA = N'EDDSDBO') and OBJECT_ID('eddsdbo.DF_ServerType_CreatedOn', 'D') IS NULL
	ALTER TABLE [eddsdbo].[ServerType] ADD  CONSTRAINT [DF_ServerType_CreatedOn]  DEFAULT (getutcdate()) FOR [CreatedOn]
GO

/****** Object:  Default [DF_PerformanceSummary_CreatedOn]    Script Date: 10/11/2011 13:32:10 ******/
IF EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'PerformanceSummary' AND TABLE_SCHEMA = N'EDDSDBO') and OBJECT_ID('eddsdbo.DF_PerformanceSummary_CreatedOn', 'D') IS NULL
	ALTER TABLE [eddsdbo].[PerformanceSummary] ADD  CONSTRAINT [DF_PerformanceSummary_CreatedOn]  DEFAULT (getutcdate()) FOR [CreatedOn]
GO

/****** Object:  Default [DF_UserCountDW_CreatedOn]    Script Date: 10/11/2011 13:32:10 ******/
IF EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'UserCountDW' AND TABLE_SCHEMA = N'EDDSDBO') and OBJECT_ID('eddsdbo.DF_UserCountDW_CreatedOn', 'D') IS NULL
	ALTER TABLE [eddsdbo].[UserCountDW] ADD  CONSTRAINT [DF_UserCountDW_CreatedOn]  DEFAULT (getutcdate()) FOR [CreatedOn]
GO

/****** Object:  Default [DF_SQLServerSummary_CreatedOn]    Script Date: 10/11/2011 13:32:13 ******/
IF EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'SQLServerSummary' AND TABLE_SCHEMA = N'EDDSDBO') and OBJECT_ID('eddsdbo.DF_SQLServerSummary_CreatedOn', 'D') IS NULL
	ALTER TABLE [eddsdbo].[SQLServerSummary] ADD  CONSTRAINT [DF_SQLServerSummary_CreatedOn]  DEFAULT (getutcdate()) FOR [CreatedOn]
GO

/****** Object:  Default [DF_SQLServerDW_CreatedOn]    Script Date: 10/11/2011 13:32:13 ******/
IF EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'SQLServerDW' AND TABLE_SCHEMA = N'EDDSDBO') and OBJECT_ID('eddsdbo.DF_SQLServerDW_CreatedOn', 'D') IS NULL
	ALTER TABLE [eddsdbo].[SQLServerDW] ADD  CONSTRAINT [DF_SQLServerDW_CreatedOn]  DEFAULT (getutcdate()) FOR [CreatedOn]
GO

/****** Object:  Default [DF_ServerSummary_CreatedOn]    Script Date: 10/11/2011 13:32:13 ******/
IF EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'ServerSummary' AND TABLE_SCHEMA = N'EDDSDBO') and OBJECT_ID('eddsdbo.DF_ServerSummary_CreatedOn', 'D') IS NULL
	ALTER TABLE [eddsdbo].[ServerSummary] ADD  CONSTRAINT [DF_ServerSummary_CreatedOn]  DEFAULT (getutcdate()) FOR [CreatedOn]
GO

/****** Object:  Default [DF_ServerProcessorSummary_CreatedOn]    Script Date: 10/11/2011 13:32:13 ******/
IF EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'ServerProcessorSummary' AND TABLE_SCHEMA = N'EDDSDBO') and OBJECT_ID('eddsdbo.DF_ServerProcessorSummary_CreatedOn', 'D') IS NULL
	ALTER TABLE [eddsdbo].[ServerProcessorSummary] ADD  CONSTRAINT [DF_ServerProcessorSummary_CreatedOn]  DEFAULT (getutcdate()) FOR [CreatedOn]
GO

/****** Object:  Default [DF_ServerProcessorDW_CreatedOn]    Script Date: 10/11/2011 13:32:13 ******/
IF EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'ServerProcessorDW' AND TABLE_SCHEMA = N'EDDSDBO') and OBJECT_ID('eddsdbo.DF_ServerProcessorDW_CreatedOn', 'D') IS NULL
	ALTER TABLE [eddsdbo].[ServerProcessorDW] ADD  CONSTRAINT [DF_ServerProcessorDW_CreatedOn]  DEFAULT (getutcdate()) FOR [CreatedOn]
GO

/****** Object:  Default [DF_ServerDW_CreatedOn]    Script Date: 10/11/2011 13:32:13 ******/
IF EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'ServerDW' AND TABLE_SCHEMA = N'EDDSDBO') and OBJECT_ID('eddsdbo.DF_ServerDW_CreatedOn', 'D') IS NULL
	ALTER TABLE [eddsdbo].[ServerDW] ADD  CONSTRAINT [DF_ServerDW_CreatedOn]  DEFAULT (getutcdate()) FOR [CreatedOn]
GO

/****** Object:  Default [DF_ServerDiskSummary_CreatedOn]    Script Date: 10/11/2011 13:32:13 ******/
IF EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'ServerDiskSummary' AND TABLE_SCHEMA = N'EDDSDBO') and OBJECT_ID('eddsdbo.DF_ServerDiskSummary_CreatedOn', 'D') IS NULL
	ALTER TABLE [eddsdbo].[ServerDiskSummary] ADD  CONSTRAINT [DF_ServerDiskSummary_CreatedOn]  DEFAULT (getutcdate()) FOR [CreatedOn]
GO

/****** Object:  Default [DF_ServerDiskDW_CreatedOn]    Script Date: 10/11/2011 13:32:13 ******/
IF EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'ServerDiskDW' AND TABLE_SCHEMA = N'EDDSDBO') and OBJECT_ID('eddsdbo.DF_ServerDiskDW_CreatedOn', 'D') IS NULL
	ALTER TABLE [eddsdbo].[ServerDiskDW] ADD  CONSTRAINT [DF_ServerDiskDW_CreatedOn]  DEFAULT (getutcdate()) FOR [CreatedOn]
GO

/****** Object:  Default [DF_Measure_IsActive]    Script Date: 10/11/2011 13:32:15 ******/
IF EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'Measure' AND TABLE_SCHEMA = N'EDDSDBO') and OBJECT_ID('eddsdbo.DF_Measure_IsActive', 'D') IS NULL
	ALTER TABLE [eddsdbo].[Measure] ADD  CONSTRAINT [DF_Measure_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO

/****** Object:  Default [DF_Measure_IsDeleted]    Script Date: 10/11/2011 13:32:15 ******/
IF EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'Measure' AND TABLE_SCHEMA = N'EDDSDBO') and OBJECT_ID('eddsdbo.DF_Measure_IsDeleted', 'D') IS NULL
	ALTER TABLE [eddsdbo].[Measure] ADD  CONSTRAINT [DF_Measure_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO

/****** Object:  Default [DF_Measure_CreatedOn]    Script Date: 10/11/2011 13:32:15 ******/
IF EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'Measure' AND TABLE_SCHEMA = N'EDDSDBO') and OBJECT_ID('eddsdbo.DF_Measure_CreatedOn', 'D') IS NULL
	ALTER TABLE [eddsdbo].[Measure] ADD  CONSTRAINT [DF_Measure_CreatedOn]  DEFAULT (getutcdate()) FOR [CreatedOn]
GO

/****** Object:  Default [DF_Measure_Frequency]    Script Date: 10/11/2011 13:32:15 ******/
IF EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'Measure' AND TABLE_SCHEMA = N'EDDSDBO') and OBJECT_ID('eddsdbo.DF_Measure_Frequency', 'D') IS NULL
	ALTER TABLE [eddsdbo].[Measure] ADD  CONSTRAINT [DF_Measure_Frequency]  DEFAULT ((60)) FOR [Frequency]
GO

/****** Object:  ForeignKey [FK_SQLServerSummary_Server]    Script Date: 10/11/2011 13:32:13 ******/
IF EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'SQLServerSummary' AND TABLE_SCHEMA = N'EDDSDBO') and OBJECT_ID('eddsdbo.FK_SQLServerSummary_Server', 'F') IS NULL
BEGIN
ALTER TABLE [eddsdbo].[SQLServerSummary]  WITH CHECK ADD  CONSTRAINT [FK_SQLServerSummary_Server] FOREIGN KEY([ServerID])
	REFERENCES [eddsdbo].[Server] ([ServerID])

ALTER TABLE [eddsdbo].[SQLServerSummary] CHECK CONSTRAINT [FK_SQLServerSummary_Server]
END
GO

/****** Object:  ForeignKey [FK_SQLServerDW_Server]    Script Date: 10/11/2011 13:32:13 ******/
IF EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'SQLServerDW' AND TABLE_SCHEMA = N'EDDSDBO') and OBJECT_ID('eddsdbo.FK_SQLServerDW_Server', 'F') IS NULL
BEGIN
ALTER TABLE [eddsdbo].[SQLServerDW]  WITH CHECK ADD  CONSTRAINT [FK_SQLServerDW_Server] FOREIGN KEY([ServerID])
	REFERENCES [eddsdbo].[Server] ([ServerID])

ALTER TABLE [eddsdbo].[SQLServerDW] CHECK CONSTRAINT [FK_SQLServerDW_Server]
END
GO

/****** Object:  ForeignKey [FK_ServerSummary_Server]    Script Date: 10/11/2011 13:32:13 ******/
IF EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'ServerSummary' AND TABLE_SCHEMA = N'EDDSDBO') and OBJECT_ID('eddsdbo.FK_ServerSummary_Server', 'F') IS NULL
BEGIN
ALTER TABLE [eddsdbo].[ServerSummary]  WITH CHECK ADD  CONSTRAINT [FK_ServerSummary_Server] FOREIGN KEY([ServerID])
	REFERENCES [eddsdbo].[Server] ([ServerID])

ALTER TABLE [eddsdbo].[ServerSummary] CHECK CONSTRAINT [FK_ServerSummary_Server]
END
GO

/****** Object:  ForeignKey [FK_ServerProcessorSummary_Server]    Script Date: 10/11/2011 13:32:13 ******/
IF EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'ServerProcessorSummary' AND TABLE_SCHEMA = N'EDDSDBO') and OBJECT_ID('eddsdbo.FK_ServerProcessorSummary_Server', 'F') IS NULL
BEGIN
ALTER TABLE [eddsdbo].[ServerProcessorSummary]  WITH CHECK ADD  CONSTRAINT [FK_ServerProcessorSummary_Server] FOREIGN KEY([ServerID])
	REFERENCES [eddsdbo].[Server] ([ServerID])

ALTER TABLE [eddsdbo].[ServerProcessorSummary] CHECK CONSTRAINT [FK_ServerProcessorSummary_Server]
END
GO

/****** Object:  ForeignKey [FK_ServerProcessorDW_Server]    Script Date: 10/11/2011 13:32:13 ******/
IF EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'ServerProcessorDW' AND TABLE_SCHEMA = N'EDDSDBO') and OBJECT_ID('eddsdbo.FK_ServerProcessorDW_Server', 'F') IS NULL
BEGIN
ALTER TABLE [eddsdbo].[ServerProcessorDW]  WITH CHECK ADD  CONSTRAINT [FK_ServerProcessorDW_Server] FOREIGN KEY([ServerID])
	REFERENCES [eddsdbo].[Server] ([ServerID])

ALTER TABLE [eddsdbo].[ServerProcessorDW] CHECK CONSTRAINT [FK_ServerProcessorDW_Server]
END
GO

/****** Object:  ForeignKey [FK_ServerDW_Server]    Script Date: 10/11/2011 13:32:13 ******/
IF EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'ServerDW' AND TABLE_SCHEMA = N'EDDSDBO') and OBJECT_ID('eddsdbo.FK_ServerDW_Server', 'F') IS NULL
BEGIN
ALTER TABLE [eddsdbo].[ServerDW]  WITH CHECK ADD  CONSTRAINT [FK_ServerDW_Server] FOREIGN KEY([ServerID])
	REFERENCES [eddsdbo].[Server] ([ServerID])

ALTER TABLE [eddsdbo].[ServerDW] CHECK CONSTRAINT [FK_ServerDW_Server]
END
GO

/****** Object:  ForeignKey [FK_ServerDiskSummary_Server]    Script Date: 10/11/2011 13:32:13 ******/
IF EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'ServerDiskSummary' AND TABLE_SCHEMA = N'EDDSDBO') and OBJECT_ID('eddsdbo.FK_ServerDiskSummary_Server', 'F') IS NULL
BEGIN
ALTER TABLE [eddsdbo].[ServerDiskSummary]  WITH CHECK ADD  CONSTRAINT [FK_ServerDiskSummary_Server] FOREIGN KEY([ServerID])
	REFERENCES [eddsdbo].[Server] ([ServerID])

ALTER TABLE [eddsdbo].[ServerDiskSummary] CHECK CONSTRAINT [FK_ServerDiskSummary_Server]
END
GO

/****** Object:  ForeignKey [FK_ServerDiskDW_Server]    Script Date: 10/11/2011 13:32:13 ******/
IF EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'ServerDiskDW' AND TABLE_SCHEMA = N'EDDSDBO') and OBJECT_ID('eddsdbo.FK_ServerDiskDW_Server', 'F') IS NULL
BEGIN
ALTER TABLE [eddsdbo].[ServerDiskDW]  WITH CHECK ADD  CONSTRAINT [FK_ServerDiskDW_Server] FOREIGN KEY([ServerID])
	REFERENCES [eddsdbo].[Server] ([ServerID])

ALTER TABLE [eddsdbo].[ServerDiskDW] CHECK CONSTRAINT [FK_ServerDiskDW_Server]
END
GO

/****** Object:  ForeignKey [FK_Measure_MeasureType]    Script Date: 10/11/2011 13:32:15 ******/
IF EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'Measure' AND TABLE_SCHEMA = N'EDDSDBO') and OBJECT_ID('eddsdbo.FK_Measure_MeasureType', 'F') IS NULL
BEGIN
ALTER TABLE [eddsdbo].[Measure]  WITH CHECK ADD  CONSTRAINT [FK_Measure_MeasureType] FOREIGN KEY([MeasureTypeId])
	REFERENCES [eddsdbo].[MeasureType] ([MeasureTypeId])

ALTER TABLE [eddsdbo].[Measure] CHECK CONSTRAINT [FK_Measure_MeasureType]
END
GO