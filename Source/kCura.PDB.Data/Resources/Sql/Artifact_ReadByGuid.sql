Select a.ArtifactID
from eddsdbo.artifact a with(nolock)
inner join eddsdbo.artifactGuid ag with(nolock) on ag.ArtifactID = a.ArtifactID
where ag.ArtifactGuid = @artifactGuid