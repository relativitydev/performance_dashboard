--Use the configured MDF/LDF paths (or the default if these are missing or empty)
IF (ISNULL(@serverName, '') = '')
	SET @serverName = @@SERVERNAME;

DECLARE @eddsFilePath nvarchar(MAX) = (SELECT TOP 1 [physical_name] FROM sys.master_files WHERE [name] = 'EDDS');

DECLARE @eddsDirPath nvarchar(MAX) = REPLACE(@eddsFilePath, 'Edds.mdf', '');
DECLARE @configDirPath nvarchar(MAX) = (SELECT TOP 1 [Value] FROM [EDDS].[eddsdbo].[Configuration]
						WHERE Section = 'kCura.EDDS.SQLServer'
						AND Name = 'DataDirectory'
						AND (MachineName = @serverName OR MachineName = '')
						ORDER BY MachineName DESC);
DECLARE @configLogDirPath nvarchar(MAX) = (SELECT TOP 1 [Value] FROM [EDDS].[eddsdbo].[Configuration]
						WHERE Section = 'kCura.EDDS.SQLServer'
						AND Name = 'LDFDirectory'
						AND (MachineName = @serverName OR MachineName = '')
						ORDER BY MachineName DESC);
					  
SELECT @serverName AS ServerName, COALESCE(NULLIF(@configDirPath, ''), @eddsDirPath) AS MDF, COALESCE(NULLIF(@configLogDirPath, ''), @eddsDirPath) AS LDF