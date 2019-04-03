update [EDDS].[eddsdbo].[Tab]
set	[DisplayOrder] = @displayOrder
	,[ExternalLink] = @externalLink 
where ArtifactId = @artifactId