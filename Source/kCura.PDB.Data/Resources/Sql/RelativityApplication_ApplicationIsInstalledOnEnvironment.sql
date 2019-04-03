SELECT CASE WHEN Count(*) > 0 THEN 1 ELSE 0 END AS 'ApplicationIsInstalled'
FROM [EDDS].eddsdbo.ArtifactGuid
JOIN [EDDS].[eddsdbo].CaseApplication ON CaseApplication.ApplicationID = ArtifactGuid.ArtifactID
WHERE ArtifactGuid = @applicationGuid