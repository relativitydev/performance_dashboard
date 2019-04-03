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
--BEGIN
--SELECT * FROM EDDS.eddsdbo.AgentType at
--	JOIN @oldAgentGuids o ON at.Guid = o.agentGuid
--END

DECLARE @agentArtifactIds table(agentArtifactId int)
INSERT INTO @agentArtifactIds 
	SELECT ArtifactID FROM EDDS.eddsdbo.Agent a
		JOIN @agentTypeIds ati on a.AgentTypeArtifactID = ati.agentTypeId
SELECT * FROM @agentArtifactIds

--DECLARE @agentServerArtifactIds table(agentServerArtifactId int)
--INSERT INTO @agentServerArtifactIds
--	SELECT ServerArtifactID FROM EDDS.eddsdbo.Agent WHERE AgentTypeArtifactID = (SELECT * FROM @agentTypeIds)
--SELECT * FROM @agentServerArtifactIds

DECLARE @agentAssemblyArtifactIds table(agentAssemblyArtifactId int)
INSERT INTO @agentAssemblyArtifactIds
	SELECT AssemblyArtifactId FROM EDDS.eddsdbo.AssemblyAgentType aat
		JOIN @agentTypeIds ati ON aat.AgentTypeID = ati.agentTypeId
SELECT * FROM @agentAssemblyArtifactIds

--DELETE old Trust Agent
SELECT * FROM eddsdbo.Agent a
	JOIN @agentTypeIds ati ON a.AgentTypeArtifactID = ati.agentTypeId 
SELECT *  FROM eddsdbo.AssemblyAgentType aat
	JOIN @agentTypeIds ati ON aat.AgentTypeID = ati.agentTypeId -- Assembly IDs of the Agents
SELECT *  FROM eddsdbo.AssemblyAgentType aat 
	JOIN @agentAssemblyArtifactIds aaai ON aat.AssemblyArtifactID = aaai.agentAssemblyArtifactId -- Agents in the Agent Assemblies

SELECT *  FROM eddsdbo.CaseApplicationAgentType caat
	JOIN @agentTypeIds ati ON caat.AgentTypeID = ati.agentTypeId
SELECT *  FROM eddsdbo.AgentType a
	JOIN @agentTypeIds ati ON a.ArtifactID = ati.agentTypeId
--SELECT *  FROM eddsdbo.ArtifactGuid ag JOIN @oldAgentGuids oag ON ag.ArtifactGuid = oag.agentGuid
SELECT *  FROM eddsdbo.ArtifactGuid ag
	JOIN @agentTypeIds ati ON ag.ArtifactID = ati.agentTypeId
SELECT *  FROM eddsdbo.ArtifactAncestry aa
	JOIN @agentTypeIds ati ON aa.ArtifactID = ati.agentTypeId
SELECT *  FROM eddsdbo.Artifact a
	JOIN @agentTypeIds ati ON a.ArtifactID = ati.agentTypeId

	
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
SELECT *  FROM eddsdbo.AssemblyEventHandler aeh
	JOIN @agentResourcefilesIds ari ON aeh.AssemblyArtifactID = ari.ArtifactId

--DELETE them ONLY if there are no more agents associated with that assembly
IF (EXISTS (SELECT *  FROM eddsdbo.AssemblyAgentType aat
	JOIN @agentAssemblyArtifactIds aaai ON aat.AssemblyArtifactID = aaai.agentAssemblyArtifactId)
OR EXISTS(SELECT *  FROM eddsdbo.AssemblyEventHandler aeh
	JOIN @agentAssemblyArtifactIds aaai ON aeh.AssemblyArtifactID = aaai.agentAssemblyArtifactId))
BEGIN
	SELECT * FROM (values('TRUE')) AS TempData2([AssemblyAgentsExist])
END

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'eddsdbo' 
                 AND  TABLE_NAME = 'ResourceFileData'))
BEGIN
	SELECT *  FROM eddsdbo.ResourceFileData rfd
		JOIN @agentResourcefilesIds ari ON rfd.ArtifactID = ari.ArtifactId
END
SELECT *  FROM eddsdbo.ResourceFile rf 
	JOIN @agentResourcefilesIds ari ON rf.ArtifactID = ari.ArtifactId
SELECT *  FROM eddsdbo.ArtifactAncestry aa
	JOIN @agentResourcefilesIds ari ON aa.ArtifactID = ari.ArtifactId
SELECT *  FROM eddsdbo.Artifact a
	JOIN @agentResourcefilesIds ari ON a.ArtifactID = ari.ArtifactId