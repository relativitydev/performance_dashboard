

INSERT INTO [eddsdbo].[Events]
           ([SourceTypeID]
		  ,[SourceID]
		  ,[StatusID]
		  ,[TimeStamp]
		  ,[EventHash]
		  ,[Delay]
		  ,[HourID]
		  ,[PreviousEventId]
		  ,[LastUpdated])
     VALUES
           (@sourceTypeID
		  ,@sourceID
		  ,@statusId
		  ,getutcdate()
		  ,@eventHash
		  ,@delay
		  ,@hourID
		  ,@previousEventId
		  ,getutcdate())
		   