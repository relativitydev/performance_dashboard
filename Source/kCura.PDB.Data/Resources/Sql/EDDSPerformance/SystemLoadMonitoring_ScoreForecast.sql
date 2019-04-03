USE EDDSPerformance

DECLARE @monitoringMinutes INT = (
	SELECT TOP 1 [Frequency]
	FROM eddsdbo.ProcessControl
	WHERE ProcessControlID = 18
)

SELECT
	s.ServerTypeID,
	s.ServerName,
	--CPU Score
	CAST(ISNULL(CASE
		WHEN pw.PoisonWaits > 0 THEN 0
		WHEN cpu.AvgCPUPct < 60 THEN 100
		WHEN cpu.AvgCPUPct > 85 THEN 0
		ELSE (85.0 - cpu.AvgCPUPct)*100/25
	END, 100) as int) CPUScore,
	--RAM Score
	CAST(ISNULL(CASE
		WHEN pw.PoisonWaits > 0 THEN 0
		WHEN s.ServerTypeID = 1 AND ram.AvgRAMAvailable >= 1048576 THEN 100
		WHEN s.ServerTypeID = 3 AND ram.AvgRAMAvailable >= 4194304 THEN 100
		ELSE (ROUND((LOG10(ram.AvgRAMAvailable + 1)
				/LOG10(CASE WHEN s.ServerTypeID = 1 THEN 1048576 ELSE 4194304 END) * 100),0)
			 + (ROUND(((ram.AvgRAMAvailable)
				/(CASE WHEN s.ServerTypeID = 1 THEN 1048576 ELSE 4194304 END) * 100),0) )
			) / 2
	END, 100) as int) RAMScore,
	--SQL Memory State Score
	CAST(ISNULL(CASE
		WHEN pw.PoisonWaits > 0 THEN 0
		WHEN ISNULL(mem.LowMemoryRatio, 0) <= 0 THEN 100
		WHEN pgo.Pageouts > 0 THEN 0
		WHEN mem.LowMemoryRatio > 0.8 THEN 0
		ELSE (0.8 - mem.LowMemoryRatio) * 100.0 / 0.8
	END, 100) as int) MemorySignalStateScore,
	--Waits Score
	CAST(ISNULL(CASE
		WHEN pw.PoisonWaits > 0 THEN 0
		WHEN ISNULL(ws.SignalWaitsRatio, 0) <= 0.1 THEN 100
		WHEN ws.SignalWaitsRatio > 0.2 THEN 0
		ELSE (0.2 - ws.SignalWaitsRatio) * 100.0 / 0.1
	END, 100) as int) WaitsScore,
	--VLF Score
	CAST(ISNULL(CASE
		WHEN pw.PoisonWaits > 0 THEN 0
		WHEN ISNULL(vlf.VirtualLogFiles, 0) < 10000 THEN 100
		ELSE 0 --This score is all or nothing
	END, 100) as int) VirtualLogFilesScore
FROM [eddsdbo].[Server] s WITH(NOLOCK)
CROSS APPLY (
	SELECT TOP 1 uc.UserCount
	FROM eddsdbo.UserCountDW uc WITH(NOLOCK)
	INNER JOIN EDDS.eddsdbo.[Case] c WITH(NOLOCK)
	ON uc.CaseArtifactID = c.ArtifactID
	WHERE uc.CreatedOn > DATEADD(MINUTE, -@monitoringMinutes, getutcdate())
		AND uc.UserCount > 0
		AND (c.ServerID = s.ArtifactID OR s.ServerTypeID = 1)
) activeUsers
CROSS APPLY (
	SELECT
		AVG(CPUProcessorTimePct) AvgCPUPct
	FROM eddsdbo.ServerProcessorDW WITH(NOLOCK)
	WHERE CreatedOn > DATEADD(MINUTE, -@monitoringMinutes, getutcdate())
		AND ServerID = s.ServerID
) cpu
CROSS APPLY (
	SELECT
		AVG(AvailableMemory) AvgRAMAvailable
	FROM eddsdbo.ServerDW WITH(NOLOCK)
	WHERE CreatedOn > DATEADD(MINUTE, -@monitoringMinutes, getutcdate())
		AND ServerID = s.ServerID
) ram
CROSS APPLY (
	SELECT
		AVG(CAST(LowMemorySignalState as tinyint)) LowMemoryRatio
	FROM eddsdbo.SQLServerDW WITH(NOLOCK)
	WHERE CreatedOn > DATEADD(MINUTE, -@monitoringMinutes, getutcdate())
		AND ServerID = s.ServerID
) mem
CROSS APPLY (
	SELECT
		ISNULL(SUM(Pageouts), 0) Pageouts
	FROM eddsdbo.SQLServerPageouts WITH(NOLOCK)
	WHERE SummaryDayHour >= DATEADD(hh, DATEDIFF(hh, 0, DATEADD(MINUTE, -@monitoringMinutes, getutcdate())), 0)
		AND ServerID = s.ServerID
) pgo
CROSS APPLY (
	SELECT TOP 1
		ISNULL(AVG(VirtualLogFiles), 0) VirtualLogFiles
	FROM eddsdbo.VirtualLogFileSummary WITH(NOLOCK)
	WHERE SummaryDayHour >= DATEADD(hh, DATEDIFF(hh, 0, DATEADD(MINUTE, -@monitoringMinutes, getutcdate())), 0)
		AND ServerArtifactID = s.ArtifactID
) vlf
OUTER APPLY (
	SELECT
		WaitSummaryID,
		SignalWaitsRatio
	FROM eddsdbo.QoS_WaitSummary ws WITH(NOLOCK)
	WHERE ws.SummaryDayHour >= DATEADD(hh, DATEDIFF(hh, 0, DATEADD(MINUTE, -@monitoringMinutes, getutcdate())), 0)
		AND ws.ServerArtifactID = s.ArtifactID
) ws
OUTER APPLY (
	SELECT
		COUNT(*) PoisonWaits
	FROM eddsdbo.QoS_WaitDetail wd WITH(NOLOCK)
	INNER JOIN eddsdbo.QoS_Waits w WITH(NOLOCK)
	ON wd.WaitTypeID = w.WaitTypeID
	WHERE wd.WaitSummaryID = ws.WaitSummaryID
		AND w.IsPoisonWait = 1
		AND wd.DifferentialWaitMs > 1000
) pw
WHERE s.ServerTypeID IN (1, 3)
	AND (S.IgnoreServer IS NULL OR S.IgnoreServer = 0)
	AND S.DeletedOn IS NULL