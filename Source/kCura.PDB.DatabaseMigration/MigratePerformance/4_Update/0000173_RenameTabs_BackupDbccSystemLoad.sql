DECLARE @pdbTabId int;
SELECT TOP 1 @pdbTabId = ArtifactID
FROM [EDDS].[eddsdbo].[Tab]
WHERE Name = 'Performance Dashboard'

UPDATE EDDS.eddsdbo.Tab
SET Name = 'Recoverability/Integrity'
WHERE ArtifactID IN
(SELECT T.ArtifactID
	FROM EDDS.eddsdbo.ArtifactAncestry AA
	INNER JOIN EDDS.eddsdbo.Tab T
	ON AA.ArtifactID = T.ArtifactID
	WHERE AA.AncestorArtifactID = @pdbTabId
	AND T.Name = 'Backup and DBCC Checks')

UPDATE EDDS.eddsdbo.Tab
SET Name = 'Infrastructure Performance'
WHERE ArtifactID IN
(SELECT T.ArtifactID
	FROM EDDS.eddsdbo.ArtifactAncestry AA
	INNER JOIN EDDS.eddsdbo.Tab T
	ON AA.ArtifactID = T.ArtifactID
	WHERE AA.AncestorArtifactID = @pdbTabId
	AND T.Name = 'System Load')