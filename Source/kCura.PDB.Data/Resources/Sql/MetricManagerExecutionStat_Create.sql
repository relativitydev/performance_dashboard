

INSERT INTO [eddsdbo].[MetricManagerExecutionStats]
		([ExecutionId]
		,[Start]
		,[End]
		,[Name]
		,[TotalTime]
		,[MaxTime]
		,[Count])
	VALUES
		(@ExecutionId
		,@Start
		,@End
		,@Name
		,@TotalTime
		,@MaxTime
		,@Count)