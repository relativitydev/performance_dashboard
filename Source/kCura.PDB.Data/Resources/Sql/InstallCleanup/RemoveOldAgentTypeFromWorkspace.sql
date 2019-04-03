DECLARE @agentTypeID int = (SELECT TOP 1 ArtifactID FROM [EDDSDBO].[AgentType] WHERE [AgentTypeGuid] = @oldAgentGuid)

DELETE FROM [EDDSDBO].[ApplicationGuid] WHERE [ArtifactGuid] = @oldAgentGuid
DELETE FROM [EDDSDBO].[ApplicationAgentType] WHERE  AgentTypeArtifactID = @agentTypeID
DELETE FROM [EDDSDBO].[AgentType] WHERE [AgentTypeGuid] = @oldAgentGuid