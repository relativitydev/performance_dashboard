ALTER TABLE EDDSPerformance.EDDSDBO.[server] ADD ArtifactID int

GO

UPDATE s
SET s.ArtifactID = rs.artifactID
FROM EDDS.EDDSDBO.ResourceServer rs
INNER JOIN EDDSPerformance.EDDSDBO.[server] s
ON s.ServerName = rs.Name 