
SELECT TOP 1 RecoverabilityIntegrityScore
FROM eddsdbo.QoS_RecoverabilityIntegritySummary WITH(NOLOCK)
WHERE SummaryDayHour <= @summaryDayHour
ORDER BY SummaryDayHour DESC
