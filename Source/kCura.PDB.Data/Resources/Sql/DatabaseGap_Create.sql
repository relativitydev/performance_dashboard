INSERT INTO [eddsdbo].[DatabaseGaps]
           ([DatabaseId]
           ,[GapStart]
           ,[GapEnd]
           ,[Duration]
           ,[ActivityType])
     VALUES
           (@DatabaseId
           ,@start
           ,@end
           ,@Duration
           ,@ActivityType)