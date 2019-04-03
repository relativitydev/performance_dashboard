

SELECT *
FROM [eddsdbo].[MetricData] with(nolock)
where [MetricID] = @metricID and ([ServerID] = @serverID OR ([ServerID] is null and @serverID is null))