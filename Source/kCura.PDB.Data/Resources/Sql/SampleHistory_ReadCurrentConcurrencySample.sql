

SELECT *
FROM eddsdbo.QoS_SampleHistoryUX with(nolock)
WHERE [IsActiveConcurrencySample] = 1
	AND ServerId = @serverId