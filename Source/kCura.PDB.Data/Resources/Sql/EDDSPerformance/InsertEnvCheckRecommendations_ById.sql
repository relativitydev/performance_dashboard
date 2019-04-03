USE [EDDSPerformance]

delete r
from eddsperformance.[eddsdbo].[EnvironmentCheckRecommendations] r
inner join eddsqos.eddsdbo.EnvironmentCheckRecommendationsDefaults d on r.[Name] = d.Name
where r.[Scope] = @Scope and d.ID = @ID

INSERT INTO eddsperformance.[eddsdbo].[EnvironmentCheckRecommendations]
           ([Scope]
           ,[Name]
           ,[Description]
           ,[Status]
           ,[Recommendation]
           ,[Value]
           ,[Section]
           ,[Severity])
(select top 1 
	@Scope as [scope],
	dflts.Name,
	dflts.[Description],
	dflts.[Status],
	dflts.Recommendation,
	@Value as [Value],
	dflts.Section,
	dflts.Severity
	from eddsqos.eddsdbo.EnvironmentCheckRecommendationsDefaults as dflts
	where dflts.id = @ID)



