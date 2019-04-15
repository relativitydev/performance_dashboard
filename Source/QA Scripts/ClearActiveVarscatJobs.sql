EXEC sp_MSforeachdb
	'USE [?];
	IF (''?'' LIKE ''EDDS%'') BEGIN
		IF EXISTS (SELECT name FROM sys.tables WHERE name = ''VarscatRunHistory'')
		BEGIN
			UPDATE eddsdbo.VarscatRunHistory SET IsActive = 0
		END
	END;'