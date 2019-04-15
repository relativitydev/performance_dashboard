------- Insert poison wait
Declare @hourTimeStamp datetime, 
 @serverArtifactId int = 1015096, -- ServerArtifactID on the environment
 @poisonWaitTypeId int = (select top 1 WaitTypeID from eddsdbo.QoS_Waits where IsPoisonWait = 1) -- Grab first poison wait type

--select @hourTimeStamp = '2017-12-12 21:00:00' -- Explicit hour
--select * from eddsdbo.Hours -- Find the hour to use
 SELECT @hourTimeStamp = HourTimeStamp -- OR Grab hour from hours table using id
 FROM eddsdbo.Hours
 Where ID = 120

declare @qosHourId bigint = eddsdbo.QoS_GetServerHourID(@serverArtifactId, @hourTimeStamp)
 
 -- Pre check
 declare @poisonWaitCount int
  SELECT @poisonWaitCount = COUNT(*)
FROM eddsdbo.QoS_WaitSummary ws WITH(NOLOCK)
INNER JOIN eddsdbo.QoS_WaitDetail wd WITH(NOLOCK)
ON ws.WaitSummaryID = wd.WaitSummaryID
INNER JOIN eddsdbo.QoS_Waits w WITH(NOLOCK)
ON wd.WaitTypeID = w.WaitTypeID
WHERE ws.SummaryDayHour = @hourTimeStamp --QoS_SystemLoadSummary.SummaryDayHour
	AND ws.ServerArtifactID = @serverArtifactId --QoS_SystemLoadSummary.ServerArtifactID
	AND w.IsPoisonWait = 1
	AND wd.DifferentialWaitMs > 1000
select @poisonWaitCount as PoisonWaitCount

IF(@poisonWaitCount = 0)
BEGIN
 -- Does a waitSummary entry exist?
 select top 1 WaitSummaryID from eddsdbo.QoS_WaitSummary where SummaryDayHour = @hourTimeStamp

 declare @summaryId int
 if not exists (select top 1 WaitSummaryID from eddsdbo.QoS_WaitSummary where SummaryDayHour = @hourTimeStamp)
 BEGIN
	-- If not, add one
	INSERT INTO eddsdbo.QoS_WaitSummary (QoSHourID, SummaryDayHour, ServerArtifactID, ServerName, SignalWaitsRatio)
	SELECT
		@qosHourId
		,@hourTimeStamp
		,@serverArtifactId
		,ServerName
		,0
	FROM eddsdbo.[Server]
		WHERE ArtifactID = @serverArtifactId
	-- Grab the summaryId
	select @summaryId = SCOPE_IDENTITY()
 END
 ELSE
 BEGIN
	-- Grab the summaryId
	select top 1 @summaryId = WaitSummaryID from eddsdbo.QoS_WaitSummary where SummaryDayHour = @hourTimeStamp
 END

 -- Insert row into the records table
 INSERT INTO eddsdbo.QoS_WaitDetail (WaitSummaryID, WaitTypeID, CumulativeWaitMs, CumulativeSignalWaitMs, CumulativeWaitingTasksCount, DifferentialWaitMs)
 Values(@summaryId, @poisonWaitTypeId, 99999, 99999, 99999, 99999)

-- Prove there are now poison waits
 SELECT COUNT(*) as PoisonWaitCount
FROM eddsdbo.QoS_WaitSummary ws WITH(NOLOCK)
INNER JOIN eddsdbo.QoS_WaitDetail wd WITH(NOLOCK)
ON ws.WaitSummaryID = wd.WaitSummaryID
INNER JOIN eddsdbo.QoS_Waits w WITH(NOLOCK)
ON wd.WaitTypeID = w.WaitTypeID
WHERE ws.SummaryDayHour = @hourTimeStamp --QoS_SystemLoadSummary.SummaryDayHour
	AND ws.ServerArtifactID = @serverArtifactId --QoS_SystemLoadSummary.ServerArtifactID
	AND w.IsPoisonWait = 1
	AND wd.DifferentialWaitMs > 1000
END