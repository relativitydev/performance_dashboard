

SELECT CASE WHEN EXISTS(
	SELECT *
	  FROM [eddsdbo].[MaintenanceSchedules] with(nolock)
	  WHERE [StartTime] <= @hourTimeStamp AND
	  [EndTime] > @hourTimeStamp
)
THEN CAST(1 AS BIT)
ELSE CAST(0 AS BIT) END