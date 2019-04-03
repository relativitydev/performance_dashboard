USE EDDS

SELECT 
	[ResourceServer].[ArtifactID]
	,[ResourceServer].[Name]
	,[ResourceServer].[URL]
FROM [EDDS].[eddsdbo].[ResourceServer] with(nolock)
INNER JOIN eddsdbo.Code with(nolock) ON Code.ArtifactID = [ResourceServer].[Type]
WHERE Code.[Name] = 'SQL - Primary'