

INSERT INTO [eddsdbo].[MetricData_AuditAnalysis]
			([MetricDataId]
			,[UserId]
			,[TotalComplexQueries]
			,[TotalLongRunningQueries]
			,[TotalSimpleLongRunningQueries]
			,[TotalQueries]
			,[TotalExecutionTime])
		VALUES
			(@metricDataId
			,@userId
			,@totalComplexQueries
			,@totalLongRunningQueries
			,@totalSimpleLongRunningQueries
			,@totalQueries
			,@totalExecutionTime)