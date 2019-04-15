

UPDATE [eddsdbo].[MaintenanceSchedules]
   SET [StartTime] = @startTime
      ,[EndTime] = @endTime
      ,[Reason] = @reason
      ,[Comments] = @comments
      ,[IsDeleted] = @isDeleted
 WHERE ID = @id