declare @summaryDayHourCheck datetime = (select top 1 SummaryDayHour FROM eddsdbo.QoS_UptimeRatings ud WHERE RowHash IS NULL order by SummaryDayHour asc)

while (@summaryDayHourCheck is not null)
begin

	UPDATE ud
	SET	WeekUptimeScore = (
		SELECT TOP 1
			CASE
				WHEN UptimePercent < 90 THEN 0 --17 hours of downtime results in max points lost
				WHEN UptimePercent >= 99.99 THEN 100 --1.008 minutes of downtime results in a score of 100
				ELSE (UptimePercent - 90.0) * 100.0 / 9.99 --3.36 hours of downtime (98% uptime) results in a score of 80
			END
		FROM (
			SELECT (168.0 - ISNULL(SUM(HoursDown), 0)) * 100.0 / 168.0 as UptimePercent
			FROM eddsdbo.QoS_UptimeRatings
			WHERE SummaryDayHour > DATEADD(dd, -7, ud.SummaryDayHour)
				AND SummaryDayHour <= ud.SummaryDayHour
				AND AffectedByMaintenanceWindow = 0
		) upWeek
	)
	FROM eddsdbo.QoS_UptimeRatings ud
	WHERE RowHash IS NULL and SummaryDayHour = @summaryDayHourCheck

	set @summaryDayHourCheck = (select top 1 SummaryDayHour FROM eddsdbo.QoS_UptimeRatings ud WHERE RowHash IS NULL and SummaryDayHour > @summaryDayHourCheck order by SummaryDayHour asc )

end




