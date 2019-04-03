USE EDDS

--only parameter: dll with agents you want to lookup
DECLARE @ResourceFile nvarchar(450) = 'kCura.PDB.Agent.dll';

-- temp table to hold the agent types from assembly
SELECT Name, Message, DetailMessage
FROM eddsdbo.Agent
WHERE AgentTypeArtifactID IN (
	SELECT AgentType.ArtifactID
	FROM eddsdbo.AgentType
	WHERE Enabled = 0
	AND AgentType.ArtifactID IN (
	    SELECT AssemblyAgentType.AgentTypeID
	    FROM eddsdbo.AssemblyAgentType
	    WHERE AssemblyAgentType.AssemblyArtifactID IN ( SELECT ResourceFile.ArtifactID FROM eddsdbo.ResourceFile WHERE ResourceFile.Name = @ResourceFile )
));