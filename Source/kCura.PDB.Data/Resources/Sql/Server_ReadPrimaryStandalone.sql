-- Returns either the ArtifactID of the Primary SQL server that has no associated Cases (Workspaces), or NULL

SELECT s.ArtifactID
FROM EDDS.eddsdbo.[Case] c WITH(NOLOCK)
RIGHT JOIN EDDS.eddsdbo.[ExtendedResourceServer] s WITH(NOLOCK)
	ON c.ServerID = s.ArtifactID
WHERE Type = 'SQL - Primary'
	AND c.ArtifactID is null