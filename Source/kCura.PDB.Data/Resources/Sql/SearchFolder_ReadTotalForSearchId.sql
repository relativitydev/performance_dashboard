SELECT COUNT(folderArtifactID)
FROM eddsdbo.SearchFolder WITH(NOLOCK)
WHERE @searchArtifactId = SearchFolder.SearchArtifactID