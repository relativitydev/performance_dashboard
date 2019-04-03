SELECT ArtifactID, [SearchText], Name
FROM [EDDSDBO].[View] WITH(NOLOCK)
WHERE [ArtifactID] = @searchArtifactId