USE [EDDS]

SELECT [Enabled]
FROM [eddsdbo].[Agent] with(nolock)
WHERE [ArtifactID] = @agentId