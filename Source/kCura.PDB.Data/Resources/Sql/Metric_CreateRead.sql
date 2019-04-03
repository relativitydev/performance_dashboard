

SELECT m.*, mt.SampleType 
	FROM [eddsdbo].[Metrics] (nolock) as m
	inner join [eddsdbo].[MetricTypes] (nolock) as mt on mt.Id = m.MetricTypeID
	where [MetricTypeID] = @metricTypeID and [HourID] = @hourID