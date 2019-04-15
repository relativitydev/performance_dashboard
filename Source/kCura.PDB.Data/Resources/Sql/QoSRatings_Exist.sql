SELECT 
CASE WHEN 
EXISTS(
	SELECT TOP 1 *
	FROM eddsdbo.[Hours] h
	INNER JOIN eddsdbo.QoS_Ratings r ON h.HourTimeStamp = r.SummaryDayHour
	WHERE h.ID = @hourId
) THEN 1 
ELSE 0 
END