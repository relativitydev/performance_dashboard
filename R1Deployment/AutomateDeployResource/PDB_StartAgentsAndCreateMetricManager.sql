USE [EDDS]
GO

IF (EXISTS (SELECT name 
FROM master.dbo.sysdatabases 
WHERE ('[' + name + ']' = 'EDDS'
OR name = 'EDDS')))
BEGIN
	--- Start Agents
	SELECT * FROM EDDS.eddsdbo.Agent
	WHERE [Name] LIKE 'Performance Dashboard - %'

	UPDATE EDDS.eddsdbo.Agent
	SET [Enabled] = 1, Updated = 1
	WHERE [Name] LIKE 'Performance Dashboard - %'

	SELECT * FROM EDDS.eddsdbo.Agent
	WHERE [Name] LIKE 'Performance Dashboard - %'

	SELECT 'PDB agents successfully restarted.' [Message]
	
	--- Create Metric Manager agents
	/*
	--only parameter: dll with agents you want to create
	DECLARE @ResourceFile nvarchar(450) = 'kCura.PDB.Agent.dll';
	DECLARE @AgentTypeName nvarchar(450) = 'Performance Dashboard - Metric Manager'
	*/

	IF NOT EXISTS (SELECT 1 FROM EDDS.eddsdbo.Agent WHERE [Name] LIKE 'Performance Dashboard - Metric Manager%')
	BEGIN

		DECLARE @ResourceFile nvarchar(450) = 'kCura.PDB.Agent.dll';
		DECLARE @AgentTypeName nvarchar(450) = 'Performance Dashboard - Metric Manager'

		-- pick an active agent server to load the agent onto
		DECLARE @ServerforPDBAgent TABLE
		(
			ServerArtifactID INT,
			NumberOfAgents INT
		)

		INSERT INTO @ServerforPDBAgent (ServerArtifactID, NumberOfAgents)
			SELECT TOP 1 RS.ArtifactID, COUNT(1) AgentsOnServer
			  FROM [EDDS].[eddsdbo].ResourceServer RS
			  INNER JOIN [EDDS].[eddsdbo].[Agent] A ON RS.ArtifactID = A.ServerArtifactID
			  WHERE RS.[Type] IN (SELECT SystemArtifact.ArtifactID FROM [eddsdbo].[SystemArtifact] WHERE (SystemArtifact.SystemArtifactIdentifier = 'AgentResourceServerType'))
						  AND RS.[Status] IN (SELECT SystemArtifact.ArtifactID FROM [eddsdbo].[SystemArtifact] WHERE (SystemArtifact.SystemArtifactIdentifier = 'ActiveResourceStatus')) 
							AND RS.Name NOT IN --exclude servers where dtSearch Search agent lives
				(
					SELECT RS.Name ServerName
					  FROM [EDDS].[eddsdbo].[Agent] A
					  INNER JOIN eddsdbo.AgentType AT on A.AgentTypeArtifactID = AT.ArtifactID
					  INNER JOIN eddsdbo.ResourceServer RS on RS.ArtifactID = A.ServerArtifactID
					  WHERE AT.Name = 'dtSearch Search Agent'
				)
				GROUP BY RS.ArtifactID
				ORDER BY AgentsOnServer --order by number of agents to choose the server with least agents

		DECLARE @ResourceServerID int = (
			SELECT TOP 1 ServerArtifactID FROM @ServerforPDBAgent
		);

		IF @ResourceServerID IS NULL
		BEGIN
			SELECT TOP 1 @ResourceServerID = ResourceServer.ArtifactID
			FROM [eddsdbo].[ResourceServer]
			WHERE ResourceServer.[Type] IN (SELECT SystemArtifact.ArtifactID FROM [eddsdbo].[SystemArtifact] WHERE (SystemArtifact.SystemArtifactIdentifier = 'AgentResourceServerType'))
				  AND ResourceServer.[Status] IN (SELECT SystemArtifact.ArtifactID FROM [eddsdbo].[SystemArtifact] WHERE (SystemArtifact.SystemArtifactIdentifier = 'ActiveResourceStatus'));
		END

		-- temp table to hold the agent types from assembly
		DECLARE @AgentTypes TABLE (AgentTypeId int, [Name] nvarchar(50));
		INSERT INTO @AgentTypes
		SELECT AgentType.ArtifactID, AgentType.[Name]
		FROM eddsdbo.AgentType
		WHERE AgentType.ArtifactID IN (
			SELECT AssemblyAgentType.AgentTypeID
			FROM eddsdbo.AssemblyAgentType
			WHERE AssemblyAgentType.AssemblyArtifactID IN ( SELECT ResourceFile.ArtifactID FROM eddsdbo.ResourceFile WHERE ResourceFile.Name = @ResourceFile )
				AND AgentType.[Name] = @AgentTypeName
		);

		--pop and deal with each item off the stack
		WHILE (1 = 1) 
		BEGIN
			-- peek stack
			DECLARE @AgentTypeId int, @AgentName nvarchar(50);
			SELECT
				@AgentTypeId = AgentTypeId,
				@AgentName = [Name] + ' (1)'
			FROM @AgentTypes;
		
			--break if no more
			IF (@@ROWCOUNT = 0)
				BREAK;

			--insert to Artifact, Artifact, ArtifactAncestryTables
			INSERT INTO eddsdbo.Artifact (
				ArtifactTypeID,
				ParentArtifactID,
				AccessControlListID,
				AccessControlListIsInherited,
				CreatedOn,
				LastModifiedOn,
				LastModifiedBy,
				CreatedBy,
				TextIdentifier,
				ContainerID,
				Keywords,
				Notes,
				DeleteFlag
			)
			VALUES (
				20, 62, 1, 1, GETUTCDATE(), GETUTCDATE(), 9, 9, @AgentName, 62, '', '', 0
			);
			DECLARE @AgentID int = SCOPE_IDENTITY();

			INSERT INTO eddsdbo.ArtifactAncestry (ArtifactID, AncestorArtifactID)
			VALUES (@AgentID, 62);

			INSERT INTO eddsdbo.Agent (
				[Name],
				[Message],
				MessageTime,
				LastUpdate,
				MessageType,
				[Enabled],
				Interval,
				DetailMessage,
				ArtifactID,
				AgentTypeArtifactID,
				ServerArtifactID,
				Updated,
				[Safe],
				LoggingLevel
			)
			VALUES (
				@AgentName, '', GETUTCDATE(), GETUTCDATE(), '', 1, 5, '', @AgentID, @AgentTypeId, @ResourceServerID, 1, 1, 1
			);

			--pop stack
			DELETE FROM @AgentTypes
			WHERE AgentTypeId = @AgentTypeID;
		END;

		--with the data entries present, the Agent Manager will pick up and initialize the new agent automatically :tada:
	END
END
ELSE
BEGIN
	SELECT N'Unable to start agents, please run against EDDS (Primary SQL Server)'
END