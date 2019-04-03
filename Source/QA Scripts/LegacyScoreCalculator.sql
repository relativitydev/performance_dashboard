/*

Script Name: PDB_LegacyScoreCalculator
Script Date: 2015-05-26
Author(s): Joseph Low

Purpose:
This script calculates a "legacy" score for PDB using the 9.1 method.
 * Build temporary samples using known arrival rate and concurrency information
 * Calculate an hourly user experience score for sample hours
 * Insert ratings for each server
 * Output the ratings corresponding to the server with the lowest weekly score

*/

USE EDDSPerformance
GO
--SET STATISTICS TIME ON
--SET STATISTICS IO ON
SET DATEFIRST 2

DECLARE @i INT = 1,
	@imax INT = 0,
	@QoSHourID BIGINT,
	@SummaryDayHour DATETIME,
	@trustHour DATETIME = DATEDIFF(dd, 0, DATEADD(dd, 1-(DATEPART(dw, getutcdate())), getutcdate()));

--Set up database objects
CREATE TABLE #sampleHistory (
	[QSampleID] [int] IDENTITY(1,1) NOT NULL,
	[QoSHourID] [bigint] NULL,
	[SummaryDayHour] [datetime] NULL,
	[ServerArtifactID] [int] NULL,
	[ArrivalRate] [decimal](10, 3) NULL,
	[AVGConcurrency] [decimal](10, 3) NULL,
	[IsActiveSample] [bit] NULL,
	[IsActive4Sample] [bit] NULL,
	[IsActiveWeeklySample] [bit] NULL,
	[IsActiveWeekly4Sample] [bit] NULL,
	PRIMARY KEY CLUSTERED 
	(
		[QSampleID] ASC
	)
)

Create TABLE #qos_get7days (
	G7DID int IDENTITY(1,1) NOT NULL,
	Summary7DayHour date
)
CREATE UNIQUE CLUSTERED INDEX [CiQoS_DateTime] ON #qos_get7days
(
	Summary7DayHour DESC
) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = ON)

CREATE TABLE #uxInstanceSummary (
	[UserXID] [int] IDENTITY(1,1) NOT NULL,
	[ServerArtifactID] [int] NULL,
	[QoSHourID] [bigint] NULL,
	[SummaryDayHour] [datetime] NULL,
	[qtyActiveUsers] [int] NULL,
	[AVGConcurrency] [decimal](10, 3) NULL,
	[ArrivalRate] [decimal](10, 3) NULL,
	[AvgSQScorePerUser] [decimal](10, 3) NULL,
	PRIMARY KEY CLUSTERED 
	(
		[UserXID] ASC
	)
)

CREATE TABLE #concurrencyItems (
	[CIID] [int] IDENTITY(1,1) NOT NULL,
	[UserID] [int] NULL,
	[ExecutionTime] [int] NULL,
	[IsComplex] [bit] NULL,
	PRIMARY KEY CLUSTERED 
	(
		[CIID] ASC
	)
)

CREATE TABLE #ratings (
	[QRatingID] [int] IDENTITY(1,1) NOT NULL,
	[ServerArtifactID] [int] NULL,
	[SummaryDayHour] [datetime] NULL,
	[QoSHourID] [bigint] NULL,
	[WeeklyScore] [decimal](5,2) NULL CONSTRAINT [DF_Ratings_WeeklyScore] DEFAULT ((100)),
	[WeekUserExperienceScore] [decimal](5,2) NULL CONSTRAINT [DF_Ratings_WeekUXScore] DEFAULT ((100)),
	[WeekSystemLoadScore] [decimal](5, 2) NULL CONSTRAINT [DF_Ratings_WeekSystemLoadScore]  DEFAULT ((100)),
	[WeekUserExperience4SLRQScore] [decimal](5, 2) NULL CONSTRAINT [DF_Ratings_WeekUserExperience4SLRQScore]  DEFAULT ((100)),
	[WeekUserExperienceSLRQScore] [decimal](5, 2) NULL CONSTRAINT [DF_Ratings_WeekUserExperienceSLRQScore]  DEFAULT ((100)),
	[WeekSystemLoadScoreWeb] [decimal](5, 2) NULL CONSTRAINT [DF_Ratings_WeekSystemLoadScoreWeb]  DEFAULT ((100)),
	[WeekSystemLoadScoreSQL] [decimal](5, 2) NULL CONSTRAINT [DF_Ratings_WeekSystemLoadScoreSQL]  DEFAULT ((100)),
	/*[UserExperience4SLRQScore] [decimal](5, 2) NULL CONSTRAINT [DF_Ratings_UserExperience4SLRQScore]  DEFAULT ((100)),
	[UserExperienceSLRQScore] [decimal](5, 2) NULL CONSTRAINT [DF_Ratings_UserExperienceSLRQScore]  DEFAULT ((100)),
	[SystemLoadScoreWeb] [decimal](5, 2) NULL CONSTRAINT [DF_Ratings_SystemLoadScoreWeb]  DEFAULT ((100)),
	[SystemLoadScoreSQL] [decimal](5, 2) NULL CONSTRAINT [DF_Ratings_SystemLoadScoreSQL]  DEFAULT ((100)),
	[SystemLoadScore] [decimal](5, 2) NULL CONSTRAINT [DF_Ratings_SystemLoadScore]  DEFAULT ((100)),*/
	PRIMARY KEY CLUSTERED 
	(
		[QRatingID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
)

--Build temporary samples using the arrival rate and concurrency in QoS_UserXInstanceSummary
;WITH topTwentyInsert AS
(
	SELECT serverArtifactID, QoSHourID, SummaryDayHour, ArrivalRate,
			NTILE(5) OVER(PARTITION BY ServerArtifactID ORDER BY ArrivalRate DESC) AS  percentile
	FROM EDDSDBO.QoS_UserXInstanceSummary WITH(NOLOCK)
	WHERE SummaryDayHour > DATEADD(dd, -90, @trustHour) 
	GROUP BY serverArtifactID, QoSHourID, SummaryDayHour, arrivalRate
)
INSERT INTO #sampleHistory (QoSHourID, SummaryDayHour, ServerArtifactID, ArrivalRate, AVGConcurrency, IsActiveSample, IsActive4Sample, IsActiveWeeklySample, IsActiveWeekly4Sample)
SELECT tt.QoSHourID, tt.summaryDayHour, tt.serverArtifactID, uxis.arrivalrate, uxis.AVGConcurrency, 1, 0, 0, 0 
from topTwentyInsert tt
INNER JOIN EDDSDBO.QoS_UserXInstanceSummary uxis on 
tt.QoSHourID = uxis.QoSHourID AND tt.serverArtifactID = uxis.serverArtifactID
Where tt.percentile = 1
AND tt.ArrivalRate >= 0.05

;WITH topFour AS
(
	SELECT serverArtifactID, QoSHourID,
			NTILE(25) OVER(PARTITION BY ServerArtifactID ORDER BY AVGConcurrency DESC) AS 'top4percent'
	FROM EDDSDBO.QoS_UserXInstanceSummary WITH(NOLOCK)
	WHERE QoSHourID in (
		SELECT QoSHourID FROM #sampleHistory Where IsActiveSample = 1
	)  
	GROUP BY ServerArtifactID,QoSHourID,AVGConcurrency
) 
UPDATE qsh
SET qsh.isActive4Sample = 1 
FROM #sampleHistory qsh
INNER JOIN topFour t4
ON qsh.QoSHourID = t4.QoSHourID
WHERE t4.top4percent = 1

INSERT INTO #qos_get7days (Summary7DayHour)
SELECT SummaryDayHour
FROM #sampleHistory
WHERE IsActiveSample = 1
ORDER BY SummaryDayHour DESC

DELETE FROM #qos_get7days
WHERE G7DID NOT IN (
	SELECT TOP 7 G7DID
	FROM #qos_get7days
	ORDER BY Summary7DayHour DESC
)

UPDATE #sampleHistory
SET IsActiveWeeklySample = 1
WHERE QoSHourID IN (
	SELECT QoSHourID FROM #sampleHistory
	WHERE isActiveSample = 1
		AND SummaryDayHour >= (
			SELECT TOP 1 Summary7DayHour
			FROM #qos_get7days
			ORDER by Summary7DayHour ASC
		)
)

;WITH topFour AS
(
	SELECT serverArtifactID, QoSHourID,
			NTILE(25) OVER(PARTITION BY ServerArtifactID ORDER BY AVGConcurrency DESC) AS  'top4percent'
	FROM EDDSDBO.QoS_UserXInstanceSummary WITH(NOLOCK)
	WHERE QoSHourID IN (SELECT QoSHourID FROM #sampleHistory WHERE IsActiveWeeklySample = 1)  
	GROUP BY ServerArtifactID,QoSHourID,AVGConcurrency
) 
UPDATE qsh
SET qsh.IsActiveWeekly4Sample = 1
FROM #sampleHistory qsh
INNER JOIN topFour t4 ON 
qsh.QoSHourID = t4.QoSHourID
WHERE t4.top4percent = 1

/*
SELECT *
FROM #sampleHistory
*/

--Collect hourly UX data and determine the AvgSQScorePerUser for hours that apply to the weekly score
INSERT INTO #uxInstanceSummary
	([ServerArtifactID], [QoSHourID], [SummaryDayHour], [AVGConcurrency], [ArrivalRate])
SELECT
	uxis.ServerArtifactID, uxis.QoSHourID, uxis.SummaryDayHour, uxis.AVGConcurrency, uxis.ArrivalRate
FROM eddsdbo.QoS_UserXInstanceSummary uxis WITH(NOLOCK)
INNER JOIN #sampleHistory sh WITH(NOLOCK)
ON uxis.QoSHourID = sh.QoSHourID
WHERE IsActiveWeeklySample = 1

SELECT
	@i = MIN(UserXID),
	@imax = MAX(UserXID)
FROM #uxInstanceSummary

WHILE (@i <= @imax)
BEGIN
	SELECT @QoSHourID = QoSHourID,
		@SummaryDayHour = SummaryDayHour
	FROM #uxInstanceSummary
	WHERE UserXID = @i;

	TRUNCATE TABLE #concurrencyItems

	--This hour's audits
	INSERT INTO #concurrencyItems
		(UserID, ExecutionTime, IsComplex)
	SELECT 
		UserID,
		round(executionTime, -3)/1000 executionTime,
		IsComplex
	FROM eddsdbo.QoS_VarscatOutputDetailCumulative
	WHERE QoSHourID = @QoSHourID
		AND QoSAction IN (281, 282)

	;WITH sHeartbeat   --this calculates an average of percent of simple queries across all simple query users.
	as
	(
		SELECT 
			sq.userID, 
			CASE
			--No long-running queries
			WHEN MAX(ISNULL(slrq.slrqCount, 0)) = 0 THEN 100
			--No short-running queries, but we already know there are some long-running ones
			WHEN MAX(ISNULL(sqs.sqCount, 0)) = 0 THEN 0
			--Mix of short and long-running queries
			ELSE
				CAST((MAX(ISNULL(sqs.sqCount, 0))/
					(CAST((MAX(ISNULL(slrq.slrqCount, 0)) + MAX(ISNULL(sqs.sqCount, 0))) as DECIMAL (10,4))
				)) as DECIMAL (10,4)) * 100
			END  SQpercentageScore, --This is a percentage determined from: SQ / (SQ + SLRQ)
			CAST((MAX(ISNULL(slrq.slrqCount, 0))) as DECIMAL (10,0)) SLRQs, --This is the number of simple queries that were long-running
			CAST((MAX(ISNULL(sqs.sqCount, 0))) as DECIMAL (10,0)) SQs --This is the number of simple queries that were NOT long-running
		FROM #concurrencyItems sq
		FULL OUTER JOIN
			(SELECT DISTINCT userID, COUNT(userID) OVER (PARTITION BY userID) slrqCount
			FROM #concurrencyItems ci
			WHERE IsComplex = 0 and executionTIME > 2) AS slrq 
			ON sq.userid = slrq.userid
		FULL OUTER JOIN
			(SELECT DISTINCT userID, COUNT(userID) OVER (PARTITION BY userID) sqCount
			FROM #concurrencyItems
			WHERE IsComplex = 0 and executionTIME <= 2) AS sqs 
		ON sqs.userid = sq.userid
		WHERE IsComplex = 0
		GROUP by sq.userID
	)
	UPDATE  #uxInstanceSummary
	 SET AvgSQScorePerUser =  (SELECT round((SUM(sHeartbeat.SQpercentageScore)/COUNT(sHeartbeat.userid)),2) FROM sHeartbeat)
	 WHERE UserXID = @i;

	SET @i = ISNULL((SELECT TOP 1 [UserXID]
		FROM #uxInstanceSummary
		WHERE [UserXID] > @i), @imax + 1);
END

--Insert ratings for each server
INSERT INTO #ratings
	(ServerArtifactID, SummaryDayHour, QoSHourID)
SELECT DISTINCT ServerID,
	AuditStartDate,
	eddsdbo.QoS_GetServerHourID(ServerID, AuditStartDate)
FROM eddsdbo.QoS_CasesToAudit WITH(NOLOCK)
WHERE AuditStartDate = (
	SELECT MAX(AuditStartDate)
	FROM eddsdbo.QoS_CasesToAudit WITH(NOLOCK)
	WHERE AuditStartDate <= @trustHour
)

--Set UX and SL scores based on the temporary samples
UPDATE qr
SET
	/*UserExperienceSLRQScore = ISNULL((
		SELECT AVG(ISNULL(AvgSQScorePerUser, 100))
		FROM #uxInstanceSummary uxis
		INNER JOIN #sampleHistory sh
		ON uxis.QoSHourID = sh.QoSHourID
		WHERE sh.IsActiveSample = 1
			AND sh.ServerArtifactID = qr.ServerArtifactID
	), 100),
	UserExperience4SLRQScore = ISNULL((
		SELECT AVG(ISNULL(AvgSQScorePerUser, 100))
		FROM #uxInstanceSummary uxis
		INNER JOIN #sampleHistory sh
		ON uxis.QoSHourID = sh.QoSHourID
		WHERE sh.IsActive4Sample = 1
			AND sh.ServerArtifactID = qr.ServerArtifactID
	), 100),*/
	WeekUserExperienceSLRQScore = ISNULL((
		SELECT AVG(ISNULL(AvgSQScorePerUser, 100))
		FROM #uxInstanceSummary uxis
		INNER JOIN #sampleHistory sh
		ON uxis.QoSHourID = sh.QoSHourID
		WHERE sh.IsActiveWeeklySample = 1
			AND sh.ServerArtifactID = qr.ServerArtifactID
	), 100),
	WeekUserExperience4SLRQScore = ISNULL((
		SELECT AVG(ISNULL(AvgSQScorePerUser, 100))
		FROM #uxInstanceSummary uxis
		INNER JOIN #sampleHistory sh
		ON uxis.QoSHourID = sh.QoSHourID
		WHERE sh.IsActiveWeekly4Sample = 1
			AND sh.ServerArtifactID = qr.ServerArtifactID
	), 100),
	/*SystemLoadScoreSQL = ISNULL((
		SELECT CASE WHEN AVG(ISNULL(sls.RAMPctScore, 100)) < 80
			THEN (AVG(ISNULL(sls.CPUScore, 100)) + AVG(ISNULL(sls.RAMPagesScore, 100)) + AVG(ISNULL(sls.RAMPctScore, 100)))/3
			ELSE (AVG(ISNULL(sls.CPUScore, 100)) + AVG(ISNULL(sls.RAMPctScore, 100)))/2
		END
		FROM eddsdbo.QoS_SystemLoadSummary sls WITH(NOLOCK)
		INNER JOIN #sampleHistory sh WITH(NOLOCK)
		ON sls.QoSHourID = sh.QoSHourID
		WHERE sh.IsActiveSample = 1
			AND sh.ServerArtifactID = qr.ServerArtifactID
			AND sls.ServerTypeID = 3
	), 100),
	SystemLoadScoreWeb = ISNULL((
		SELECT CASE WHEN AVG(ISNULL(sls.RAMPctScore, 100)) < 80
			THEN (AVG(ISNULL(sls.CPUScore, 100)) + AVG(ISNULL(sls.RAMPagesScore, 100)) + AVG(ISNULL(sls.RAMPctScore, 100)))/3
			ELSE (AVG(ISNULL(sls.CPUScore, 100)) + AVG(ISNULL(sls.RAMPctScore, 100)))/2
		END
		FROM eddsdbo.QoS_SystemLoadSummary sls WITH(NOLOCK)
		INNER JOIN #sampleHistory sh WITH(NOLOCK)
		ON sls.QoSHourID = sh.QoSHourID
		WHERE sh.IsActiveSample = 1
			AND sls.ServerTypeID = 1
	), 100),*/
	WeekSystemLoadScoreSQL = ISNULL((
		SELECT CASE WHEN AVG(ISNULL(sls.RAMPctScore, 100)) < 80
			THEN (AVG(ISNULL(sls.CPUScore, 100)) + AVG(ISNULL(sls.RAMPagesScore, 100)) + AVG(ISNULL(sls.RAMPctScore, 100)))/3
			ELSE (AVG(ISNULL(sls.CPUScore, 100)) + AVG(ISNULL(sls.RAMPctScore, 100)))/2
		END
		FROM eddsdbo.QoS_SystemLoadSummary sls WITH(NOLOCK)
		INNER JOIN #sampleHistory sh WITH(NOLOCK)
		ON sls.QoSHourID = sh.QoSHourID
		WHERE sh.IsActiveSample = 1
			AND sh.ServerArtifactID = qr.ServerArtifactID
			AND sls.ServerTypeID = 3
	), 100),
	WeekSystemLoadScoreWeb = ISNULL((
		SELECT CASE WHEN AVG(ISNULL(sls.RAMPctScore, 100)) < 80
			THEN (AVG(ISNULL(sls.CPUScore, 100)) + AVG(ISNULL(sls.RAMPagesScore, 100)) + AVG(ISNULL(sls.RAMPctScore, 100)))/3
			ELSE (AVG(ISNULL(sls.CPUScore, 100)) + AVG(ISNULL(sls.RAMPctScore, 100)))/2
		END
		FROM eddsdbo.QoS_SystemLoadSummary sls WITH(NOLOCK)
		INNER JOIN #sampleHistory sh WITH(NOLOCK)
		ON sls.QoSHourID = sh.QoSHourID
		WHERE sh.IsActiveSample = 1
			AND sls.ServerTypeID = 1
	), 100)
FROM #ratings qr;

UPDATE #ratings
SET
	/*SystemLoadScore = CASE
		WHEN SystemLoadScoreSQL < SystemLoadScoreWeb THEN SystemLoadScoreSQL
		ELSE SystemLoadScoreWeb
	END,*/
	WeekSystemLoadScore = CASE
		WHEN WeekSystemLoadScoreSQL < WeekSystemLoadScoreWeb THEN WeekSystemLoadScoreSQL
		ELSE WeekSystemLoadScoreWeb
	END,
	WeekUserExperienceScore = (WeekUserExperienceSLRQScore + WeekUserExperience4SLRQScore)/2.0;

UPDATE #ratings
SET WeeklyScore = (WeekUserExperienceScore + WeekSystemLoadScore)/2.0

--Spit out scores
SELECT *
FROM #ratings
ORDER BY WeeklyScore

--Cleanup
DROP TABLE #sampleHistory
DROP TABLE #uxInstanceSummary
DROP TABLE #concurrencyItems
DROP TABLE #qos_get7days
DROP TABLE #ratings