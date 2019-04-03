

UPDATE eddsdbo.QoS_SampleHistoryUX
SET IsActiveArrivalRateSample = 0
,IsActiveConcurrencySample = 0
WHERE ServerId = @serverId