SELECT TOP 1 ServerID
FROM EDDS.eddsdbo.[Case] WITH(NOLOCK)
WHERE ArtifactID = @CaseArtifactID