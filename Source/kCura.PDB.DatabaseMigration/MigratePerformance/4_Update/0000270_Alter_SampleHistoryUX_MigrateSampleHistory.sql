USE [EDDSPerformance];

-- Check that the old SampleHistory and new SampleHistoryUX tables exist
IF EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'QoS_SampleHistory' AND TABLE_SCHEMA = N'EDDSDBO') 
	AND EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'QoS_SampleHistoryUX' AND TABLE_SCHEMA = N'EDDSDBO') 
BEGIN
	-- Migrate data to SampleHistoryUX
	INSERT INTO eddsdbo.[QoS_SampleHistoryUX] (ServerId, HourId, IsActiveArrivalRateSample, IsActiveConcurrencySample)
	SELECT s.ServerID, h.ID, 0, 0
	FROM eddsdbo.[QoS_SampleHistory] sh
		INNER JOIN eddsdbo.[Server] s on sh.ServerArtifactID = s.ArtifactID
		INNER JOIN eddsdbo.[Hours] h on sh.SummaryDayHour = h.HourTimeStamp -- This won't work if the hours don't exist yet.
		LEFT JOIN eddsdbo.[QoS_SampleHistoryUX] shux on shux.ServerId = s.ServerID AND shux.HourId = h.ID
	WHERE shux.HourId is null -- Don't include duplicates (should new hours be added?) [No, the create hour event is responsible for creating all of the cascading events as well, so it needs to create them if need be.]
END