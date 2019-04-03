USE EDDS
--Gather all old Resource Files
DECLARE @applicationGuid UNIQUEIDENTIFIER = '60a1d0a3-2797-4fb3-a260-614cbfd3fa0d'
DECLARE @oldResourcefilesIds table(ArtifactId int);
INSERT @oldResourcefilesIds(ArtifactId) 
	SELECT ArtifactId 
	FROM eddsdbo.ResourceFile 
	WHERE ApplicationGuid = @applicationGuid 
	AND ([Name] = 'kCura.PDB.Agent.Package.dll'
	OR [Name] = 'kCura.PDB.Agent.Trust.dll')

--DELETE them
IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'eddsdbo' 
                 AND  TABLE_NAME = 'ResourceFileData'))
BEGIN
	DELETE eddsdbo.ResourceFileData
	FROM eddsdbo.ResourceFileData 
	JOIN @oldResourcefilesIds orfi ON ResourceFileData.ArtifactID = orfi.ArtifactId
	WHERE ResourceFileData.ArtifactID = orfi.ArtifactId
END

DELETE eddsdbo.ResourceFile
FROM eddsdbo.ResourceFile 
JOIN @oldResourcefilesIds orfi ON ResourceFile.ArtifactID = orfi.ArtifactId
WHERE ResourceFile.ArtifactID = orfi.ArtifactId

DELETE eddsdbo.ArtifactGuid
FROM eddsdbo.ArtifactGuid 
JOIN @oldResourcefilesIds orfi ON ArtifactGuid.ArtifactID = orfi.ArtifactId
WHERE ArtifactGuid.ArtifactID = orfi.ArtifactId

DELETE eddsdbo.ArtifactAncestry
FROM eddsdbo.ArtifactAncestry 
JOIN @oldResourcefilesIds orfi ON ArtifactAncestry.ArtifactID = orfi.ArtifactId
WHERE ArtifactAncestry.ArtifactID = orfi.ArtifactId

DELETE eddsdbo.Artifact
FROM eddsdbo.Artifact 
JOIN @oldResourcefilesIds orfi ON Artifact.ArtifactID = orfi.ArtifactId
WHERE Artifact.ArtifactID = orfi.ArtifactId