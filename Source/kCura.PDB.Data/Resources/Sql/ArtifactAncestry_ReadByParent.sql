SELECT ArtifactID FROM EDDS.eddsdbo.ArtifactAncestry with(nolock)
WHERE AncestorArtifactID = @artifactId