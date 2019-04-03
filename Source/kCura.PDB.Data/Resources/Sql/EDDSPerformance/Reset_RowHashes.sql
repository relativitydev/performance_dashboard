USE EDDSPerformance

IF ('{0}' < DATEADD(dd, -1, getutcdate()))
RETURN

UPDATE [EDDSPerformance].[eddsdbo].[QoS_SampleHistory]
SET RowHash = HASHBYTES('SHA1',
	CAST(ISNULL(QSampleID, 0) AS varchar) +
	CAST(ISNULL(QoSHourID, 0) AS varchar) +
	CAST(ISNULL(SummaryDayHour, 0) AS varchar) +
	CAST(ISNULL(ServerArtifactID, 0) AS varchar) +
	CAST(ISNULL(ArrivalRate, 0) AS varchar) +
	CAST(ISNULL(AVGConcurrency, 0) AS varchar)
	)

UPDATE [EDDSPerformance].[eddsdbo].[QoS_SystemLoadSummary]
SET RowHash = HASHBYTES('SHA1',
	CAST(ISNULL(QSLSID, 0) AS varchar) +
	CAST(ISNULL(ServerArtifactID, 0) AS varchar) +
	CAST(ISNULL(ServerTypeID, 0) AS varchar) +
	CAST(ISNULL(RAMPagesScore, 0) AS varchar) +
	CAST(ISNULL(RAMPctScore, 0) AS varchar) +
	CAST(ISNULL(CPUScore, 0) AS varchar) +
	CAST(ISNULL(MemorySignalStateScore, 0) AS varchar) +
	CAST(ISNULL(PoisonWaits, 0) AS varchar) +
	CAST(ISNULL(WaitsScore, 0) AS varchar) +
	CAST(ISNULL(VLFScore, 0) AS varchar) +
	CAST(ISNULL(LatencyScore, 0) AS varchar) +
	CAST(ISNULL(QoSHourID, 0) AS varchar) +
	CAST(ISNULL(SummaryDayHour, 0) AS varchar)
	)
	
UPDATE [EDDSPerformance].[eddsdbo].[QoS_UserXInstanceSummary]
SET RowHash = HASHBYTES('SHA1',
	CAST(ISNULL(UserXID, 0) AS varchar) +
	CAST(ISNULL(ServerArtifactID, 0) AS varchar) +
	CAST(ISNULL(QoSHourID, 0) AS varchar) +
	CAST(ISNULL(SummaryDayHour, 0) AS varchar) +
	CAST(ISNULL(qtyActiveUsers, 0) AS varchar) +
	CAST(ISNULL(AVGConcurrency, 0) AS varchar) +
	CAST(ISNULL(ArrivalRate, 0) AS varchar) +
	CAST(ISNULL(AvgSQScorePerUser, 0) AS varchar) +
	CAST(ISNULL(DocumentSearchScore, 0) AS varchar)
)

UPDATE [EDDSPerformance].[eddsdbo].[QoS_Ratings]
SET RowHash = HASHBYTES('SHA1',
	CAST(ISNULL(QoSHourID, 0) AS varchar) +
	CAST(ISNULL(UserExperience4SLRQScore, 0) AS varchar) +
	CAST(ISNULL(UserExperienceSLRQScore, 0) AS varchar) +
	CAST(ISNULL(SystemLoadScoreWeb, 0) AS varchar) +
	CAST(ISNULL(SystemLoadScoreSQL, 0) AS varchar) +
	CAST(ISNULL(SystemLoadScore, 0) AS varchar) +
	CAST(ISNULL(IntegrityScore, 0) AS varchar) +
	CAST(ISNULL(WeekUserExperience4SLRQScore, 0) AS varchar) +
	CAST(ISNULL(WeekUserExperienceSLRQScore, 0) AS varchar) +
	CAST(ISNULL(WeekSystemLoadScoreWeb, 0) AS varchar) +
	CAST(ISNULL(WeekSystemLoadScoreSQL, 0) AS varchar) +
	CAST(ISNULL(WeekSystemLoadScore, 0) AS varchar) +
	CAST(ISNULL(WeekIntegrityScore, 0) as varchar) +
	CAST(ISNULL(SummaryDayHour, 0) AS varchar) +
	CAST(ISNULL(QRatingID, 0) AS varchar)
)

UPDATE [EDDSPerformance].[eddsdbo].[QoS_UptimeRatings]
SET RowHash = HASHBYTES('SHA1',
	CAST(ISNULL(UpRatingID, 0) AS varchar) +
	CAST(ISNULL(HoursDown, 0) AS varchar) +
	CAST(ISNULL(UptimeScore, 0) AS varchar) +
	CAST(ISNULL(SummaryDayHour, 0) AS varchar)
	)

UPDATE [EDDSPerformance].[eddsdbo].[QoS_CasesToAudit]
SET RowHash = HASHBYTES('SHA1',
	CAST(ISNULL(RowID, 0) AS varchar) +
	CAST(ISNULL(ServerID, 0) AS varchar) +
	CAST(ISNULL(CaseArtifactID, 0) AS varchar) +
	CAST(ISNULL(DatabaseName, 0) AS varchar) +
	CAST(ISNULL(AuditStartDate, 0) AS varchar) +
	CAST(ISNULL(IsActive, 0) AS varchar)
	)