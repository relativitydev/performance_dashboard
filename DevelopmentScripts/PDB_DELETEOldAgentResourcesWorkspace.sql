DECLARE @oldAgentGuids table(agentGuid UNIQUEIDENTIFIER)
INSERT @oldAgentGuids(agentGuid) 
	SELECT * from (
		values('514D019C-6BDC-40FF-A2E8-938B7C5E43B1')
		,('FDC70DC6-9DFB-4F8E-8D8C-3C7CFF515A47')
		,('5177338F-9B85-4AC5-A18A-BFAA98AF1804')
		,('5C475C47-90F0-407B-828F-83A37E29F2F8')
		,('93DCA58D-2E8E-4EF2-BB31-CA7F4354E91C')
		) AS TempTable([Guid])

DECLARE @agentTypeIds table(agentTypeId int) 
INSERT INTO @agentTypeIds 
	SELECT ArtifactID FROM [EDDSDBO].[AgentType] 
		JOIN @oldAgentGuids o ON [AgentTypeGuid] = o.agentGuid
		--WHERE [AgentTypeGuid] = (SELECT * FROM @oldAgentGuids)
--SELECT * FROM @agentTypeIds

DELETE [EDDSDBO].[ApplicationGuid]
FROM [EDDSDBO].[ApplicationGuid]
JOIN @oldAgentGuids o ON [ApplicationGuid].[ArtifactGuid] = o.agentGuid
WHERE [ApplicationGuid].[ArtifactGuid] = o.agentGuid

DELETE [EDDSDBO].[ApplicationAgentType]
FROM [EDDSDBO].[ApplicationAgentType]
JOIN @agentTypeIds ati ON [ApplicationAgentType].[AgentTypeArtifactID] = ati.agentTypeId
WHERE [ApplicationAgentType].[AgentTypeArtifactID] = ati.agentTypeId

DELETE [EDDSDBO].[AgentType]
FROM [EDDSDBO].[AgentType]
JOIN @oldAgentGuids o ON [AgentType].[AgentTypeGuid] = o.agentGuid
WHERE [AgentType].[AgentTypeGuid] = o.agentGuid