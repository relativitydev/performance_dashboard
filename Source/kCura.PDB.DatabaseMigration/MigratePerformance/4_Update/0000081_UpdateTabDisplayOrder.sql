DECLARE @pdbTabId int;
SELECT TOP 1 @pdbTabId = ArtifactID
FROM [EDDS].[eddsdbo].[Tab]
WHERE Name = 'Performance Dashboard'

UPDATE EDDS.eddsdbo.Tab
SET DisplayOrder = 10100
WHERE ArtifactID IN
(SELECT T.ArtifactID
	FROM EDDS.eddsdbo.ArtifactAncestry AA
	INNER JOIN EDDS.eddsdbo.Tab T
	ON AA.ArtifactID = T.ArtifactID
	WHERE AA.AncestorArtifactID = @pdbTabId
	AND T.Name = 'Delivery Metrics')

UPDATE EDDS.eddsdbo.Tab
SET DisplayOrder = 10200
WHERE ArtifactID IN
(SELECT T.ArtifactID
	FROM EDDS.eddsdbo.ArtifactAncestry AA
	INNER JOIN EDDS.eddsdbo.Tab T
	ON AA.ArtifactID = T.ArtifactID
	WHERE AA.AncestorArtifactID = @pdbTabId
	AND T.Name = 'User Experience')

UPDATE EDDS.eddsdbo.Tab
SET DisplayOrder = 10300
WHERE ArtifactID IN
(SELECT T.ArtifactID
	FROM EDDS.eddsdbo.ArtifactAncestry AA
	INNER JOIN EDDS.eddsdbo.Tab T
	ON AA.ArtifactID = T.ArtifactID
	WHERE AA.AncestorArtifactID = @pdbTabId
	AND T.Name = 'System Load')

UPDATE EDDS.eddsdbo.Tab
SET DisplayOrder = 10400
WHERE ArtifactID IN
(SELECT T.ArtifactID
	FROM EDDS.eddsdbo.ArtifactAncestry AA
	INNER JOIN EDDS.eddsdbo.Tab T
	ON AA.ArtifactID = T.ArtifactID
	WHERE AA.AncestorArtifactID = @pdbTabId
	AND T.Name = 'Backup and DBCC Checks')

UPDATE EDDS.eddsdbo.Tab
SET DisplayOrder = 10500
WHERE ArtifactID IN
(SELECT T.ArtifactID
	FROM EDDS.eddsdbo.ArtifactAncestry AA
	INNER JOIN EDDS.eddsdbo.Tab T
	ON AA.ArtifactID = T.ArtifactID
	WHERE AA.AncestorArtifactID = @pdbTabId
	AND T.Name = 'Uptime')

UPDATE EDDS.eddsdbo.Tab
SET DisplayOrder = 10600
WHERE ArtifactID IN
(SELECT T.ArtifactID
	FROM EDDS.eddsdbo.ArtifactAncestry AA
	INNER JOIN EDDS.eddsdbo.Tab T
	ON AA.ArtifactID = T.ArtifactID
	WHERE AA.AncestorArtifactID = @pdbTabId
	AND T.Name = 'Application Performance')

UPDATE EDDS.eddsdbo.Tab
SET DisplayOrder = 10700
WHERE ArtifactID IN
(SELECT T.ArtifactID
	FROM EDDS.eddsdbo.ArtifactAncestry AA
	INNER JOIN EDDS.eddsdbo.Tab T
	ON AA.ArtifactID = T.ArtifactID
	WHERE AA.AncestorArtifactID = @pdbTabId
	AND T.Name = 'Server Health')
	
UPDATE EDDS.eddsdbo.Tab
SET DisplayOrder = 10800
WHERE ArtifactID IN
(SELECT T.ArtifactID
	FROM EDDS.eddsdbo.ArtifactAncestry AA
	INNER JOIN EDDS.eddsdbo.Tab T
	ON AA.ArtifactID = T.ArtifactID
	WHERE AA.AncestorArtifactID = @pdbTabId
	AND T.Name = 'Configuration')