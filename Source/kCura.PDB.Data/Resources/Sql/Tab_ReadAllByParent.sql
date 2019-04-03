SELECT T.Name, T.ArtifactId, T.DisplayOrder, T.ExternalLink, A.AncestorArtifactID as ParentArtifactId
FROM eddsdbo.Tab T WITH(NOLOCK)
INNER JOIN eddsdbo.ArtifactAncestry A WITH(NOLOCK)
ON T.ArtifactID = A.ArtifactID
WHERE A.AncestorArtifactID = @artifactId