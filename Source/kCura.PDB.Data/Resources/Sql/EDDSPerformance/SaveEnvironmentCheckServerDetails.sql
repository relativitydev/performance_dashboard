delete from [EDDSPerformance].[eddsdbo].[EnvironmentCheckDatabaseDetails]
where [ServerName] = @serverName;

INSERT INTO [EDDSPerformance].[eddsdbo].[EnvironmentCheckDatabaseDetails]
		   ([ServerName]
		   ,[SQLVersion]
		   ,[AdHocWorkLoad]
		   ,[MaxServerMemory]
		   ,[MaxDegreeOfParallelism]
		   ,[TempDBDataFiles]
		   ,[LastSQLRestart])
	 VALUES
		   (@serverName
		   ,@sqlversion
		   ,@adhocWorkloads
		   ,@maxServerMemory
		   ,@maxDegreeOfParallelism
		   ,@tempDbFileCount
		   ,@lastSqlRestart)