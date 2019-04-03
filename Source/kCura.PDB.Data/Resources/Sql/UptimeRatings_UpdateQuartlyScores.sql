;WITH scorableWeeks AS
(
	SELECT DATEADD(dd, 1-(DATEPART(dw, @summaryDayHour)), DATEDIFF(dd, 0, @summaryDayHour)) AS SummaryDayHour
	UNION ALL (
		SELECT DATEADD(wk, -1, SummaryDayHour)
		FROM scorableWeeks
		WHERE DATEADD(wk, -1, SummaryDayHour) >= DATEADD(dd, -90, @summaryDayHour)
	)
)
UPDATE u
SET 
	UptimeScore = (SELECT ISNULL(AVG(a.WeekUptimeScore), u.WeekUptimeScore)
		FROM (
			--Include all Trust hours in the last 90 days
			SELECT
				WeekUptimeScore
			FROM eddsdbo.QoS_UptimeRatings ur
			INNER JOIN scorableWeeks s
				ON ur.SummaryDayHour = s.SummaryDayHour
			WHERE s.SummaryDayHour <= u.SummaryDayHour
			UNION ALL (
				--Include this row's weekly score if not already included
				SELECT
					WeekUptimeScore
				FROM eddsdbo.QoS_UptimeRatings
				WHERE SummaryDayHour = u.SummaryDayHour
					AND DATEADD(dd, 1-(DATEPART(dw, SummaryDayHour)), DATEDIFF(dd, 0, SummaryDayHour)) != SummaryDayHour --This is not a Trust hour (included above)
			)
		) a
	)
FROM eddsdbo.QoS_UptimeRatings u
WHERE RowHash IS NULL

UPDATE [eddsdbo].[QoS_UptimeRatings]
SET RowHash = HASHBYTES('SHA1',
	CAST(ISNULL(UpRatingID, 0) AS varchar) +
	CAST(ISNULL(HoursDown, 0) AS varchar) +
	CAST(ISNULL(UptimeScore, 0) AS varchar) +
	CAST(ISNULL(SummaryDayHour, 0) AS varchar)
)
WHERE RowHash IS NULL