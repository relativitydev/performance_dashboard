

UPDATE eddsdbo.QoS_SampleHistoryUX
SET IsActiveArrivalRateSample = @isActiveArrivalRateSample
,IsActiveConcurrencySample = @isActiveConcurrencySample
WHERE ServerId = @serverId 
	AND HourId = @hourId