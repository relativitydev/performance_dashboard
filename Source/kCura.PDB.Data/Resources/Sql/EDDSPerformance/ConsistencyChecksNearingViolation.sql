USE EDDSPerformance

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_DBCCResults]') AND type in (N'U'))
BEGIN
	SELECT [DBName]
		  ,[DaysSinceLast]
	FROM [eddsdbo].QoS_DBCCResults
	WHERE LoggedDate = (SELECT MAX(LoggedDate) FROM [eddsdbo].QoS_DBCCResults)
	AND DaysSinceLast >= 7
END