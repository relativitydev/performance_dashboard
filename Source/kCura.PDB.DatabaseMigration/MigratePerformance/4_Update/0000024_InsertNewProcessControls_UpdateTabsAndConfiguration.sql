USE [EDDS]
UPDATE [eddsdbo].[Tab]
  SET ExternalLink = '%ApplicationPath%/CustomPages/60a1d0a3-2797-4fb3-a260-614cbfd3fa0d/ApplicationPerformance.aspx'
  WHERE Name = 'Application Performance'
UPDATE [eddsdbo].[Tab]
  SET ExternalLink = '%ApplicationPath%/CustomPages/60a1d0a3-2797-4fb3-a260-614cbfd3fa0d/ServerHealth.aspx'
  WHERE Name = 'Server Health'

USE [EDDSPerformance]
IF NOT EXISTS (SELECT * FROM [eddsdbo].[ProcessControl] WHERE ProcessControlID = 5)
	INSERT INTO [eddsdbo].[ProcessControl] ([ProcessControlID], [ProcessTypeDesc], [LastProcessExecDateTime], [Frequency]) VALUES (5, N'Install Server Scripts', DATEADD(HOUR, DATEDIFF(HH, 0, GETUTCDATE()), 0), 60)
IF NOT EXISTS (SELECT * FROM [eddsdbo].[ProcessControl] WHERE ProcessControlID = 6)
	INSERT INTO [eddsdbo].[ProcessControl] ([ProcessControlID], [ProcessTypeDesc], [LastProcessExecDateTime], [Frequency]) VALUES (6, N'Install Workspace Scripts', DATEADD(HOUR, DATEDIFF(HH, 0, GETUTCDATE()), 0), 60)
IF NOT EXISTS (SELECT * FROM [eddsdbo].[ProcessControl] WHERE ProcessControlID = 7)
	INSERT INTO [eddsdbo].[ProcessControl] ([ProcessControlID], [ProcessTypeDesc], [LastProcessExecDateTime], [Frequency]) VALUES (7, N'Run Looking Glass', DATEADD(HOUR, DATEDIFF(HH, 0, GETUTCDATE()), 0), 60)

IF NOT EXISTS (SELECT * FROM [EDDSPerformance].[eddsdbo].[Configuration]
	WHERE Section='kCura.PDB' AND Name = 'RollupAgent')
BEGIN
	INSERT INTO [EDDSPerformance].[eddsdbo].[Configuration]
		(Section, Name, Value, MachineName, Description)
	VALUES ('kCura.PDB', 'RollupAgent', '', '', '')
END