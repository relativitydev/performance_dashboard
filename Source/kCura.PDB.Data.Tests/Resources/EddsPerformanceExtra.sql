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
			(42, 'Backup Gaps', null, 0),
			(43, 'DBCC Gaps', null, 0),
			(44, 'RPO', null, 0),
			(45, 'RTO', null, 0),
			(46, 'Backup Frequency', null, 0),
			(47, 'DBCC Frequency', null, 0),
			(48, 'Backup Coverage', null, 0),
			(49, 'DBCC Coverage', null, 0)
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
			(42, 3),
			(43, 3),
			(44, 3),
			(45, 3),
			(46, 3),
			(47, 3),
			(48, 3),
			(49, 3)
END

if exists (select * from eddsdbo.MetricTypesToCategoryTypes where MetricTypeID in (10, 11))
begin
	delete from eddsdbo.MetricTypesToCategoryTypes
	where MetricTypeID in (10, 11)
End

if exists (select * from eddsdbo.MetricTypes where id in (10, 11))
begin
	delete from eddsdbo.MetricTypes
	where id in (10, 11)
end

if not exists (select * from eddsdbo.MetricTypes where id in (12))
begin
	insert into eddsdbo.MetricTypes
	([ID], [Name], [SampleType])
	values
	(12, 'AuditAnalysis', 0)
end

if not exists (select * from eddsdbo.MetricTypesToCategoryTypes where MetricTypeID in (12))
begin
	insert into eddsdbo.MetricTypesToCategoryTypes
	(MetricTypeID, CategoryTypeID)
	values
	(12, 1)
end


IF not exists(select top 1 * from [eddsdbo].[EventSourceSystemControl])
begin
	INSERT INTO [eddsdbo].[EventSourceSystemControl]
           ([State])
     VALUES
           (1)
END

-- Add localhost to sys.servers
IF not exists(select top 1 * from sys.servers where name = 'localhost')
begin
	EXEC sp_addlinkedserver @server='localhost'
end
GO

-- Add uptimeDetails view
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.VIEWS WHERE TABLE_SCHEMA = 'eddsdbo' AND TABLE_NAME = 'UptimeDetail')
	DROP VIEW eddsdbo.UptimeDetail;
GO

CREATE VIEW eddsdbo.UptimeDetail AS
SELECT
	SummaryDayHour,
	CAST(CASE
		WHEN (100.0 - ISNULL(HoursDown, 0) * 100.0) >= 99.95 THEN 100 --To get a perfect score for the week, you really need 100% uptime
		WHEN (100.0 - HoursDown * 100.0) < 90 THEN 0 --17 hours of downtime results in max points lost
		ELSE ((100.0 - HoursDown * 100.0) - 90.0) * 100.0 / 9.95
	END AS INT) Score,
	CASE WHEN HoursDown = 0 THEN 'Accessible'
		WHEN IsWebDownTime = 1 THEN 'All Web Servers Down'
		ELSE 'SQL/Agent Servers Down'
	END [Status],
	100.0 - HoursDown * 100.0 AS Uptime,
	AffectedByMaintenanceWindow
FROM eddsdbo.QoS_UptimeRatings WITH(NOLOCK)
WHERE SummaryDayHour > DATEADD(dd, -90, getutcdate())