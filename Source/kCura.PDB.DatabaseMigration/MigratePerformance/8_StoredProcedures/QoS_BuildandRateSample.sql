USE EDDSPerformance
Go

IF EXISTS (select 1 from sysobjects where [name] = 'QoS_BuildAndRateSample' and type = 'P')  
BEGIN
	DROP PROCEDURE EDDSDBO.QoS_BuildAndRateSample
END

GO

CREATE PROCEDURE EDDSDBO.QoS_BuildAndRateSample
	@hourId int,
	@weekIntegrityScore decimal(5,2),
	@logging bit
AS
BEGIN

DECLARE @summaryDayHour datetime,
	@upTimePCT DECIMAL (7, 3), 
	@earliestUptimeChecked datetime,
	@lastRatedHour datetime,
	@fcmValid bit,
	@updatedHour datetime,
	@loggingVars varchar(max),
	@lastCompletedTask varchar(max),
	@runStart datetime = getutcdate(),
	@tmpHour datetime,
	@upgradeWeeklyGracePeriod int = 0,
	@eddsServerId INT = (
		SELECT TOP 1 ArtifactID
		FROM EDDS.eddsdbo.ExtendedResourceServer WITH(NOLOCK)
		WHERE [Type] = 'SQL - Primary'
	),
	@activityThreshold DECIMAL(10,2) = (
		select isnull(
			(select convert(decimal(10,2), [value]) from eddsdbo.Configuration where Section = 'kCura.PDB' and Name = 'ActivityThreshold')
			, 0.75
		)
	),
	@batchSize int = 10000,
	@SQL varchar(max);

select @summaryDayHour = HourTimeStamp
from eddsdbo.[Hours] as h
where h.ID = @hourId
	
DECLARE @isEddsStandalone BIT = CASE
		WHEN EXISTS (
			SELECT TOP 1 1
			FROM EDDS.eddsdbo.[Case] WITH(NOLOCK)
			WHERE ServerID = @eddsServerId
		) THEN 0
		ELSE 1
	END;

SET DATEFIRST 2
SET XACT_ABORT ON

IF @logging = 1
BEGIN
	SET @loggingVars = '@summaryDayHour = ' + CAST(@summaryDayHour as varchar) + ', @isEddsStandalone = ' + CAST(@isEddsStandalone as varchar) + ', @eddsServerId = ' + CAST(@eddsServerId as varchar);
	EXEC EDDSPerformance.eddsdbo.QoS_LogAppend
		@module = 'BuildandRateSample',
		@taskCompleted = 'Finished initialization steps for scoring',
		@otherVars = @loggingVars,
		@nextTask = 'Calling HasSystemVersionChanged'
END	

-- Transaction used to start here
set @lastCompletedTask = 'Starting Transaction and Cleaning out old records.'
--Keep track of Relativity upgrades and determine if the version has changed
exec [eddsdbo].[QoS_HasSystemVersionChanged] @summaryDayHour = @summaryDayHour, @upgradeWeeklyGracePeriod = @upgradeWeeklyGracePeriod output

--clean out old records
DELETE sh
FROM EDDSDBO.QoS_SampleHistoryUX sh
INNER JOIN EDDSDBO.[Hours] h ON sh.HourId = h.Id
WHERE h.HourTimeStamp < DATEADD(dd, -180, getUTCdate()) and h.Status != 4

DELETE FROM EDDSDBO.QoS_SampleHistory WHERE summaryDayHour < DATEADD(dd, -180, getUTCdate())
DELETE FROM EDDSDBO.QoS_SystemLoadSummary WHERE SummaryDayHour < DATEADD(dd, -180, getUTCdate())

IF @logging = 1
BEGIN
	EXEC EDDSPerformance.eddsdbo.QoS_LogAppend
		@module = 'BuildandRateSample',
		@taskCompleted = 'Cleaned out old records',
		@nextTask = 'Gather system load summary data'
END
set @lastCompletedTask = 'Cleaned out old records.'
--gather system load data (needs to happen before the sample is generated as poison waits affect sample eligibility)
INSERT INTO EDDSDBO.QoS_SystemLoadSummary
	(ServerArtifactID, ServerTypeID, AvgRAMPagesePerSec, AvgCPUpct, AvgRAMpct, AvgRAMAvailKB, MemorySignalStateRatio,
	 Pageouts, PoisonWaits, QoSHourID, SummaryDayHour)
SELECT
	x.ArtifactID,
	x.ServerTypeID,
	x.RAMPagesPerSec,
	x.PCore,
	AVG(ISNULL(x.RAMPct, 0)) OVER ( PARTITION BY x.MeasureDate, x.ServerID ) AvgRAMpct,
	AvailableMemory,
	x.LowMemRatio,
	x.Pageouts,
	0,
	eddsdbo.QoS_GetServerHourID(x.ArtifactID, x.MeasureDate),
	x.MeasureDate
FROM (
	SELECT DISTINCT
		ss.MeasureDate ,
		ss.ServerID ,
		ca.PCore ,
		s.ArtifactID ,
		s.ServerTypeID ,
		ss.RAMPagesPerSec ,
		ss.RAMPct ,
		ss.AvailableMemory,
		sss.LowMemRatio,
		ssp.Pageouts
	FROM eddsdbo.ServerSummary ss
	INNER JOIN eddsdbo.[Server] s
		ON ss.ServerID = s.ServerID
	INNER JOIN EDDS.eddsdbo.ResourceServer rs
		ON s.ArtifactID = rs.ArtifactID
	CROSS APPLY (
		SELECT
			AVG(ISNULL(CPUProcessorTimePct, 0)) OVER ( PARTITION BY MeasureDate, ServerID ) PCore
		FROM [eddsdbo].[ServerProcessorSummary] sps
		WHERE ss.MeasureDate = sps.MeasureDate
			AND ss.ServerID = sps.ServerID
	) AS ca
	OUTER APPLY (
		SELECT
			AVG(ISNULL(LowMemorySignalStateRatio, 0)) OVER (PARTITION BY MeasureDate, ServerID) LowMemRatio
		FROM eddsdbo.SQLServerSummary
		WHERE MeasureDate = ss.MeasureDate
			AND ServerID = ss.ServerID
	) AS sss
	OUTER APPLY (
		SELECT
			AVG(ISNULL(Pageouts, 0)) OVER (PARTITION BY SummaryDayHour, ServerID) Pageouts
		FROM eddsdbo.SQLServerPageouts
		WHERE SummaryDayHour = ss.MeasureDate
			AND ServerID = ss.ServerID
	) AS ssp
	WHERE s.ServerTypeID IN ( 1, 3 )
		--AND (s.IgnoreServer IS NULL OR s.IgnoreServer = 0)
		AND s.DeletedOn IS NULL
		AND s.ArtifactID IS NOT NULL
		AND ss.MeasureDate = @summaryDayHour
) AS x

UPDATE sls
SET sls.SignalWaitsRatio = ws.SignalWaitsRatio
FROM eddsdbo.QoS_SystemLoadSummary sls
INNER JOIN eddsdbo.QoS_WaitSummary ws WITH(NOLOCK)
ON sls.SummaryDayHour = ws.SummaryDayHour
	AND sls.ServerArtifactID = ws.ServerArtifactID
WHERE sls.SummaryDayHour = @summaryDayHour
	AND RowHash IS NULL
	and ws.PercentOfCPUThreshold >= @activityThreshold

UPDATE eddsdbo.QoS_SystemLoadSummary
SET PoisonWaits = (
	SELECT COUNT(*)
	FROM eddsdbo.QoS_WaitSummary ws WITH(NOLOCK)
	INNER JOIN eddsdbo.QoS_WaitDetail wd WITH(NOLOCK)
	ON ws.WaitSummaryID = wd.WaitSummaryID
	INNER JOIN eddsdbo.QoS_Waits w WITH(NOLOCK)
	ON wd.WaitTypeID = w.WaitTypeID
	WHERE ws.SummaryDayHour = QoS_SystemLoadSummary.SummaryDayHour
		AND ws.ServerArtifactID = QoS_SystemLoadSummary.ServerArtifactID
		AND w.IsPoisonWait = 1
		AND wd.DifferentialWaitMs > 1000
)
WHERE SummaryDayHour = @summaryDayHour
	AND RowHash IS NULL

UPDATE sls
SET sls.MaxVirtualLogFiles = vlf.VirtualLogFiles
FROM eddsdbo.QoS_SystemLoadSummary sls
INNER JOIN eddsdbo.VirtualLogFileSummary vlf WITH(NOLOCK)
ON sls.SummaryDayHour = vlf.SummaryDayHour
	AND sls.ServerArtifactID = vlf.ServerArtifactID
WHERE sls.SummaryDayHour = @summaryDayHour
	AND RowHash IS NULL

UPDATE sls
SET sls.HighestLatencyDatabase = fls.HighestLatencyDatabase,
	sls.IOWaitsHigh = fls.IOWaitsHigh,
	sls.ReadLatencyMs = fls.ReadLatencyMs,
	sls.WriteLatencyMs = fls.WriteLatencyMs,
	sls.IsDataFile = fls.IsDataFile,
	sls.LatencyScore = fls.LatencyScore
FROM eddsdbo.QoS_SystemLoadSummary sls
INNER JOIN eddsdbo.QoS_FileLatencySummary fls WITH(NOLOCK)
ON sls.SummaryDayHour = fls.SummaryDayHour
	AND sls.ServerArtifactID = fls.ServerArtifactID
WHERE sls.SummaryDayHour = @summaryDayHour
	AND RowHash IS NULL

--Calculate scores for the new hour based on metric columns
UPDATE eddsdbo.QoS_SystemLoadSummary
SET
	CPUScore =
		CASE
			WHEN PoisonWaits > 0 THEN 0
			WHEN ISNULL(AvgCPUPct, 0) < 60 THEN 100
			WHEN AvgCPUPct > 85 THEN 0
			ELSE (85.0 - CAST(AvgCPUpct AS DECIMAL(5,2)))*100/25
		END,
	RAMPagesScore =
		CASE
			WHEN PoisonWaits > 0 THEN 0
			WHEN AvgRAMPagesePerSec > 150 THEN 0
			ELSE (150 - CAST(ISNULL(AvgRAMPagesePerSec, 0) AS DECIMAL(5,2)))*100/150
		END,
	RAMPctScore =
		CASE
			WHEN PoisonWaits > 0 THEN 0
			WHEN AvgRAMAvailKB IS NULL THEN 100
			WHEN ServerTypeID = 1 THEN
				CASE
					WHEN AvgRAMAvailKB >= 1048576 THEN 100 -- Web servers should have 1 GB free
					ELSE (ROUND((LOG10(AvgRAMAvailKB + 1)/LOG10(1048576) * 100),0) + (ROUND((AvgRAMAvailKB / 1048576 * 100),0) )) / 2 --If less than 1 GB is free, deduct points
				END
			ELSE
				CASE
					WHEN AvgRAMAvailKB >= 4194304 THEN 100 -- SQL servers should have 4 GB free
					ELSE (ROUND((LOG10(AvgRAMAvailKB + 1)/LOG10(4194304) * 100),0) + (ROUND((AvgRAMAvailKB / 4194304 * 100),0) )) / 2 --If less than 4 GB is free, deduct points
				END
		END,
	MemorySignalStateScore =
		CASE
			WHEN PoisonWaits > 0 THEN 0
			WHEN ISNULL(MemorySignalStateRatio, 0) <= 0 THEN 100
			WHEN Pageouts > 0 THEN 0 --If the low memory signal has started to appear (> 0%) and we're getting pageouts, tank the score
			WHEN MemorySignalStateRatio > 0.80 THEN 0 --If the low memory signal appears more than 80% of the time, tank the score
			ELSE (0.8 - MemorySignalStateRatio) * 100.0 / 0.8 --Otherwise, deduct points on a linear scale according to the low memory signal ratio
		END,
	WaitsScore =
		CASE
			WHEN PoisonWaits > 0 THEN 0
			WHEN ISNULL(SignalWaitsRatio, 0) <= 0.1 THEN 100
			WHEN SignalWaitsRatio > 0.25 THEN 0
			ELSE (0.25 - SignalWaitsRatio) * 100.0 / 0.15
		END,
	VLFScore =
		CASE
			WHEN PoisonWaits > 0 THEN 0
			WHEN ISNULL(MaxVirtualLogFiles, 0) < 10000 THEN 100
			ELSE 0 --This score is all or nothing!
		END,
	LatencyScore =
		CASE
			WHEN PoisonWaits > 0 THEN 0
			ELSE LatencyScore --This score just gets passed through unless poison waits exceeded the threshold
		END
WHERE SummaryDayHour = @summaryDayHour
	AND RowHash IS NULL

set @lastCompletedTask = 'Gathered system load summary data.'
IF @logging = 1
BEGIN
	EXEC EDDSPerformance.eddsdbo.QoS_LogAppend
		@module = 'BuildandRateSample',
		@taskCompleted = 'Gathered system load summary data',
		@nextTask = 'Initialize ratings for the current hour'
END

--Seed the ratings table with this hour
INSERT INTO EDDSDBO.QoS_Ratings (ServerArtifactID,SummaryDayHour,QoSHourID)
SELECT DISTINCT ArtifactID AS ServerArtifactID,
	@summaryDayHour as SummaryDayHour,
	eddsdbo.QoS_GetServerHourID(ArtifactID, @summaryDayHour) AS QoSHourID
FROM eddsdbo.[Server] WITH(NOLOCK)
WHERE ServerTypeID = 3 -- Databases only
	AND DeletedOn IS NULL
	AND (IgnoreServer = 0 OR IgnoreServer IS NULL)

IF (@isEddsStandalone = 1)
BEGIN
	INSERT INTO eddsdbo.QoS_Ratings (ServerArtifactID, SummaryDayHour, QoSHourID)
	SELECT @eddsServerId, @summaryDayHour, eddsdbo.QoS_GetServerHourID(@eddsServerId, @summaryDayHour)
END

set @lastCompletedTask = 'Initialized ratings for the current hour.'
IF @logging = 1
BEGIN
	EXEC EDDSPerformance.eddsdbo.QoS_LogAppend
		@module = 'BuildandRateSample',
		@taskCompleted = 'Initialized ratings for the current hour',
		@nextTask = 'Calculating weekly user experience scores'
END

--User Experience Weekly Scores --Updated to use UserExperienceRatings table
--set the top 4 simple long running query score (4SLRQ) to the average score of the SIMPLE score when concurrency in the system is high.
;with WeekConcurrencyScore
as
(
	select s.ArtifactID ServerArtifactID, avg(isnull(uxr.ConcurrencyUXScore, 100)) Score
	from eddsdbo.QoS_UserExperienceRatings uxr
	inner join eddsdbo.QoS_SampleHistoryUX sh on uxr.HourId = sh.HourId
	inner join eddsdbo.[Server] s on s.ArtifactId = uxr.ServerArtifactId and s.ServerID = sh.ServerId
	where IsActiveConcurrencySample = 1
	group by s.ArtifactID
)
UPDATE qr 
SET WeekUserExperience4SLRQScore = ISNULL(uxr.Score, 100) 
FROM EDDSDBO.QoS_Ratings qr
left JOIN WeekConcurrencyScore uxr ON uxr.ServerArtifactID = qr.ServerArtifactID
WHERE qr.SummaryDayHour = @summaryDayHour

;with WeekArrivalRateScore
as
(
	select s.ArtifactID ServerArtifactID, avg(isnull(uxr.ArrivalRateUXScore, 100)) Score
	from eddsdbo.QoS_UserExperienceRatings uxr
	inner join eddsdbo.QoS_SampleHistoryUX sh on sh.HourId = uxr.HourId
	inner join eddsdbo.[Server] s on s.ArtifactId = uxr.ServerArtifactId and s.ServerID = sh.ServerId
	where IsActiveArrivalRateSample = 1
	group by s.ArtifactID
)
UPDATE qr 
SET WeekUserExperienceSLRQScore = ISNULL(uxr.Score, 100)
FROM EDDSDBO.QoS_Ratings qr
left JOIN WeekArrivalRateScore uxr ON uxr.ServerArtifactID = qr.ServerArtifactID
WHERE qr.SummaryDayHour = @summaryDayHour

set @lastCompletedTask = 'Calculated weekly user experience scores.'
IF @logging = 1
BEGIN
	EXEC EDDSPerformance.eddsdbo.QoS_LogAppend
		@module = 'BuildandRateSample',
		@taskCompleted = 'Calculated weekly user experience scores.',
		@nextTask = 'Calculating quarterly user experience scores'
END

-- this grabs the summarydayhour for midnight at all past trust weeks for the current quarter
;WITH scorableWeeks AS
(
	-- most recent trust date
	SELECT DATEADD(dd, 1-(DATEPART(dw, @summaryDayHour)), DATEDIFF(dd, 0, @summaryDayHour)) AS SummaryDayHour
	UNION ALL (
		-- recursively 1 week prior
		SELECT DATEADD(wk, -1, SummaryDayHour)
		FROM scorableWeeks
		WHERE DATEADD(wk, -1, SummaryDayHour) >= DATEADD(dd, -90, @summaryDayHour)
	)
)
SELECT SummaryDayHour
INTO #scorableWeeks
FROM scorableWeeks
OPTION (MAXRECURSION 0)


--User Experience Quarterly Scores
;WITH uxScores AS
(
	SELECT ServerArtifactID,
		AVG(a.WeekUserExperience4SLRQScore) UserExperience4Score,
		AVG(a.WeekUserExperienceSLRQScore) UserExperienceScore
	FROM (
		SELECT ServerArtifactID,
			WeekUserExperience4SLRQScore,
			WeekUserExperienceSLRQScore
		FROM eddsdbo.QoS_Ratings qr
		INNER JOIN #scorableWeeks s ON qr.SummaryDayHour = s.SummaryDayHour
		UNION ALL (
			--Also include the current weekly score if this isn't a Tuesday midnight hour
			SELECT ServerArtifactID,
				WeekUserExperience4SLRQScore,
				WeekUserExperienceSLRQScore
			FROM eddsdbo.QoS_Ratings
			WHERE SummaryDayHour = @summaryDayHour
				AND DATEADD(dd, 1-(DATEPART(dw, @summaryDayHour)), DATEDIFF(dd, 0, @summaryDayHour)) != @summaryDayHour
		)
	) a
	GROUP BY ServerArtifactID
)
UPDATE qr
SET UserExperience4SLRQScore = s.UserExperience4Score,
	UserExperienceSLRQScore = s.UserExperienceScore
FROM eddsdbo.QoS_Ratings qr
INNER JOIN uxScores s
	ON qr.ServerArtifactID = s.ServerArtifactID
WHERE qr.SummaryDayHour = @summaryDayHour

set @lastCompletedTask = 'Calculated user experience scores.'
IF @logging = 1
BEGIN
	EXEC EDDSPerformance.eddsdbo.QoS_LogAppend
		@module = 'BuildandRateSample',
		@taskCompleted = 'Calculating quarterly user experience scores',
		@nextTask = 'Calculating infrastructure performance scores'
END

--System Load Weekly Scores

--Score Web servers together as a group. This code should be modified at some point to allow for a loading server exception.
UPDATE qr
SET WeekSystemLoadScoreWeb = (ISNULL((SELECT (
		AVG(ISNULL(sls.CPUScore, 100)) +
		AVG(ISNULL(sls.RAMPctScore, 100))
	)/2.0
	FROM [eddsdbo].QoS_SystemLoadSummary sls
	INNER JOIN EDDSDBO.[Server] s WITH (NOLOCK)
		ON sls.ServerArtifactID = s.ArtifactID
	INNER JOIN EDDSDBO.[Hours] h WITH (NOLOCK)
		ON sls.SummaryDayHour = h.HourTimeStamp and h.Status != 4
	INNER JOIN EDDSDBO.QoS_SampleHistoryUX sh WITH (NOLOCK)
		ON h.Id = sh.HourId
	WHERE sh.IsActiveArrivalRateSample = 1
		AND s.ServerTypeID = 1
	), 100))
FROM EDDSDBO.QoS_Ratings qr
WHERE SummaryDayHour = @summaryDayHour

/*
	Score SQL servers - Each SQL server gets an individual score.
	 CPU, RAM, Memory Signal State, Waits - each 22.5% weight
	 VLF, Latency - each 5% weight
*/
UPDATE qr
SET WeekSystemLoadScoreSQL = (ISNULL((SELECT
		AVG(ISNULL(sls.CPUScore, 100)) * 0.225 +
		AVG(ISNULL(sls.RAMPctScore, 100)) * 0.225 +
		AVG(ISNULL(sls.MemorySignalStateScore, 100)) * 0.225 +
		AVG(ISNULL(sls.WaitsScore, 100)) * 0.225 +
		AVG(ISNULL(sls.VLFScore, 100)) * 0.05 +
		AVG(ISNULL(sls.LatencyScore, 100)) * 0.05
	FROM [eddsdbo].QoS_SystemLoadSummary sls
	INNER JOIN EDDSDBO.[Server] s WITH (NOLOCK)
		ON sls.ServerArtifactID = s.ArtifactID
	INNER JOIN EDDSDBO.[Hours] h WITH (NOLOCK)
		ON sls.SummaryDayHour = h.HourTimeStamp and h.Status != 4
	INNER JOIN EDDSDBO.QoS_SampleHistoryUX sh WITH (NOLOCK)
		ON h.Id = sh.HourId
			AND s.ServerID = sh.ServerId
	WHERE sh.IsActiveArrivalRateSample = 1
		AND s.ServerTypeID = 3
		AND s.DeletedOn IS NULL
		AND (s.IgnoreServer = 0 OR s.IgnoreServer IS NULL)
		AND sls.ServerArtifactID = qr.ServerArtifactID
	GROUP BY sls.ServerArtifactID
	), 100))
FROM EDDSDBO.QoS_Ratings qr
WHERE SummaryDayHour = @summaryDayHour

--Finish it with the lower average of each server as the SystemLoadScore
UPDATE EDDSDBO.QoS_Ratings
SET WeekSystemLoadScore =
	CASE
		WHEN WeekSystemLoadScoreSQL <= WeekSystemLoadScoreWeb THEN WeekSystemLoadScoreSQL
		ELSE WeekSystemLoadScoreWeb
	END	
WHERE SummaryDayHour = @summaryDayHour

--System Load Quarterly Scores
;WITH slScores AS
(
	SELECT ServerArtifactID,
		AVG(a.WeekSystemLoadScoreSQL) AvgSQLScore,
		AVG(a.WeekSystemLoadScoreWeb) AvgWebScore
	FROM (
		SELECT ServerArtifactID,
			WeekSystemLoadScoreSQL,
			WeekSystemLoadScoreWeb
		FROM eddsdbo.QoS_Ratings qr
		INNER JOIN #scorableWeeks s
			ON qr.SummaryDayHour = s.SummaryDayHour
		UNION ALL (
			--Also include the current weekly score if this isn't a Sunday midnight hour
			SELECT ServerArtifactID,
				WeekSystemLoadScoreSQL,
				WeekSystemLoadScoreWeb
			FROM eddsdbo.QoS_Ratings
			WHERE SummaryDayHour = @summaryDayHour
				AND DATEADD(dd, 1-(DATEPART(dw, @summaryDayHour)), DATEDIFF(dd, 0, @summaryDayHour)) != @summaryDayHour
		)
	) a
	GROUP BY ServerArtifactID
)
UPDATE qr
SET SystemLoadScoreSQL = AvgSQLScore,
	SystemLoadScoreWeb = AvgWebScore,
	SystemLoadScore =
		CASE
			WHEN AvgSQLScore <= AvgWebScore THEN AvgSQLScore
			ELSE AvgWebScore
		END
FROM eddsdbo.QoS_Ratings qr
INNER JOIN slScores s
	ON qr.ServerArtifactID = s.ServerArtifactID
WHERE qr.SummaryDayHour = @summaryDayHour

set @lastCompletedTask = 'Calculated infrastructure performance scores.'
IF @logging = 1
BEGIN
	EXEC EDDSPerformance.eddsdbo.QoS_LogAppend
		@module = 'BuildandRateSample',
		@taskCompleted = 'Calculated infrastructure performance scores',
		@nextTask = 'Calculating recoverability/integrity scores'
END

--Recoverability/Integrity Weekly Scores
UPDATE eddsdbo.QoS_Ratings
SET WeekIntegrityScore = @weekIntegrityScore
WHERE SummaryDayHour = @summaryDayHour

--Recoverability/Integrity Quarterly Scores
UPDATE eddsdbo.QoS_Ratings
SET IntegrityScore = (SELECT AVG(a.WeekIntegrityScore)
	FROM (
		SELECT
			WeekIntegrityScore
		FROM eddsdbo.QoS_Ratings qr
		INNER JOIN #scorableWeeks s
			ON qr.SummaryDayHour = s.SummaryDayHour
		UNION ALL (
			--Also include the current weekly score if this isn't a Tuesday midnight hour
			SELECT
				WeekIntegrityScore
			FROM eddsdbo.QoS_Ratings
			WHERE SummaryDayHour = @summaryDayHour
				AND DATEADD(dd, 1-(DATEPART(dw, @summaryDayHour)), DATEDIFF(dd, 0, @summaryDayHour)) != @summaryDayHour
		)
	) a
)
WHERE SummaryDayHour = @summaryDayHour;

set @lastCompletedTask = 'Calculated recoverability/integrity scores.'
IF @logging = 1
BEGIN
	EXEC EDDSPerformance.eddsdbo.QoS_LogAppend
		@module = 'BuildandRateSample',
		@taskCompleted = 'Calculated recoverability/integrity scores',
		@nextTask = 'Taking snapshot of the completed hour'
END


--Sometimes multiple hours will come in together (e.g. previously failed case-hour completes, split finishes)
--In this case, we need to regenerate the snapshots
SET @updatedHour = (SELECT MIN(SummaryDayHour) FROM
(
	SELECT @summaryDayHour SummaryDayHour
	UNION ALL SELECT SummaryDayHour FROM eddsdbo.QoS_SystemLoadSummary
		WHERE RowHash IS NULL
	UNION ALL SELECT SummaryDayHour FROM eddsdbo.QoS_UptimeRatings
		WHERE RowHash IS NULL
	UNION ALL SELECT SummaryDayHour FROM eddsdbo.QoS_UserXInstanceSummary
		WHERE RowHash IS NULL
) H);

/* Set hashes for new rows */
UPDATE [EDDSPerformance].[eddsdbo].[QoS_SystemLoadSummary]
SET RowHash = HASHBYTES('SHA1',
	CAST(ISNULL(QSLSID, 0) AS varchar) +
	CAST(ISNULL(ServerArtifactID, 0) AS varchar) +
	CAST(ISNULL(ServerTypeID, 0) AS varchar) +
	CAST(ISNULL(RAMPagesScore, 0) AS varchar) +
	CAST(ISNULL(RAMPctScore, 0) AS varchar) +
	CAST(ISNULL(CPUScore, 0) AS varchar) +
	CAST(ISNULL(MemorySignalStateScore, 0) AS varchar) +
	CAST(ISNULL(PoisonWaits, 0) AS varchar) +
	CAST(ISNULL(WaitsScore, 0) AS varchar) +
	CAST(ISNULL(VLFScore, 0) AS varchar) +
	CAST(ISNULL(LatencyScore, 0) AS varchar) +
	CAST(ISNULL(QoSHourID, 0) AS varchar) +
	CAST(ISNULL(SummaryDayHour, 0) AS varchar)
)
WHERE RowHash IS NULL

UPDATE [EDDSPerformance].[eddsdbo].[QoS_UserXInstanceSummary]
SET RowHash = HASHBYTES('SHA1',
	CAST(ISNULL(UserXID, 0) AS varchar) +
	CAST(ISNULL(ServerArtifactID, 0) AS varchar) +
	CAST(ISNULL(QoSHourID, 0) AS varchar) +
	CAST(ISNULL(SummaryDayHour, 0) AS varchar) +
	CAST(ISNULL(qtyActiveUsers, 0) AS varchar) +
	CAST(ISNULL(AVGConcurrency, 0) AS varchar) +
	CAST(ISNULL(ArrivalRate, 0) AS varchar) +
	CAST(ISNULL(AvgSQScorePerUser, 0) AS varchar) +
	CAST(ISNULL(DocumentSearchScore, 0) AS varchar)
)
WHERE RowHash IS NULL

UPDATE [EDDSPerformance].[eddsdbo].[QoS_Ratings]
SET RowHash = HASHBYTES('SHA1',
	CAST(ISNULL(QoSHourID, 0) AS varchar) +
	CAST(ISNULL(UserExperience4SLRQScore, 0) AS varchar) +
	CAST(ISNULL(UserExperienceSLRQScore, 0) AS varchar) +
	CAST(ISNULL(SystemLoadScoreWeb, 0) AS varchar) +
	CAST(ISNULL(SystemLoadScoreSQL, 0) AS varchar) +
	CAST(ISNULL(SystemLoadScore, 0) AS varchar) +
	CAST(ISNULL(IntegrityScore, 0) AS varchar) +
	CAST(ISNULL(WeekUserExperience4SLRQScore, 0) AS varchar) +
	CAST(ISNULL(WeekUserExperienceSLRQScore, 0) AS varchar) +
	CAST(ISNULL(WeekSystemLoadScoreWeb, 0) AS varchar) +
	CAST(ISNULL(WeekSystemLoadScoreSQL, 0) AS varchar) +
	CAST(ISNULL(WeekSystemLoadScore, 0) AS varchar) +
	CAST(ISNULL(WeekIntegrityScore, 0) as varchar) +
	CAST(ISNULL(SummaryDayHour, 0) AS varchar) +
	CAST(ISNULL(QRatingID, 0) AS varchar)
)
WHERE RowHash IS NULL

set @lastCompletedTask = 'Determined hours that need to be included in snapshots.'
IF @logging = 1
BEGIN
	SET @loggingVars = '@updatedHour = ' + CAST(@updatedHour as varchar) + ', @summaryDayHour = ' + CAST(@summaryDayHour as varchar);
	EXEC EDDSPerformance.eddsdbo.QoS_LogAppend
		@module = 'BuildandRateSample',
		@taskCompleted = 'Determined hours that need to be included in snapshots',
		@otherVars = @loggingVars
END

END -- Create sproc end