

INSERT INTO [eddsdbo].[MaintenanceSchedules]
           ([StartTime]
           ,[EndTime]
           ,[Reason]
           ,[Comments]
           ,[IsDeleted])
     VALUES
           (@startTime
           ,@endTime
           ,@reason
           ,@comments
           ,@isDeleted)

SELECT * FROM [eddsdbo].[MaintenanceSchedules] with(nolock) WHERE ID = @@IDENTITY