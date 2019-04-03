

select top(1) h.*
from eddsdbo.[Hours] as h with(nolock)
left outer join eddsdbo.QoS_Ratings as r with(nolock) on r.SummaryDayHour = h.HourTimeStamp and h.Status != 4
where 
	r.QRatingID is null -- No rating entry
	and h.HourTimeStamp >= DATEADD(d, @backfillDays, getutcdate()) -- Is within the current backfill period
	and h.HourTimeStamp > (
		select 
			ISNULL(MAX(SummaryDayHour), '1800-01-01')
		from eddsdbo.QoS_Ratings
		) -- Is greater than the current Max timestamp from the ratings table
		and h.Status != 4
order by h.HourTimeStamp