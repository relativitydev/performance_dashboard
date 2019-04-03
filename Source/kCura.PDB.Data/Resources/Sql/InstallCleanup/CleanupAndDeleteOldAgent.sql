USE EDDS

DECLARE @applicationGuid UNIQUEIDENTIFIER = '60a1d0a3-2797-4fb3-a260-614cbfd3fa0d'

--Gather all old Trust Agent Resource Files
DECLARE @agentResourcefilesIds table(ArtifactId int);
INSERT @agentResourcefilesIds(ArtifactId) 
SELECT rf.ArtifactId 
FROM eddsdbo.ResourceFile as rf
inner join EDDS.eddsdbo.AssemblyAgentType as aat on rf.ArtifactID = aat.AssemblyArtifactID
inner join EDDS.eddsdbo.AgentType as at on aat.AgentTypeID = at.ArtifactID
WHERE at.[Guid] in @oldAgentGuid

DECLARE @agentTypeIds table(ArtifactId int);
INSERT @agentTypeIds(ArtifactId) 
SELECT at.ArtifactID
from EDDS.eddsdbo.AgentType at
WHERE at.[Guid] in @oldAgentGuid

--DELETE any old EventHandlers if this Agent Assembly holds them
DELETE aevh FROM eddsdbo.AssemblyEventHandler aevh
inner join @agentResourcefilesIds ari ON ari.ArtifactID = aevh.AssemblyArtifactID


--DELETE old Trust Agent
DELETE a FROM eddsdbo.Agent as a
inner join @agentTypeIds as at on a.AgentTypeArtifactID = at.ArtifactID

DELETE a FROM eddsdbo.AgentType a
inner join eddsdbo.AssemblyAgentType as aat on a.ArtifactID = aat.AgentTypeID
inner join eddsdbo.ResourceFile as rf on aat.AssemblyArtifactID = rf.ArtifactID
INNER JOIN @agentResourcefilesIds ari ON rf.ArtifactID = ari.ArtifactId

DELETE aat FROM eddsdbo.AssemblyAgentType aat
inner join @agentTypeIds as at on aat.AgentTypeID = at.ArtifactID

DELETE aat FROM eddsdbo.AssemblyAgentType aat
inner join eddsdbo.ResourceFile as rf on aat.AssemblyArtifactID = rf.ArtifactID
INNER JOIN @agentResourcefilesIds ari ON rf.ArtifactID = ari.ArtifactId

DELETE caat FROM eddsdbo.CaseApplicationAgentType caat
inner join @agentTypeIds as at on caat.AgentTypeID = at.ArtifactID

DELETE ag FROM eddsdbo.ArtifactGuid ag
inner join  @agentTypeIds as at on ag.ArtifactID = at.ArtifactID

DELETE aa FROM eddsdbo.ArtifactAncestry aa
inner join @agentTypeIds as at on aa.ArtifactID = at.ArtifactID

DELETE a FROM eddsdbo.AgentType a
inner join @agentTypeIds as at on a.ArtifactID = at.ArtifactID

DELETE a FROM eddsdbo.Artifact a
inner join @agentTypeIds as at on a.ArtifactID = at.ArtifactID


--DELETE them
IF (EXISTS (SELECT * 
				 FROM INFORMATION_SCHEMA.TABLES 
				 WHERE TABLE_SCHEMA = 'eddsdbo' 
				 AND  TABLE_NAME = 'ResourceFileData'))
BEGIN
	DELETE eddsdbo.ResourceFileData
	FROM eddsdbo.ResourceFileData 
	JOIN @agentResourcefilesIds ari ON ResourceFileData.ArtifactID = ari.ArtifactId
	WHERE ResourceFileData.ArtifactID = ari.ArtifactId
END

DELETE eddsdbo.ResourceFile
FROM eddsdbo.ResourceFile
JOIN @agentResourcefilesIds ari ON ResourceFile.ArtifactID = ari.ArtifactId
WHERE ResourceFile.ArtifactID = ari.ArtifactId

DELETE eddsdbo.ArtifactGuid
FROM eddsdbo.ArtifactGuid 
JOIN @agentResourcefilesIds ari ON ArtifactGuid.ArtifactID = ari.ArtifactId
WHERE ArtifactGuid.ArtifactID = ari.ArtifactId

DELETE eddsdbo.ArtifactAncestry
FROM eddsdbo.ArtifactAncestry 
JOIN @agentResourcefilesIds ari ON ArtifactAncestry.ArtifactID = ari.ArtifactId
WHERE ArtifactAncestry.ArtifactID = ari.ArtifactId

DELETE eddsdbo.Artifact
FROM eddsdbo.Artifact 
JOIN @agentResourcefilesIds ari ON Artifact.ArtifactID = ari.ArtifactId
WHERE Artifact.ArtifactID = ari.ArtifactId
