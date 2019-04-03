USE [EDDS]

DECLARE @credentialCodeTypeID INT = 10,
@parentArtifactID INT,
@userID AS INT,
@aclID INT,
@artifactID INT

BEGIN TRANSACTION
-- Get the root node's ArtifactID for parent, and user and ACLID
SELECT TOP (1)
	@parentArtifactID = ArtifactID,
	@userID = CreatedBy,
	@aclID = AccessControlListID
FROM
	EDDSDBO.Artifact
WHERE
	ParentArtifactID IS NULL AND ArtifactTypeID = 1


IF NOT EXISTS(SELECT * FROM [EDDSDBO].[Code] WHERE CodeTypeID = @credentialCodeTypeID AND Name = 'PerformanceDashboardTrustSetting')
	BEGIN
	-- Create a new Artifact of Code type called PerformanceDashboardTrustSetting - @artifactId is the Code's ID
	INSERT INTO [EDDSDBO].[Artifact] (
		[ArtifactTypeID],[ParentArtifactID],[AccessControlListID],[AccessControlListIsInherited],
		[CreatedOn],[LastModifiedOn],[LastModifiedBy],[CreatedBy],
		[ContainerID],[Keywords],[Notes],[DeleteFlag],[TextIdentifier]
	) VALUES (
		7, @parentArtifactID, @aclID, 1,
		GETUTCDATE(), GETUTCDATE(), @userID, @userID,
		@parentArtifactID, '',  '', 0, 'PerformanceDashboardTrustSetting'
	);
	SET @artifactID = SCOPE_IDENTITY();

	INSERT INTO [EDDSDBO].ArtifactAncestry (ArtifactID, AncestorArtifactID) VALUES (@artifactID, @parentArtifactID)

	INSERT INTO [EDDSDBO].[Code] ([ArtifactID], [CodeTypeID], [Name], [Order], [IsActive], [UpdateInSearchEngine]) 
	VALUES (@artifactID, @credentialCodeTypeID, 'PerformanceDashboardTrustSetting', 0, 1, 0);
END

SET @artifactID = (SELECT TOP 1 [ArtifactID] FROM [EDDSDBO].[Code] WHERE CodeTypeID = @credentialCodeTypeID AND Name = 'PerformanceDashboardTrustSetting')
IF NOT EXISTS(SELECT TOP 1 [ArtifactID] FROM [EDDSDBO].[ArtifactGuid] WHERE [ArtifactID] = @artifactID)
  INSERT INTO EDDSDBO.[ArtifactGuid] ([ArtifactID], [ArtifactGuid])
  VALUES (@artifactID, 'B231AE06-6647-4A9E-815C-430D3DC0E53A') -- PerformanceDashboardTrustSetting
  
SET @artifactID = (SELECT TOP 1 [ArtifactID] FROM [EDDSDBO].[Code] WHERE CodeTypeID = @credentialCodeTypeID AND Name = 'PerformanceDashboardTrustSetting')
IF NOT EXISTS(Select TOP 1 [ArtifactID] FROM [EDDSDBO].[SystemArtifact] WHERE SystemArtifactIdentifier = 'PerformanceDashboardTrustSetting')
	INSERT INTO [EDDSDBO].[SystemArtifact]([ArtifactID], [SystemArtifactIdentifier])
	VALUES (@artifactID, 'PerformanceDashboardTrustSetting')
	
COMMIT TRANSACTION