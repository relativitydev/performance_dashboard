IF (EXISTS (SELECT name 
FROM master.dbo.sysdatabases 
WHERE ('[' + name + ']' = 'EDDS'
OR name = 'EDDS')))
BEGIN
	--- Stop Agents
	SELECT * FROM EDDS.eddsdbo.Agent
	WHERE [Name] LIKE 'Performance Dashboard - %'

	UPDATE EDDS.eddsdbo.Agent
	SET [Enabled] = 0, Updated = 1
	WHERE [Name] LIKE 'Performance Dashboard - %'

	SELECT * FROM EDDS.eddsdbo.Agent
	WHERE [Name] LIKE 'Performance Dashboard - %'

	WHILE EXISTS 
	(SELECT 1 FROM EDDS.eddsdbo.Agent
	WHERE [Name] LIKE 'Performance Dashboard - %'
	AND Updated = 1 
	--AND [Message] <> N'Completed.'
	)
	BEGIN
	IF EXISTS (SELECT 1 FROM EDDS.eddsdbo.Agent WHERE [Name] LIKE 'Performance Dashboard - %' AND [Enabled] = 1)
	BEGIN
	SELECT 'Agents that have been re-enabled unexpectedly and will be disabled again.' [Message]
	SELECT * FROM EDDS.eddsdbo.Agent WHERE [Name] LIKE 'Performance Dashboard - %' AND [Enabled] = 1

	UPDATE EDDS.eddsdbo.Agent
	SET [Enabled] = 0, Updated = 1
	WHERE [Name] LIKE 'Performance Dashboard - %'
	END

	PRINT 'Waiting for agents to complete work and come to a stop.'
	WAITFOR DELAY '00:00:10'
	END

	SELECT 'PDB agents successfully stopped.' [Message]
END
ELSE
BEGIN
	SELECT N'Unable to stop agents, please run against EDDS (Primary SQL Server)'
END