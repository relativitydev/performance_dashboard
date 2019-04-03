

INSERT INTO [eddsdbo].[Server]
           ([ServerName]
           ,[CreatedOn]
           ,[DeletedOn]
           ,[ServerTypeID]
           ,[ServerIPAddress]
           ,[IgnoreServer]
           ,[ResponsibleAgent]
           ,[ArtifactID]
           ,[LastChecked]
           ,[UptimeMonitoringResourceHost]
           ,[UptimeMonitoringResourceUseHttps]
           ,[LastServerBackup]
           ,[AdminScriptsVersion]
		   ,[IsQoSDeployed])
     VALUES
           (@serverName
			,@createdOn
			,@deletedOn
			,@serverTypeID
			,@serverIPAddress
			,@ignoreServer
			,@responsibleAgent
			,@artifactID
			,@lastChecked
			,@uptimeMonitoringResourceHost
			,@uptimeMonitoringResourceUseHttps
			,@lastServerBackup
			,@adminScriptsVersion
			,0)

SELECT * FROM [eddsdbo].[Server] WHERE ServerID = @@IDENTITY