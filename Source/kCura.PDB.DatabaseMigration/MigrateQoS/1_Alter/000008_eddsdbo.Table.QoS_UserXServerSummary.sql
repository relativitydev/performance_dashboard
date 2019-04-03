USE EDDSQoS

GO

IF EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'QoS_UserXServerSummary' AND TABLE_SCHEMA = 'EDDSDBO')
	DROP TABLE [EDDSDBO].[QoS_UserXServerSummary]

GO

IF NOT EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'QoS_UserXServerSummary' AND TABLE_SCHEMA = 'EDDSDBO')
BEGIN
	CREATE TABLE EDDSDBO.QoS_UserXServerSummary (
		UserXID INT IDENTITY (1, 1), PRIMARY KEY (UserXID)
	--environment attributes
		,ServerArtifactID int
		,QoSHourID bigint --hour identity (ServerArtifactID + ‘Year’ + ‘Julian day’ + ‘hour’) (bigint) (from lookingGlass - this should be passed in)
		,GlassRunID INT
		,SummaryDayHour datetime --this is being stored for consistency checking purposes.  
		,BusiestWorkspace INT  
	--this is the one with the highest individual concurrency in an hour. Remember, just because there are a lot of requests doesn't mean the server was "busy". Imagine a workspace with thousands of queries that only take a millisecond to complete.  Concurency per second could average near 1.  Concurrency, paired with total activity, is the metric to look at.  Across the 90 days of activity, when there are thousands of queries running in an hour, what is the concurrency?  Is it low or high? 
		,BusiestUser int --for future use
		,QtyActiveUsers int  
		,TotalExecutionTime int
		,QConcurrency DECIMAL (10, 3)  --not yet used --all query concurrency over active time.
		,AVGConcurrency DECIMAL (10, 3) 
		,MaxConcurrency int  -- all queries
		,ArrivalRate DECIMAL (10, 3) -- this is all queries, including 0 rounded queries, and is a per second average over the hour. 
		,SimpleQueryConcurrency DECIMAL (10, 3)--( 281, 282, 283 )
		,ComplexQueryConcurrency DECIMAL (10, 3)
		,TotalSimpleQuery int
		,TotalComplexQuery int 
		,TotalViews int --( 1 )
		,TotalEdits int --( 3 )
		,TotalMassOps int --( 4, 5, 6 )
		,TotalOtherActions int --( 29, 32, 33, 35, 37 )
		,TotalSLRQ int
		,TotalCLRQ int
		--Total long views (TLV)
		--Total long Edits (TLE)
		--User Metrics
		,MaxConcurrenctUsers int 
		,MaxArrivalRateUsers Decimal (10, 3)
		,ProductivityPercent DECIMAL (10, 3)--live wire, handle with care.  MaxArrivalRate/MaxArrivalRateUsers
		,TotalUniqueUsers int --unique users in the hour
		,TotalSQUsers int -- (includes SLRQs)
		,TotalCQUsers int -- (Includes CLRQs)
		--,TotalVUsers int -- (users that viewed documents)
		--,TotalEUsers  int --(users that edited documents)
		--Total execution times
		,TotalSQExecutionTime int --(includes LSRQ time)
		,TotalCQViewExecutionTime int --(includes CLRQs) 
		,TotalTOPQueryExecutionTime int
		,TotalCOUNTQueryExecutionTime int 
		,TotalLongRunningExecutionTime int
		,TotalSLRQExecutionTime int
		,TotalCLRQExecutionTime int
		--,TotalLongViewExecutionTime int
		--,TotalLongEditExecutionTime int
		--Averages [Computed]
		,AverageSQPerUser DECIMAL (10, 3)  --( SQ / total users = ASQpU )
		,AverageCQPerUser DECIMAL (10, 3) --( CQ / total users =  ACPQpU )
		,AverageViewsPerUser DECIMAL (10, 3) --( view / users = AVpU)
		,AverageEditsPerUser DECIMAL (10, 3) --( edits / users = AEpU)
		--Long Running Averages [computed]
		,AverageUserSLRQ DECIMAL (10, 3) 
		,AverageUserCLRQ DECIMAL (10, 3) 
		,AverageLVEPerUser DECIMAL (10, 3) 
		,AverageLEPerUser DECIMAL (10, 3) 
		--Average Percentages [computed] (publish to PDB)
		,AvgSQScorePerUser DECIMAL (10, 3) -- ( SLRQ / SQ )
		,AvgCQScorePerUser DECIMAL (10, 3) -- ( CLRQ / CQ )
		--,PercentLVEPerUser DECIMAL 
		--,PercentLEPerUser DECIMAL
		--Limits and Waits [computed] (publish to PDB)
		,SLRQLimit int -- ( Total Simple Queries * 2 )
		,CLRQLimit int -- ( Total Complex Queries * 8)
		,AverageUserSLRQWaits DECIMAL (10, 3)  -- (average user spent x seconds on simple queries) 
		,AverageUserCLRQWaits DECIMAL (10, 3) 
		--Clean out records that are over 180 days old every night. 
		,AVGSLRQOverage DECIMAL (10, 3)--(the average amount of time that was OVER the LRQ threshold for Simple queries)
		,AVGCLRQOverage DECIMAL (10, 3)--(the average amount of time that was OVER the LRQ threshold for complex queries)
		,DocumentSearchScore DECIMAL (10, 3) --This is a variant of AvgSQScorePerUser that takes into account QueryID groupings for search audits (prospective UX score for PDB 9.3+)
	)
END