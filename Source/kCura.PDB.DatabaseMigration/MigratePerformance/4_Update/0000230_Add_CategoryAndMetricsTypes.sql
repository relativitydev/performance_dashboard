USE [EDDSPerformance]

IF not exists(select top 1 * from [eddsdbo].[MetricTypes])
BEGIN
	INSERT INTO [eddsdbo].[MetricTypes]
           ([ID]
           ,[Name]
           ,[Description]
           ,[SampleType])
     VALUES
			-- Uptime
			(1, 'Agent Uptime', null, 1),
			(2, 'Web Uptime', null, 1),
			-- User Experience
			(10, 'Long Running Querries', null, 0),
			(11, 'Conversion Speed', null, 0),
			-- Infrastructure Performance
			(20, 'Ram', null, 0),
			(21, 'Cpu', null, 0),
			(22, 'Number of Agents Per Server', null, 0),
			(23, 'Sql Server Waits', null, 0),
			(24, 'Sql Server Page Outs', null, 0),
			(25, 'Sql Server Latency For Data File', null, 0),
			(26, 'Sql Server Latency For Log File', null, 0),
			(27, 'Sql Server Virtual Log File Count', null, 0),
			-- Recoverability & Integrity
			(40, 'Backups', null, 0),
			(41, 'Dbcc Checks', null, 0)
END

IF not exists(select top 1 * from [eddsdbo].[CategoryTypes])
BEGIN
	INSERT INTO [eddsdbo].[CategoryTypes]
           ([ID]
           ,[Name]
           ,[Description])
     VALUES
           (1, 'User Experience', null),
		   (2, 'Infrastructure Performance', null),
		   (3, 'Recoverability & Integrity', null),
		   (4, 'Uptime', null)
END

IF not exists(select top 1 * from [eddsdbo].[MetricTypesToCategoryTypes])
BEGIN
	INSERT INTO [eddsdbo].[MetricTypesToCategoryTypes]
           ([MetricTypeID]
           ,[CategoryTypeID])
     VALUES
			-- Uptime
			(1, 4),
			(2, 4),
			-- User Experience
			(10, 1),
			(11, 1),
			-- Infrastructure Performance
			(20, 2),
			(21, 2),
			(22, 2),
			(23, 2),
			(24, 2),
			(25, 2),
			(26, 2),
			(27, 2),
			-- Recoverability & Integrity
			(40, 3),
			(41, 3)
END