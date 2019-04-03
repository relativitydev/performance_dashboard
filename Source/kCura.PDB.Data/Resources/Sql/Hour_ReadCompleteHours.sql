

select h.*
from eddsdbo.[Hours] as h with(nolock)
inner join eddsdbo.QoS_Ratings as r with(nolock) on r.SummaryDayHour = h.HourTimeStamp
where 
	h.HourTimeStamp >= DATEADD(d, @backfillDays, getutcdate()) -- Is within the current backfill period
	and h.Status = 3
order by h.HourTimeStamp