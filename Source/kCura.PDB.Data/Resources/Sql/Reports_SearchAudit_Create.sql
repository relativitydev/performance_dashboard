

INSERT INTO [eddsdbo].[Reports_SearchAudits]
           ([HourId]
		   ,[ServerId]
           ,[SearchId]
		   ,[MinAuditId]
           ,[UserId]
           ,[WorkspaceId]
           ,[IsComplex]
           ,[TotalSearchAudits]
           ,[PercentLongRunning]
           ,[TotalExecutionTime])
     VALUES
           (@HourId
		   ,@serverId
           ,@searchId
		   ,@minAuditId
           ,@userId
           ,@workspaceId
           ,@isComplex
           ,@totalSearchAudits
           ,@percentLongRunning
           ,@totalExecutionTime)