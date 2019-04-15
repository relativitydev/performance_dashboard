USE EDDS;

-- pick an active agent server to load the agent onto
DECLARE @ResourceServerID int = (
    SELECT ResourceServer.ArtifactID
    FROM [eddsdbo].[ResourceServer]
    WHERE ResourceServer.[Type] IN (SELECT SystemArtifact.ArtifactID FROM [eddsdbo].[SystemArtifact] WHERE (SystemArtifact.SystemArtifactIdentifier = 'AgentResourceServerType'))
          AND ResourceServer.[Status] IN (SELECT SystemArtifact.ArtifactID FROM [eddsdbo].[SystemArtifact] WHERE (SystemArtifact.SystemArtifactIdentifier = 'ActiveResourceStatus'))
);

-- temp table to hold the agent types from assembly
DECLARE @AgentTypes TABLE (AgentTypeId int, [Name] nvarchar(50));
INSERT INTO @AgentTypes
	SELECT AgentType.ArtifactID, AgentType.[Name]
	FROM eddsdbo.AgentType
	WHERE AgentType.Guid in (@AgentGuids);


--pop and deal with each item off the stack
WHILE (1 = 1) 
BEGIN
    -- peek stack
    DECLARE @AgentTypeId int, @AgentName nvarchar(50);
    SELECT
        @AgentTypeId = AgentTypeId,
        @AgentName = '[Integration] ' + [Name]
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