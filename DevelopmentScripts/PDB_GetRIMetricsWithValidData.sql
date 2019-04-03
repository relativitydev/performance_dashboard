-- queries the metric data table for valid metric data and then doeese distinct on metric type returning the total metric types with valid data

SELECT count(distinct MetricTypeID) as MetricsWithValidData
  FROM [EDDSPerformance].[eddsdbo].[MetricData] md
  inner join [EDDSPerformance].eddsdbo.Metrics m on m.id = md.MetricID
  where m.MetricTypeID between 44 and 49
	and [data] is not null
	and [score] is not null
	and [data] not in ('{"TimeToRecover":null}', '{"WindowExceededBy":null}')
	and ([Data] not like '%"DatabasesCovered":0}' and MetricTypeID = 49)
	and ([Data] not like '%"DatabasesCovered":0}' and MetricTypeID = 48)