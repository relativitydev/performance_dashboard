USE EDDSPerformance
GO

IF NOT EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'QoS_GlassRunLog' AND TABLE_SCHEMA = N'EDDSDBO') 
BEGIN
	CREATE TABLE EDDSDBO.QoS_GlassRunLog
	(
		GRLogID INT IDENTITY ( 1 , 1 ),PRIMARY KEY (GRLogID)
		,GlassRunID INT
		,RunTimeUTC datetime -- this is the time when the call to the procedure happened.
		,LogTimestampUTC datetime -- this is the time of the log entry.  
		,LogIntervalDurationMS int  -- the time that this logged item took to complete
		,RunDurationMS int -- this is the total length of time that the procedure that called the logging procedure has been running. 
		,Module varchar (100)
		,TaskCompleted varchar(500)
		,OtherVars varchar(max)
		,NextTask varchar (500)
	)
END
GO