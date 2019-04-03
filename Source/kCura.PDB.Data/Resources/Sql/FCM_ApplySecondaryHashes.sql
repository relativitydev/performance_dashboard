UPDATE [EDDSPerformance].[eddsdbo].[QoS_RecoverabilityIntegritySummary]
SET RowHash = HASHBYTES('SHA1',
	CAST(ISNULL(RISID, 0) AS varchar) +
	CAST(ISNULL(RecoverabilityIntegrityScore, 0) AS varchar)
)
WHERE RowHash IS NULL
OPTION (MAXDOP 2)