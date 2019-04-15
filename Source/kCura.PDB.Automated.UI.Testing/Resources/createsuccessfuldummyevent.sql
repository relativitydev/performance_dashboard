USE [EDDSPerformance]

INSERT INTO [eddsdbo].[Events]
           ([SourceTypeID]
           ,[SourceID]
           ,[StatusID]
           ,[TimeStamp]
           ,[LastUpdated])
     VALUES
           (99901
           ,1234
           ,@status
           ,GetUTCDate()
           ,GetUTCDate())


