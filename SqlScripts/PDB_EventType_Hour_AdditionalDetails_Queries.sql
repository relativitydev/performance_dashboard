---- Make queries that can find the hour ID from a specific event

DECLARE @backfillLimit DATETIME = DATEADD(d, -7, getutcdate());
DECLARE @eventLifetimeLimit DATETIME = DATEADD(d, -90, GETUTCDATE());

--- Obsolete events
-- 1, 2, 5, 6, 8
DECLARE @sourceTypeIds TABLE (id INT NOT null);
INSERT @sourceTypeIds(id) VALUES(1), (2), (5), (6), (8);
SELECT e.ID AS ObsoleteEventId, et.Name, e.TimeStamp, e.LastUpdated, e.Delay, e.ExecutionTime, e.Retries, e.PreviousEventID
FROM EDDSPerformance.eddsdbo.Events e
LEFT JOIN EDDSPerformance.eddsdbo.EventTypes et ON e.SourceTypeID = et.Id
WHERE e.SourceTypeID IN (SELECT id FROM @sourceTypeIds)
--AND e.TimeStamp < @backfillLimit -- Past backfill limit
AND e.StatusID = 4 -- Errored

--- Polling events (no sourceId)
-- 100	CreateNextHour
-- 7000	CheckForHourPrerequisites
-- 304	FindNextCategoriesToScore

--- No sourceID events
-- 7003	CheckAllPrerequisitesComplete
-- 2000	SendScoreAlerts
--DECLARE @sourceTypeIds TABLE (id INT NOT null);
DELETE FROM @sourceTypeIds
INSERT @sourceTypeIds(id) VALUES(100), (7000), (304), (2000), (7003);
SELECT e.ID AS PollingEventId, et.Name, e.TimeStamp, e.LastUpdated, e.Delay, e.ExecutionTime, e.Retries, e.PreviousEventID -- Relevant Event data
FROM EDDSPerformance.eddsdbo.Events e
LEFT JOIN EDDSPerformance.eddsdbo.EventTypes et ON e.SourceTypeID = et.Id
WHERE e.SourceTypeID IN (SELECT id FROM @sourceTypeIds)
--AND e.TimeStamp < @backfillLimit -- Past backfill limit
AND e.StatusID = 4 -- Errored


--- Hour Events
-- 9	Score Hour
-- 101	CheckIfHourReadyToScore
-- 102	HourCleanup
-- 200	CreateMetricsForHour
-- 300	CreateCategoriesForHour
--DECLARE @sourceTypeIds TABLE (id INT NOT null);
DELETE FROM @sourceTypeIds
INSERT @sourceTypeIds(id) VALUES(9), (101), (102), (200), (300);
SELECT e.id AS HourEventId, et.Name, e.TimeStamp, e.LastUpdated, e.Delay, e.ExecutionTime, e.Retries, e.PreviousEventID -- Relevant Event data
,h.ID AS HourId, h.HourTimeStamp, h.Score AS HourScore -- Hour specifics
FROM EDDSPerformance.eddsdbo.Events e
LEFT JOIN EDDSPerformance.eddsdbo.EventTypes et ON et.Id = e.SourceTypeID
LEFT JOIN EDDSPerformance.eddsdbo.Hours h ON e.SourceTypeID IN (SELECT id from @sourceTypeIds) AND e.SourceID = h.Id
WHERE e.SourceTypeID in (SELECT id FROM @sourceTypeIds)
--AND e.TimeStamp < @backfillLimit -- Past backfill limit
AND e.StatusID = 4 -- Errored

--- ServerCleanup table (contains hour/server pair and cleanup status)
-- 103 ServerCleanup 
DECLARE @sourceTypeId INT = 103
SELECT e.id AS ServerCleanupEventId, 
et.Name, e.TimeStamp, e.LastUpdated, e.Delay, e.ExecutionTime, e.Retries, e.PreviousEventID -- Relevant Event data
,h.ID AS HourId, h.HourTimeStamp, h.Score AS HourScore -- Hour specifics
,sc.Id AS ServerCleanupId, sc.ServerId, sc.Success
FROM EDDSPerformance.eddsdbo.Events e
LEFT JOIN EDDSPerformance.eddsdbo.EventTypes et ON et.Id = e.SourceTypeID
LEFT JOIN EDDSPerformance.eddsdbo.ServerCleanups sc ON e.SourceTypeID = @sourceTypeId AND e.SourceID = sc.Id
INNER JOIN EDDSPerformance.eddsdbo.Hours h ON sc.HourId = h.ID
WHERE e.SourceTypeID = @sourceTypeId
--AND e.TimeStamp < @backfillLimit -- Past backfill limit
AND e.StatusID = 4 -- Errored

--- MetricData events
-- 202	CheckMetricDataIsReadyForDataCollection
-- 3	CollectMetricData
-- 1001	CreateAuditProcessingBatches
-- 4	ScoreMetricData
-- 203	CheckSamplingPeriodForMetricData
-- 204	StartPrerequisitesForMetricData
--DECLARE @sourceTypeIds TABLE (id INT NOT null);
DELETE FROM @sourceTypeIds
INSERT @sourceTypeIds(id) VALUES(3), (4), (202), (203), (204), (1001);
SELECT e.id AS MetricDataEventId, 
et.Name, e.TimeStamp, e.LastUpdated, e.Delay, e.ExecutionTime, e.Retries, e.PreviousEventID -- Relevant Event data
,h.ID AS HourId, h.HourTimeStamp, h.Score AS HourScore -- Hour specifics
,m.ID AS MetricId, m.MetricTypeID, mt.Name AS MetricTypeName -- Metric/MetricType info
,md.ID AS MetricDataId, md.ServerID, md.Data, md.Score AS MetricDataScore -- MetricData info
FROM EDDSPerformance.eddsdbo.Events e
LEFT JOIN EDDSPerformance.eddsdbo.EventTypes et ON et.Id = e.SourceTypeID
LEFT JOIN EDDSPerformance.eddsdbo.MetricData md ON e.SourceTypeID IN (SELECT id from @sourceTypeIds) AND e.SourceID = md.Id
INNER JOIN EDDSPerformance.eddsdbo.Metrics m ON md.MetricID = m.ID
INNER JOIN EDDSPerformance.eddsdbo.MetricTypes mt ON m.MetricTypeID = mt.ID
INNER JOIN EDDSPerformance.eddsdbo.Hours h ON m.HourID = h.ID
WHERE e.SourceTypeID in (SELECT id FROM @sourceTypeIds)
--AND e.TimeStamp < @backfillLimit -- Past backfill limit
AND e.StatusID = 4 -- Errored

--- AuditBatchProcessing
-- 1011	ProcessAuditBatches
--DECLARE @sourceTypeId INT = 1011
SET @sourceTypeId = 1011
SELECT e.id AS ProcessAuditBatchId,
et.Name, e.TimeStamp, e.LastUpdated, e.Delay, e.ExecutionTime, e.Retries, e.PreviousEventID -- Relevant Event data
,h.ID AS HourId, h.HourTimeStamp, h.Score AS HourScore -- Hour specifics
,hsab.BatchesCreated, hsab.BatchesCompleted -- Hour batch info
,sab.Id AS BatchId, sab.ServerId, sab.BatchStart, sab.BatchSize -- Created Batch info
,sabr.UserId, sabr.TotalExecutionTime, sabr.TotalQueries, sabr.TotalComplexQueries, sabr.TotalLongRunningQueries, sabr.TotalSimpleLongRunningQueries -- Batch result info
FROM EDDSPerformance.eddsdbo.Events e
LEFT JOIN EDDSPerformance.eddsdbo.EventTypes et ON et.Id = e.SourceTypeID
LEFT JOIN EDDSPerformance.eddsdbo.SearchAuditBatch sab ON e.SourceTypeID = @sourceTypeId AND e.SourceID = sab.Id
LEFT JOIN EDDSPerformance.eddsdbo.SearchAuditBatchResult sabr ON sabr.BatchId = sab.Id
INNER JOIN EDDSPerformance.eddsdbo.Hours h ON sab.HourId = h.ID
LEFT JOIN EDDSPerformance.eddsdbo.HourSearchAuditBatches hsab ON hsab.HourId = sab.HourId
WHERE e.SourceTypeID = @sourceTypeId
--AND e.TimeStamp < @backfillLimit -- Past backfill limit
AND e.StatusID = 4 -- Errored

--- Category events
-- 301	CreateCategoryScoresForCategory
--DECLARE @sourceTypeId INT = 301
SET @sourceTypeId = 301
SELECT e.id AS CategoryEventId, 
et.Name, e.TimeStamp, e.LastUpdated, e.Delay, e.ExecutionTime, e.Retries, e.PreviousEventID -- Relevant Event data
,h.ID AS HourId, h.HourTimeStamp, h.Score AS HourScore -- Hour specifics
,c.ID AS CategoryId, c.CategoryTypeID, ct.Name -- Category/Type info
FROM EDDSPerformance.eddsdbo.Events e
LEFT JOIN EDDSPerformance.eddsdbo.EventTypes et ON et.Id = e.SourceTypeID
LEFT JOIN EDDSPerformance.eddsdbo.Categories c ON e.SourceTypeID = @sourceTypeId AND e.SourceID = c.Id
INNER JOIN EDDSPerformance.eddsdbo.CategoryTypes ct ON c.CategoryTypeID = ct.ID
INNER JOIN EDDSPerformance.eddsdbo.Hours h ON c.HourID = h.ID
WHERE e.SourceTypeID = @sourceTypeId
--AND e.TimeStamp < @backfillLimit -- Past backfill limit
AND e.StatusID = 4 -- Errored

--- Metric events
-- 201	CreateMetricDatasForMetric
--DECLARE @sourceTypeId INT = 201
SET @sourceTypeId = 201
SELECT e.id AS MetricEventId,
et.Name, e.TimeStamp, e.LastUpdated, e.Delay, e.ExecutionTime, e.Retries, e.PreviousEventID -- Relevant Event data
,h.ID AS HourId, h.HourTimeStamp, h.Score AS HourScore -- Hour specifics
,m.ID AS MetricId, m.MetricTypeID, mt.Name AS MetricTypeName -- Metric/MetricType info
FROM EDDSPerformance.eddsdbo.Events e
LEFT JOIN EDDSPerformance.eddsdbo.EventTypes et ON et.Id = e.SourceTypeID
LEFT JOIN EDDSPerformance.eddsdbo.Metrics m ON e.SourceTypeID = @sourceTypeId AND e.SourceID = m.Id
INNER JOIN EDDSPerformance.eddsdbo.MetricTypes mt ON m.MetricTypeID = mt.ID
INNER JOIN EDDSPerformance.eddsdbo.Hours h ON m.HourID = h.ID
WHERE e.SourceTypeID = @sourceTypeId
--AND e.TimeStamp < @backfillLimit -- Past backfill limit
AND e.StatusID = 4 -- Errored

--- Server events (No direct Hour relation)
-- 7002	DeployServerDatabases
--DECLARE @sourceTypeId INT = 7002
SET @sourceTypeId = 7002
SELECT e.id AS ServerEventId,
et.Name, e.TimeStamp, e.LastUpdated, e.Delay, e.ExecutionTime, e.Retries, e.PreviousEventID -- Relevant Event data
,s.*
FROM EDDSPerformance.eddsdbo.Events e
LEFT JOIN EDDSPerformance.eddsdbo.EventTypes et ON et.Id = e.SourceTypeID
LEFT JOIN EDDSPerformance.eddsdbo.[Server] s ON e.SourceTypeID = @sourceTypeId AND e.SourceID = s.ServerID
WHERE e.SourceTypeID = @sourceTypeId
AND e.TimeStamp < @backfillLimit -- Past backfill limit
AND e.StatusID = 4 -- Errored

--- CategoryScore events
-- 7	ScoreCategoryScore
--DECLARE @sourceTypeId INT = 7
SET @sourceTypeId = 7
SELECT e.id AS CategoryScoreEventId, 
et.Name, e.TimeStamp, e.LastUpdated, e.Delay, e.ExecutionTime, e.Retries, e.PreviousEventID -- Relevant Event data
,h.ID AS HourId, h.HourTimeStamp, h.Score AS HourScore -- Hour specifics
,c.ID AS CategoryId, c.CategoryTypeID, ct.Name -- Category/Type info
,cs.ID AS CategoryScoreId, cs.ServerID, cs.CategoryID AS CategoryId2, cs.Score AS CategoryScore -- CategoryScore info
FROM EDDSPerformance.eddsdbo.Events e
LEFT JOIN EDDSPerformance.eddsdbo.EventTypes et ON et.Id = e.SourceTypeID
LEFT JOIN EDDSPerformance.eddsdbo.CategoryScores cs ON e.SourceTypeID = @sourceTypeId AND e.SourceID = cs.Id
INNER JOIN EDDSPerformance.eddsdbo.Categories c ON c.ID = cs.CategoryID
LEFT JOIN EDDSPerformance.eddsdbo.CategoryTypes ct ON c.CategoryTypeID = ct.ID
INNER JOIN EDDSPerformance.eddsdbo.Hours h ON c.HourID = h.ID
WHERE e.SourceTypeID = @sourceTypeId
--AND e.TimeStamp < @backfillLimit -- Past backfill limit
AND e.StatusID = 4 -- Errored