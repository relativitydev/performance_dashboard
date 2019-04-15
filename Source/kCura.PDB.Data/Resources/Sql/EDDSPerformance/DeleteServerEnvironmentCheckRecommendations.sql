DELETE FROM EDDSPerformance.eddsdbo.EnvironmentCheckRecommendations
WHERE 
Scope not in (select distinct ServerName from EDDSPerformance.eddsdbo.[Server] where DeletedOn is null)
and Scope <> 'Relativity'

--clear out previous results
DELETE FROM EDDSPerformance.eddsdbo.EnvironmentCheckRecommendations
WHERE Scope = @ServerName and [section] = 'SQL Configuration';