SELECT T.ArtifactId, A.AncestorArtifactID as ParentArtifactId, T.Name, T.DisplayOrder, T.ExternalLink
FROM eddsdbo.Tab T WITH(NOLOCK)
INNER JOIN eddsdbo.ArtifactAncestry A WITH(NOLOCK)
ON T.ArtifactID = A.ArtifactID
WHERE Name = @tabName