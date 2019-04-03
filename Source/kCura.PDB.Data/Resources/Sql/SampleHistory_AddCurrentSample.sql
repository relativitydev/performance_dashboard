

IF(NOT EXISTS(SELECT * FROM eddsdbo.QoS_SampleHistoryUX WHERE ServerId = @serverId AND HourId = @hourId))
BEGIN
	INSERT INTO eddsdbo.QoS_SampleHistoryUX(ServerId, HourId, IsActiveArrivalRateSample, IsActiveConcurrencySample)
	VALUES(@serverId, @hourId, @isActiveArrivalRateSample, @isActiveConcurrencySample)
END
ELSE
BEGIN
	UPDATE eddsdbo.QoS_SampleHistoryUX
	SET IsActiveArrivalRateSample = @isActiveArrivalRateSample
	,IsActiveConcurrencySample = @isActiveConcurrencySample
	WHERE ServerId = @serverId 
		AND HourId = @hourId
END