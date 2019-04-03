USE [master];

EXEC sp_MSforeachdb
	'USE [?];
	IF (''?'' LIKE ''EDDS1%'') BEGIN
		IF EXISTS (SELECT name FROM sys.procedures where name = ''QoS_VARSCAT'')
			DROP PROCEDURE eddsdbo.QoS_VARSCAT;
		IF EXISTS (SELECT name FROM sys.procedures where name = ''QoS_kIE_VARSCAT'')
			DROP PROCEDURE eddsdbo.QoS_kIE_VARSCAT;
		IF EXISTS (SELECT name FROM sys.procedures where name = ''QoS_Bullfrog'')
			DROP PROCEDURE eddsdbo.QoS_Bullfrog;
		IF EXISTS(SELECT name FROM sys.tables WHERE name = ''RHScriptsRun'')
			DROP TABLE EDDSDBO.RHScriptsRun;
		IF EXISTS(SELECT name FROM sys.tables WHERE name = ''RHScriptsRunErrors'')
			DROP TABLE EDDSDBO.RHScriptsRunErrors;
		IF EXISTS(SELECT name FROM sys.tables WHERE name = ''RHVersion'')
			DROP TABLE EDDSDBO.RHVersion;
	END;'