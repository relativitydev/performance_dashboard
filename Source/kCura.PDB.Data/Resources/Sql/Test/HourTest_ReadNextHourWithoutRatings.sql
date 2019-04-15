-- EDDSPerformance

select top(1) h.*
from eddsdbo.[MockHours] as mh with(nolock)
inner join eddsdbo.[Hours] as h with(nolock) on h.Id = mh.HourId
left outer join eddsdbo.QoS_Ratings as r with(nolock) on r.SummaryDayHour = h.HourTimeStamp and h.Status != 4
where 
	r.QRatingID is null -- No rating entry
	and h.HourTimeStamp > (
		select 
			ISNULL(MAX(SummaryDayHour), '1800-01-01')
		from eddsdbo.QoS_Ratings
		) -- Is greater than the current Max timestamp from the ratings table
		and h.Status != 4
order by h.HourTimeStamp