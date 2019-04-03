SELECT [Version]
FROM [EDDS].eddsdbo.ArtifactGuid WITH(NOLOCK)
JOIN [EDDS].[eddsdbo].CaseApplication WITH(NOLOCK) ON CaseApplication.ApplicationID = ArtifactGuid.ArtifactID
WHERE ArtifactGuid = @applicationGuid AND CaseApplication.CaseID = @caseArtifactId