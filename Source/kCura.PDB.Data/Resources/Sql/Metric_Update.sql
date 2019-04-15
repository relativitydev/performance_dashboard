

UPDATE [eddsdbo].[Metrics]
   SET [MetricTypeID] = @metricTypeID
      ,[HourID] = @hourID
	WHERE [ID] = @id