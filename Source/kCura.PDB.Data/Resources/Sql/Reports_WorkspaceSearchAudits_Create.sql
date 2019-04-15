

INSERT INTO [eddsdbo].[Reports_WorkspaceSearchAudits]
           ([HourId]
           ,[ServerId]
		   ,[SearchId]
		   ,[searchName]
           ,[WorkspaceId]
           ,[TotalExecutionTime]
           ,[TotalSearchAudits]
           ,[IsComplex])
     VALUES
           (@hourId
           ,@serverId
		   ,@searchId
		   ,@searchName
           ,@workspaceId
           ,@totalExecutionTime
           ,@totalSearchAudits
           ,@isComplex)