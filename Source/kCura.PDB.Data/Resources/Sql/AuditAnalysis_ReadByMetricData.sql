

select * from eddsdbo.MetricData_AuditAnalysis as aa with(nolock)
where aa.MetricDataId = @metricDataId