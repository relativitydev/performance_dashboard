USE EDDSPerformance

SELECT count(a.ArtifactID) as NumberOfAgents, at.Name
FROM [EDDS].[eddsdbo].[Agent] as a WITH(NOLOCK)
right join edds.eddsdbo.AgentType as at 
on at.ArtifactID = a.AgentTypeArtifactID and (a.[Enabled] = 1 and a.[Updated] = 0)
WHERE at.Name LIKE 'Performance Dashboard - %'
group by a.AgentTypeArtifactID, at.Name