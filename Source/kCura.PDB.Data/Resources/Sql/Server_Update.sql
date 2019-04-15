

UPDATE [eddsdbo].[Server]
   SET [ServerName] = @serverName
      ,[CreatedOn] = @createdOn
      ,[DeletedOn] = @deletedOn
      ,[ServerTypeID] = @serverTypeID
      ,[ServerIPAddress] = @serverIPAddress
      ,[IgnoreServer] = @ignoreServer
      ,[ResponsibleAgent] = @responsibleAgent
      ,[ArtifactID] = @artifactID
      ,[LastChecked] = @lastChecked
      ,[UptimeMonitoringResourceHost] = @uptimeMonitoringResourceHost
      ,[UptimeMonitoringResourceUseHttps] = @uptimeMonitoringResourceUseHttps
      ,[LastServerBackup] = @lastServerBackup
      ,[AdminScriptsVersion] = @adminScriptsVersion
	  ,[IsQoSDeployed] = @isQoSDeployed
WHERE
	ServerID = @serverID

