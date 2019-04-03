

select top(1) *
from [eddsdbo].[MetricData_AuditAnalysis] with(nolock)
where [MetricDataId] = @metricDataId and [UserId] = @userId