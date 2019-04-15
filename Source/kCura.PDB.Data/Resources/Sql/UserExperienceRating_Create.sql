

INSERT INTO eddsdbo.QoS_UserExperienceRatings(
	ServerArtifactId,
	ArrivalRateUXScore,
	ConcurrencyUXScore,
	HourId)
VALUES(
	@serverArtifactId,
	@arrivalRateUXScore,
	@concurrencyUXScore,
	@hourId)