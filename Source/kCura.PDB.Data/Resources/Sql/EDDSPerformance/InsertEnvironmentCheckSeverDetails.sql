USE [EDDSPerformance]


delete from [eddsdbo].[EnvironmentCheckServerDetails]
where [ServerIPAddress] = @serverIPAddress

INSERT INTO [eddsdbo].[EnvironmentCheckServerDetails]
           ([ServerName]
           ,[OSVersion]
           ,[OSName]
           ,[LogicalProcessors]
           ,[Hyperthreaded]
		   ,[ServerIPAddress])
     VALUES
           (@serverName
           ,@osVersion
           ,@osName
           ,@LogicalProcessors
           ,@hyperthreaded
		   ,@serverIPAddress)



