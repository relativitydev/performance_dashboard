insert into [eddsdbo].[Artifact] 
	([ArtifactTypeID],
	[ParentArtifactID],
	[AccessControlListID],
	[AccessControlListIsInherited],
	[CreatedBy],
	[LastModifiedBy],
	[CreatedOn],
	[LastModifiedOn],
	[TextIdentifier],
	[ContainerID],
	[Keywords],
	[Notes],
	[DeleteFlag]) 
values
	(@artifactTypeId, -- ArtifactTypeID
	@parentArtifactID, -- ParentArtifactID
	1, -- AccessControlListID
	1, -- AccessControlListIsInherited
	@createdBy, -- CreatedBy
	@createdBy, -- LastModifiedBy
	GETDATE(), -- CreatedOn
	GETDATE(), -- LastModifiedOn
	@name, -- TextIdentifier
	@containerId, -- ContainerID
	@keywords, -- Keywords
	@notes, -- Notes
	0) -- DeleteFlag

SELECT @@IDENTITY