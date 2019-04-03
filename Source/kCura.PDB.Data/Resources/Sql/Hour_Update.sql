

UPDATE [eddsdbo].[Hours]
   SET [HourTimeStamp] = @hourTimeStamp
      ,[Score] = @score
      ,[InSample] = @inSample
	  ,[StartedOn] = @StartedOn
	  ,[CompletedOn] = @CompletedOn
	  ,[Status] = @status
 WHERE ID = @id