INSERT INTO [eddsdbo].[QoS_UptimeRatings]
           ([HoursDown]
           ,[SummaryDayHour]
           ,[IsWebDowntime]
		   ,[AffectedByMaintenanceWindow])
     VALUES
           (@hoursDown
           ,@summaryDayHour
           ,@isWebDowntime
		   ,@affectedByMaintenanceWindow)