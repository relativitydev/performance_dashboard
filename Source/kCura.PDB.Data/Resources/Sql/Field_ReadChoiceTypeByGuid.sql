select f.CodeTypeID
from eddsdbo.Field f
inner join eddsdbo.ArtifactGuid ag on ag.ArtifactId = f.ArtifactId
where ag.ArtifactGuid = @artifactGuid