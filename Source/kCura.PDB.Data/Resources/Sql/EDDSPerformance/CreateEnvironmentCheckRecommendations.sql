INSERT INTO [EDDSPerformance].[eddsdbo].[EnvironmentCheckRecommendations]
([Scope],[Name],[Description],[Status],[Recommendation],[Value],[Section],[Severity])
	VALUES
(@Scope,@Name,@Description,@Status,@Recommendation,@Value,@Section,@Severity)