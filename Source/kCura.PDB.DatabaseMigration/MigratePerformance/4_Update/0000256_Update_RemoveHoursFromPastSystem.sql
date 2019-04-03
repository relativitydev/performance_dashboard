USE [EddsPerformance]

-- Grab all hours that don't have an hour score (or Ratings score)
DECLARE @hoursToCheck table(Id int)
INSERT INTO @hoursToCheck
SELECT h.ID
FROM eddsdbo.Hours h WITH(NOLOCK)
	LEFT OUTER JOIN eddsdbo.QoS_Ratings r WITH(NOLOCK) ON h.HourTimeStamp = r.SummaryDayHour
WHERE (Score IS NULL OR r.QRatingID IS NULL)
--SELECT * from @hoursToCheck

-- Reset them to the past hand have the system re-create those hours
UPDATE eddsdbo.Hours
SET HourTimeStamp = '1/1/1800 12:00:00 AM' -- min datetime
WHERE Id IN (SELECT Id FROM @hoursToCheck)