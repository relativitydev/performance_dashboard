USE EDDSPerformance
GO

UPDATE eddsdbo.QoS_UptimeRatings
SET IsWebDowntime = (
	SELECT TOP 1
		CASE WHEN MAX(UptimePct) = 0 THEN 1
			ELSE 0
		END
	FROM eddsdbo.WebServerSummary wss WITH(NOLOCK)
	WHERE wss.MeasureDate = SummaryDayHour
)