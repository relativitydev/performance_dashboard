

UPDATE [eddsdbo].[Events]
   SET [SourceTypeID] = @sourceTypeID
	  ,[SourceID] = @sourceID
      ,[StatusID] = @statusId
	  ,[EventHash] = @eventHash
	  ,[Delay] = @delay
	  ,[LastUpdated] = getutcdate()
	  ,[Retries] = @retries
	  ,[ExecutionTime] = @executionTime
	  ,[HourId] = @hourId
 WHERE ID = @Id