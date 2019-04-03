INSERT INTO [eddsdbo].ArtifactAncestry
	(AncestorArtifactID, ArtifactID)
VALUES
	(@parentArtifactId, @artifactId)
	
INSERT INTO [eddsdbo].ArtifactAncestry
	(AncestorArtifactID, ArtifactID) 
SELECT AncestorArtifactID, @artifactId
FROM [eddsdbo].ArtifactAncestry
WHERE ArtifactID = @parentArtifactId