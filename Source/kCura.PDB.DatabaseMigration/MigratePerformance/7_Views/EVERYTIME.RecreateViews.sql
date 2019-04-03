IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.VIEWS WHERE TABLE_SCHEMA = 'eddsdbo' AND TABLE_NAME = 'UserExperienceWorkspaceDetail')
	DROP VIEW eddsdbo.UserExperienceWorkspaceDetail;
	
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.VIEWS WHERE TABLE_SCHEMA = 'eddsdbo' AND TABLE_NAME = 'UserExperienceSearchDetail')
	DROP VIEW eddsdbo.UserExperienceSearchDetail;
	
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.VIEWS WHERE TABLE_SCHEMA = 'eddsdbo' AND TABLE_NAME = 'UserExperienceServerDetail')
	DROP VIEW eddsdbo.UserExperienceServerDetail;
	
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.VIEWS WHERE TABLE_SCHEMA = 'eddsdbo' AND TABLE_NAME = 'SystemLoadServerDetail')
	DROP VIEW eddsdbo.SystemLoadServerDetail;

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.VIEWS WHERE TABLE_SCHEMA = 'eddsdbo' AND TABLE_NAME = 'UptimeDetail')
	DROP VIEW eddsdbo.UptimeDetail;

GO

CREATE VIEW EDDSDBO.SystemLoadServerDetail
AS
SELECT SLS.ServerArtifactID,
	MIN(S.ServerName) [Server],
	SLS.ServerTypeID [ServerType],
	SLS.SummaryDayHour,
	CASE WHEN SLS.[ServerTypeID] = 3
			THEN 
				AVG(ISNULL(sls.CPUScore, 100)) * 0.225 +
				AVG(ISNULL(sls.RAMPctScore, 100)) * 0.225 +
				AVG(ISNULL(sls.MemorySignalStateScore, 100)) * 0.225 +
				AVG(ISNULL(sls.WaitsScore, 100)) * 0.225 +
				AVG(ISNULL(sls.VLFScore, 100)) * 0.05 +
				AVG(ISNULL(sls.LatencyScore, 100)) * 0.05
			ELSE (AVG(ISNULL(CPUScore, 100)) + AVG(ISNULL(RAMPctScore, 100)))/2
	END Score,
	AVG(ISNULL(SLS.CPUScore, 100)) CPUScore,
	AVG(ISNULL(SLS.RAMPctScore, 100)) RAMPctScore,
	CASE WHEN SLS.ServerTypeID = 1 THEN NULL
		ELSE AVG(ISNULL(SLS.MemorySignalStateScore, 100))
	END MemorySignalStateScore,
	CASE WHEN SLS.ServerTypeID = 1 THEN NULL
		ELSE CAST(MAX(ISNULL(SLS.MemorySignalStateRatio * 100, 0)) AS int)
	END MemorySignalStateRatio,
	CASE WHEN SLS.ServerTypeID = 1 THEN NULL
		ELSE MAX(ISNULL(SLS.Pageouts, 0))
	END Pageouts,
	CASE WHEN SLS.ServerTypeID = 1 THEN NULL
		ELSE AVG(ISNULL(SLS.WaitsScore, 100))
	END WaitsScore,
	CASE WHEN SLS.ServerTypeID = 1 THEN NULL
		ELSE AVG(ISNULL(SLS.VLFScore, 100))
	END VirtualLogFilesScore,
	CASE WHEN SLS.ServerTypeID = 1 THEN NULL
		ELSE AVG(ISNULL(SLS.MaxVirtualLogFiles, 0))
	END MaxVirtualLogFiles,
	CASE WHEN SLS.ServerTypeID = 1 THEN NULL
		ELSE AVG(ISNULL(SLS.LatencyScore, 100))
	END LatencyScore,
	CASE WHEN SLS.ServerTypeID = 1 THEN NULL
		ELSE MIN(HighestLatencyDatabase)
	END HighestLatencyDatabase,
	CASE WHEN SLS.ServerTypeID = 1 THEN NULL
		ELSE MIN(ISNULL(ReadLatencyMs, 0))
	END ReadLatencyMs,
	CASE WHEN SLS.ServerTypeID = 1 THEN NULL
		ELSE MIN(ISNULL(WriteLatencyMs, 0))
	END WriteLatencyMs,
	CASE WHEN SLS.ServerTypeID = 1 THEN NULL
		ELSE CAST(MIN(CAST(ISNULL(IsDataFile, 0) AS tinyint)) AS bit)
	END IsDataFile,
	MAX(CAST(ISNULL(SH.IsActiveArrivalRateSample, 0) as tinyint)) IsActiveArrivalRateSample
FROM eddsdbo.QoS_SystemLoadSummary SLS WITH(NOLOCK)
INNER JOIN eddsdbo.[Server] S WITH(NOLOCK)
	ON SLS.ServerArtifactID = S.ArtifactID AND SLS.ServerTypeID = S.ServerTypeID
INNER JOIN eddsdbo.[Hours] H
	ON SLS.SummaryDayHour = H.HourTimeStamp
INNER JOIN eddsdbo.QoS_SampleHistoryUX SH WITH(NOLOCK)
	ON H.Id = SH.HourId
	AND (S.ServerID = SH.ServerId OR SLS.ServerTypeID = 1)
WHERE SLS.SummaryDayHour > DATEADD(dd, -90, getutcdate())
AND (S.IgnoreServer IS NULL OR S.IgnoreServer = 0)
AND (S.DeletedOn IS NULL)
GROUP BY SLS.ServerArtifactID, SLS.ServerTypeID, SLS.SummaryDayHour

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