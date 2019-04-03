USE [EDDSPerformance]

IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'eddsdbo' AND TABLE_NAME = 'Events') 
	AND NOT EXISTS(SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[Events]') AND name = N'IX_Events_SourceTypeID_SourceID')
BEGIN
	
	-- When looking for specific events
	CREATE INDEX [IX_Events_SourceTypeID_SourceID] ON [EDDSPerformance].[eddsdbo].[Events] 
	([SourceTypeID], [SourceID])

END

IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'eddsdbo' AND TABLE_NAME = 'Events') 
	AND NOT EXISTS(SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[Events]') AND name = N'IX_Events_SourceTypeID_SourceID_StatusID')
BEGIN

	-- When looking for specific events in a specific status
	CREATE INDEX [IX_Events_SourceTypeID_SourceID_StatusID] ON [EDDSPerformance].[eddsdbo].[Events] 
	([SourceTypeID], [SourceID],[StatusID])

END


IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'eddsdbo' AND TABLE_NAME = 'CategoryScores') 
	AND NOT EXISTS(SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[CategoryScores]') AND name = N'IX_CategoryScores_CategoryID_ServerID')
BEGIN

	CREATE INDEX [IX_CategoryScores_CategoryID_ServerID] ON [EDDSPerformance].[eddsdbo].[CategoryScores] 
	([CategoryID], [ServerID])

END

IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'eddsdbo' AND TABLE_NAME = 'AgentHistory') 
	AND NOT EXISTS(SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[AgentHistory]') AND name = N'IX_AgentHistory_TimeStamp')
BEGIN

	CREATE INDEX [IX_AgentHistory_TimeStamp] ON [EDDSPerformance].[eddsdbo].[AgentHistory] 
	([TimeStamp]) 
	INCLUDE ([ID], [AgentArtifactId], [Successful])

END

IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'eddsdbo' AND TABLE_NAME = 'MetricData') 
	AND NOT EXISTS(SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[MetricData]') AND name = N'IX_MetricData_MetricID_ServerID')
BEGIN

	CREATE INDEX [IX_MetricData_MetricID_ServerID] ON [EDDSPerformance].[eddsdbo].[MetricData] 
	([MetricID], [ServerID]) 
	INCLUDE ([ID], [Data], [Score])
	
END

IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'eddsdbo' AND TABLE_NAME = 'CategoryScores') 
	AND NOT EXISTS(SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[CategoryScores]') AND name = N'IX_CategoryScores_CategoryID')
BEGIN
	-- Getting Category Score per hourID
	CREATE INDEX [IX_CategoryScores_CategoryID] ON [EDDSPerformance].[eddsdbo].[CategoryScores] 
	([CategoryID]) 
	INCLUDE ([ID], [ServerID], [Score])
END