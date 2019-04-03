declare @hourTimeStamp datetime = '2017-02-15 04:00:00.000'

select ur.*, 
DuringMaintenance = 
	case 
		when exists (select 1 from eddsdbo.MaintenanceSchedules as ms where ms.StartTime <= ur.SummaryDayHour and ms.EndTime >= ur.SummaryDayHour and IsDeleted = 0)
			then 1
		else 0
	end
from eddsdbo.QoS_UptimeRatings as ur
where ur.SummaryDayHour > @hourTimeStamp
order by ur.SummaryDayHour desc