USE [master];

EXEC sp_MSforeachdb
	'USE [?]; 
	IF EXISTS (SELECT * FROM sys.tables WHERE name = ''ApplicationInstallEventHandlerRunOnce'')
		DELETE FROM [eddsdbo].[ApplicationInstallEventHandlerRunOnce]
		WHERE InstallEventHandlerGuid = ''2BB0289C-05F4-4D10-95F8-B5126DFEFF80'';'