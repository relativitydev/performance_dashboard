/*
	Apply System Administrator group permissions to Performance Dashboard tabs in 8.2
*/

USE [EDDS];

--GroupTab only exists in 8.2+
IF EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'GroupTab') 
BEGIN
  --Declarations
  DECLARE
	@systemAdminGroupId int,
	@pdbParentTabId int;

  --Determine PDB parent tab artifact ID and apply permissions to it and all child tabs
  SET @systemAdminGroupId =
	(SELECT TOP 1 ArtifactID
	FROM [EDDS].[eddsdbo].[Group]
	WHERE Name = 'System Administrators')
  SET @pdbParentTabId = (SELECT TOP 1 ArtifactId FROM [EDDS].[eddsdbo].[Tab] WHERE Name='Performance Dashboard')

  INSERT INTO [EDDS].[eddsdbo].[GroupTab] (GroupArtifactID, TabArtifactID)
  SELECT @systemAdminGroupId, T.ArtifactID
  FROM [EDDS].[eddsdbo].[Tab] T
  LEFT JOIN [EDDS].[eddsdbo].[GroupTab] GT
  ON T.ArtifactID = GT.TabArtifactID
  WHERE GT.TabArtifactID IS NULL AND
  (T.ArtifactID = @pdbParentTabId OR
  T.ArtifactID IN
	(SELECT ArtifactID FROM [EDDS].[eddsdbo].[ArtifactAncestry] WHERE AncestorArtifactID = @pdbParentTabId));
END