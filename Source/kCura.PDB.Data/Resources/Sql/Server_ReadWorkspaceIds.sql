SELECT c.ArtifactID
FROM [EDDSPerformance].[eddsdbo].[Server] s with(nolock)
inner join [EDDS].[eddsdbo].[Case] c with(nolock) on s.ArtifactID = c.ServerID
inner join [EDDS].[eddsdbo].[Artifact] a with(nolock) on c.ArtifactID = a.ArtifactID
inner join [EDDS].[eddsdbo].[WorkspaceUpgradeStatus] us with(nolock) on c.ArtifactID = us.ArtifactID
WHERE s.[ServerID] = @serverId -- on this specific server
AND a.DeleteFlag <> 1 -- Not deleted
AND c.ArtifactID <> -1 -- Not admin case
AND us.[Status] = 5 -- Completed workspace upgrade