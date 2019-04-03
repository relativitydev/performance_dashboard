

SELECT COUNT(*)
FROM eddsdbo.QoS_WaitSummary ws WITH(NOLOCK)
INNER JOIN eddsdbo.QoS_WaitDetail wd WITH(NOLOCK)
ON ws.WaitSummaryID = wd.WaitSummaryID
INNER JOIN eddsdbo.QoS_Waits w WITH(NOLOCK)
ON wd.WaitTypeID = w.WaitTypeID
INNER JOIN eddsdbo.[Server] s WITH(NOLOCK)
ON s.ArtifactId = ws.ServerArtifactID
WHERE ws.SummaryDayHour = @hourTimeStamp --QoS_SystemLoadSummary.SummaryDayHour
	AND s.ServerID = @serverId
	--AND ws.ServerArtifactID = @serverArtifactId --QoS_SystemLoadSummary.ServerArtifactID
	AND w.IsPoisonWait = 1
	AND wd.DifferentialWaitMs > 1000