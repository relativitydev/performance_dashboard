USE [EDDS]

SELECT aa.[ArtifactID]
FROM [EDDS].[eddsdbo].[Agent] as aa WITH(NOLOCK)
inner join [EDDS].[eddsdbo].[AgentType] as att on aa.AgentTypeArtifactID = att.ArtifactID
WHERE att.[Guid] in @agentGuids