-- Expected Arguments
-- @hourId int
-- @logging bit


DECLARE @summaryDayHour datetime,
	@lastRatedHour datetime,
	@loggingVars varchar(max),
	@lastCompletedTask varchar(max),
	@runStart datetime = getutcdate()

-- Grab the summaryDayHour
select @summaryDayHour = HourTimeStamp
from eddsdbo.[Hours] as h
where h.ID = @hourId

--Verify the integrity of last hour's data
SELECT @lastRatedHour = MAX(SummaryDayHour)
FROM EDDSDBO.QoS_Ratings
WHERE RowHash IS NOT NULL;