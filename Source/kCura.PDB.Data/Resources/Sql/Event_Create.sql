

IF NOT EXISTS(SELECT * FROM [eddsdbo].[Events] (UPDLOCK) WHERE [SourceID] = @sourceID AND [SourceTypeID] = @sourceTypeID)
BEGIN
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
	SELECT * FROM [eddsdbo].[Events] WHERE ID = @@IDENTITY
END
ELSE
	SELECT * FROM [eddsdbo].[Events] WHERE [SourceID] = @sourceID AND [SourceTypeID] = @sourceTypeID