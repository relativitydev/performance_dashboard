SELECT CASE WHEN COUNT(*) > 0 THEN 1 ELSE 0 END AS 'ArtifactExists'
FROM [EDDS].eddsdbo.Artifact (nolock)
WHERE ArtifactID = @artifactId and DeleteFlag = 0