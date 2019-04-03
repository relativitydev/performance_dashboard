USE [master];

DECLARE @WorkspaceStatus TABLE(DBName NVARCHAR(255), VarscatInstalled bit, BullfrogInstalled bit)
INSERT INTO @WorkspaceStatus
EXEC sp_MSforeachdb
	'USE [?];
	DECLARE @scatInstalled bit = 0;
	DECLARE @frogInstalled bit = 0;
	IF (''?'' LIKE ''EDDS1%'') BEGIN
		IF EXISTS (SELECT name FROM sys.procedures where name = ''QoS_VARSCAT'')
			SET @scatInstalled = 1;
		IF EXISTS (SELECT name FROM sys.procedures where name = ''QoS_Bullfrog'')
			SET @frogInstalled = 1;
			
		SELECT ''?'', @scatInstalled, @frogInstalled;
	END;'

SELECT * FROM @WorkspaceStatus
ORDER BY VarscatInstalled & BullfrogInstalled ASC