SELECT TOP (1) 1
  FROM [EDDS].[eddsdbo].[Agent] a with(nolock)
  inner join [edds].eddsdbo.AgentType at with(nolock) on a.AgentTypeArtifactID = at.ArtifactID
  where at.[Guid] = @agentTypeGuid