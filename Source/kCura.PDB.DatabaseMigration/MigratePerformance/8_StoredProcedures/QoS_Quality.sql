USE EDDSPerformance
GO

IF EXISTS (select 1 from sysobjects where [name] = 'QoS_Quality' and type = 'P')  
BEGIN
	DROP PROCEDURE EDDSDBO.QoS_Quality
END
GO
CREATE PROCEDURE EDDSDBO.QoS_Quality
	@depth INT = 1
AS
BEGIN
	--This procedure returns the most recent quarterly and weekly scores
	;WITH serverScores AS
	(
		SELECT TOP (@depth)
			rs.ArtifactID ServerArtifactID,
			rs.name ServerName,
			c.name serverType,
			(UserExperience4SLRQScore + UserExperienceSLRQScore)/2 UserExperienceScore,
			SystemLoadScore,
			IntegrityScore,
			ISNULL(UptimeScore, 100) UptimeScore,
			(WeekUserExperience4SLRQScore + WeekUserExperienceSLRQScore)/2 WeekUserExperienceScore,
			WeekSystemLoadScore,
			WeekIntegrityScore,
			ISNULL(WeekUptimeScore, 100) WeekUptimeScore
		FROM EDDSDBO.QoS_Ratings qr WITH(NOLOCK)
		INNER JOIN [EDDS].[eddsdbo].ResourceServer rs WITH (NOLOCK)
		ON qr.ServerArtifactID = rs.ArtifactID
		INNER JOIN edds.eddsdbo.[Code] c WITH(NOLOCK)
					ON RS.[Type] = C.artifactID
		LEFT JOIN EDDSDBO.QoS_UptimeRatings qur WITH(NOLOCK)
		ON qr.SummaryDayHour = qur.SummaryDayHour
		WHERE qr.SummaryDayHour = (SELECT MAX(SummaryDayHour) FROM eddsdbo.QoS_Ratings WITH(NOLOCK) WHERE RowHash IS NOT NULL)
	)
	SELECT
		ServerArtifactID,
		ServerName,
		serverType,
		(UserExperienceScore + SystemLoadScore + IntegrityScore + UptimeScore) / 4 AS AVGRating,
		(WeekUserExperienceScore + WeekSystemLoadScore + WeekIntegrityScore + WeekUptimeScore) / 4 AS WeeklyRating,
		UserExperienceScore,
		SystemLoadScore,
		IntegrityScore,
		UptimeScore,
		WeekUserExperienceScore,
		WeekSystemLoadScore,
		WeekIntegrityScore,
		WeekUptimeScore
	FROM serverScores
	ORDER BY AVGRating
END