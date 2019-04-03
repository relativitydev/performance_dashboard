

SELECT *
FROM eddsdbo.QoS_SampleHistoryUX with(nolock)
WHERE [IsActiveArrivalRateSample] = 1
	AND ServerId = @serverId