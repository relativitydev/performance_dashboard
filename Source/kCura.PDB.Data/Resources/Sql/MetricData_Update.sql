

UPDATE [eddsdbo].[MetricData]
   SET [MetricID] = @metricID,
       [ServerID] = @serverID,
       [Score] = @score,
	   [Data] = @data
 WHERE ID = @id