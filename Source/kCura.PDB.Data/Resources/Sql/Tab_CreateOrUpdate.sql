USE EDDS

IF NOT EXISTS (
	select * FROM [EDDS].[eddsdbo].[Tab] t with(nolock)
	inner join eddsdbo.Artifact a on a.ArtifactID = t.ArtifactID and ParentArtifactID = @parentArtifactID and a.ArtifactID = @artifactID
	)
BEGIN
	insert into [EDDS].[eddsdbo].[Tab]
		([ArtifactID], [Name], [DisplayOrder], [ObjectArtifactTypeID], [ExternalLink], [IsDefault], [TabDisplay], [Visible]) 
	values
		(@artifactId, @name, @displayOrder, NULL, @externalLink, 0, 0, 1)
END
ELSE
BEGIN
	update [EDDS].[eddsdbo].[Tab]
	set	[DisplayOrder] = @displayOrder
		,[ExternalLink] = @externalLink 
	where ArtifactId = @artifactId
END