
select top(1) md.*
from eddsdbo.MetricData md  with(nolock)
inner join eddsdbo.Metrics m  with(nolock) on m.Id = md.MetricId and m.HourId = @hourId and m.MetricTypeId = @metricType
where md.[ServerID] = @serverID OR (md.[ServerID] is null and @serverID is null)