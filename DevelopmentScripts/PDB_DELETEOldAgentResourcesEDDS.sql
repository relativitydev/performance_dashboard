USE EDDS

DECLARE @oldAgentGuids table(agentGuid UNIQUEIDENTIFIER)
INSERT @oldAgentGuids(agentGuid) 
	SELECT * FROM (
		values('514D019C-6BDC-40FF-A2E8-938B7C5E43B1')
		,('FDC70DC6-9DFB-4F8E-8D8C-3C7CFF515A47')
		,('5177338F-9B85-4AC5-A18A-BFAA98AF1804')
		,('5C475C47-90F0-407B-828F-83A37E29F2F8')
		,('93DCA58D-2E8E-4EF2-BB31-CA7F4354E91C')
		)
		AS TempData([agentGuid])
--SELECT * from @oldAgentGuids

DECLARE @applicationGuid UNIQUEIDENTIFIER = '60a1d0a3-2797-4fb3-a260-614cbfd3fa0d'

DECLARE @agentTypeIds table(agentTypeId int)
INSERT INTO @agentTypeIds
	SELECT ArtifactID FROM EDDS.eddsdbo.AgentType at
		JOIN @oldAgentGuids o ON at.Guid = o.agentGuid
SELECT * FROM @agentTypeIds

DECLARE @agentArtifactIds table(agentArtifactId int)
INSERT INTO @agentArtifactIds 
	SELECT ArtifactID FROM EDDS.eddsdbo.Agent a
		JOIN @agentTypeIds ati ON a.AgentTypeArtifactID = ati.agentTypeId
SELECT * FROM @agentArtifactIds

DECLARE @agentAssemblyArtifactIds table(agentAssemblyArtifactId int)
INSERT INTO @agentAssemblyArtifactIds
	SELECT AssemblyArtifactId FROM EDDS.eddsdbo.AssemblyAgentType aat
		JOIN @agentTypeIds ati ON aat.AgentTypeID = ati.agentTypeId
SELECT * FROM @agentAssemblyArtifactIds

--DELETE old Trust Agent
DELETE eddsdbo.Agent 
FROM eddsdbo.Agent 
JOIN @agentTypeIds ati ON Agent.AgentTypeArtifactID = ati.agentTypeId
WHERE Agent.AgentTypeArtifactID = ati.agentTypeId

DELETE eddsdbo.AssemblyAgentType
FROM eddsdbo.AssemblyAgentType
JOIN @agentTypeIds ati ON AssemblyAgentType.AgentTypeID = ati.agentTypeId
WHERE AssemblyAgentType.AgentTypeID = ati.agentTypeId

DELETE eddsdbo.CaseApplicationAgentType
FROM eddsdbo.CaseApplicationAgentType
JOIN @agentTypeIds ati ON CaseApplicationAgentType.AgentTypeID = ati.agentTypeId
WHERE CaseApplicationAgentType.AgentTypeID = ati.agentTypeId

DELETE eddsdbo.AgentType
FROM eddsdbo.AgentType
JOIN @agentTypeIds ati ON AgentType.ArtifactID = ati.agentTypeId
WHERE AgentType.ArtifactID = ati.agentTypeId

DELETE eddsdbo.ArtifactGuid
FROM eddsdbo.ArtifactGuid
JOIN @agentTypeIds ati ON ArtifactGuid.ArtifactID = ati.agentTypeId
WHERE ArtifactGuid.ArtifactID = ati.agentTypeId

DELETE eddsdbo.ArtifactAncestry
FROM eddsdbo.ArtifactAncestry
JOIN @agentTypeIds ati ON ArtifactAncestry.ArtifactID = ati.agentTypeId
WHERE ArtifactAncestry.ArtifactID = ati.agentTypeId

DELETE eddsdbo.Artifact
FROM eddsdbo.Artifact
JOIN @agentTypeIds ati ON Artifact.ArtifactID = ati.agentTypeId
WHERE Artifact.ArtifactID = ati.agentTypeId

--Gather all old Trust Agent Resource Files
DECLARE @agentResourcefilesIds table(ArtifactId int);
 -- This requires the agent to exist already before we can delete the resource files? See: @agentArtifactId
INSERT @agentResourcefilesIds(ArtifactId) 
	SELECT ArtifactId FROM eddsdbo.ResourceFile r
		JOIN @agentAssemblyArtifactIds aaai ON r.ArtifactID = aaai.agentAssemblyArtifactId
		--JOIN @agentServerArtifactIds asai ON r.ArtifactID = asai.agentServerArtifactId
		WHERE ApplicationGuid = @applicationGuid
-- Explicitly delete potentially orphaned resource files as well
INSERT @agentResourcefilesIds(ArtifactId) 
	SELECT ArtifactId FROM eddsdbo.ResourceFile r
		WHERE ApplicationGuid = @applicationGuid 
			AND ([Name] = 'kCura.PDB.Agent.Package.dll'
			OR [Name] = 'kCura.PDB.Agent.Trust.dll')
			AND NOT EXISTS( SELECT * FROM @agentResourcefilesIds AS ari WHERE ari.ArtifactId = r.ArtifactID )
SELECT * FROM @agentResourcefilesIds

-- Remove any event handlers currently attached to these files to be removed
DELETE eddsdbo.AssemblyEventHandler
FROM eddsdbo.AssemblyEventHandler
JOIN @agentResourcefilesIds ari ON AssemblyEventHandler.AssemblyArtifactID = ari.ArtifactId
WHERE AssemblyEventHandler.AssemblyArtifactID = ari.ArtifactId

--DELETE them ONLY if there are no more agents associated with that assembly
IF (NOT EXISTS (SELECT *  FROM eddsdbo.AssemblyAgentType aat
	JOIN @agentAssemblyArtifactIds aaai ON aat.AssemblyArtifactID = aaai.agentAssemblyArtifactId))
BEGIN
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

	DELETE eddsdbo.ArtifactAncestry
	FROM eddsdbo.ArtifactAncestry
	JOIN @agentResourcefilesIds ari ON ArtifactAncestry.ArtifactID = ari.ArtifactId
	WHERE ArtifactAncestry.ArtifactID = ari.ArtifactId
	
	DELETE eddsdbo.Artifact
	FROM eddsdbo.Artifact
	JOIN @agentResourcefilesIds ari ON Artifact.ArtifactID = ari.ArtifactId
	WHERE Artifact.ArtifactID = ari.ArtifactId
END