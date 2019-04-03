

SELECT m.*, mt.SampleType 
FROM [eddsdbo].[Metrics] as m with(nolock)
inner join [eddsdbo].[MetricTypes] as mt with(nolock) on mt.Id = m.MetricTypeID
WHERE m.ID = @id