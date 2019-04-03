USE [EDDSPerformance]

-- Migrate from old system
IF COL_LENGTH ('eddsdbo.QoS_UserExperienceRatings' ,'SummaryDayHour') IS NOT NULL
BEGIN
	UPDATE uxr
	SET uxr.HourId = h.Id
	FROM eddsdbo.[QoS_UserExperienceRatings] uxr
	INNER JOIN eddsdbo.Hours h 
		ON uxr.SummaryDayHour = h.HourTimeStamp
	WHERE HourId = 0
	
	ALTER TABLE eddsdbo.[QoS_UserExperienceRatings]
    DROP COLUMN [SummaryDayHour]
END