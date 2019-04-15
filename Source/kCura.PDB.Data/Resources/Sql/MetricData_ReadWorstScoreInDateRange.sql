
select top(1) md.*
from eddsdbo.MetricData md with(nolock)
inner join eddsdbo.Metrics m with(nolock) on m.ID = md.MetricID and m.MetricTypeID = @metricTypeId
inner join eddsdbo.[Hours] h with(nolock) on h.ID = m.HourID
where h.HourTimeStamp >= @startTime
	and h.HourTimeStamp <= @endTime
	and h.Status != 4
	and md.ServerID = @serverId
	and md.Score is not null -- this case *should* never happen but just in case
order by score asc