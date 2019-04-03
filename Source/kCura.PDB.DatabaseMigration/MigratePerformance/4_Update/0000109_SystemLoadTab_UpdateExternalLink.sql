DECLARE @pdbTabId int;
SELECT TOP 1 @pdbTabId = ArtifactID
FROM [EDDS].[eddsdbo].[Tab]
WHERE Name = 'Performance Dashboard'

UPDATE EDDS.eddsdbo.Tab
SET ExternalLink = '%ApplicationPath%/CustomPages/60a1d0a3-2797-4fb3-a260-614cbfd3fa0d/SystemLoadServer.aspx?StandardsCompliance=true'
WHERE ArtifactID IN
(SELECT T.ArtifactID
	FROM EDDS.eddsdbo.ArtifactAncestry AA
	INNER JOIN EDDS.eddsdbo.Tab T
	ON AA.ArtifactID = T.ArtifactID
	WHERE AA.AncestorArtifactID = @pdbTabId
	AND T.Name = 'System Load')
