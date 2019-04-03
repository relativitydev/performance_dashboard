

select Count(Distinct SummaryDayHour) as HoursCompletedScoring
FROM eddsdbo.QoS_Ratings qr WITH(NOLOCK)
	WHERE qr.SummaryDayHour >= DATEADD(dd, @backFillHours, getutcdate())