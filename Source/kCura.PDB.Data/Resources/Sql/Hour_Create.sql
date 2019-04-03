

INSERT INTO [eddsdbo].[Hours]
			([HourTimeStamp]
           ,[Score]
           ,[InSample]
		   ,[StartedOn]
		   ,[CompletedOn]
		   ,[Status])
     VALUES 
			(@hourTimeStamp
           ,@score
           ,@inSample
		   ,@StartedOn
		   ,@CompletedOn
		   ,@status)

SELECT * FROM [eddsdbo].[Hours] with(nolock) WHERE ID = @@IDENTITY