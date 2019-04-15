

SELECT * 
FROM eddsdbo.QoS_UserExperienceRatings with(nolock)
WHERE
	ServerArtifactId = @ServerArtifactId
	and HourId = @HourId