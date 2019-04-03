USE EDDSPerformance
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_Ratings]') AND type in (N'U'))
UPDATE [EDDSPerformance].[eddsdbo].[QoS_Ratings]
SET RowHash = HASHBYTES('SHA1',
	CAST(ISNULL(QoSHourID, 0) AS varchar) +
	CAST(ISNULL(UserExperience4SLRQScore, 0) AS varchar) +
	CAST(ISNULL(UserExperienceSLRQScore, 0) AS varchar) +
	CAST(ISNULL(SystemLoadScoreWeb, 0) AS varchar) +
	CAST(ISNULL(SystemLoadScoreSQL, 0) AS varchar) +
	CAST(ISNULL(SystemLoadScore, 0) AS varchar) +
	CAST(ISNULL(BackupScore, 0) AS varchar) +
	CAST(ISNULL(DBCCScore, 0) AS varchar) +
	CAST(ISNULL(WeekUserExperience4SLRQScore, 0) AS varchar) +
	CAST(ISNULL(WeekUserExperienceSLRQScore, 0) AS varchar) +
	CAST(ISNULL(WeekSystemLoadScoreWeb, 0) AS varchar) +
	CAST(ISNULL(WeekSystemLoadScoreSQL, 0) AS varchar) +
	CAST(ISNULL(WeekSystemLoadScore, 0) AS varchar) +
	CAST(ISNULL(SummaryDayHour, 0) AS varchar) +
	CAST(ISNULL(QRatingID, 0) AS varchar)
	)

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_CasesToAudit]') AND type in (N'U'))
UPDATE [EDDSPerformance].[eddsdbo].[QoS_CasesToAudit]
SET RowHash = HASHBYTES('SHA1',
	CAST(ISNULL(RowID, 0) AS varchar) +
	CAST(ISNULL(ServerName, 0) AS varchar) +
	CAST(ISNULL(ServerID, 0) AS varchar) +
	CAST(ISNULL(CaseArtifactID, 0) AS varchar) +
	CAST(ISNULL(DatabaseName, 0) AS varchar) +
	CAST(ISNULL(WorkspaceName, 0) AS varchar) +
	CAST(ISNULL(AuditStartDate, 0) AS varchar) +
	CAST(ISNULL(IsActive, 0) AS varchar)
	)
WHERE IsCompleted = 1;

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_SampleHistory]') AND type in (N'U'))
UPDATE [EDDSPerformance].[eddsdbo].[QoS_SampleHistory]
SET RowHash = HASHBYTES('SHA1',
	CAST(ISNULL(QSampleID, 0) AS varchar) +
	CAST(ISNULL(QoSHourID, 0) AS varchar) +
	CAST(ISNULL(SummaryDayHour, 0) AS varchar) +
	CAST(ISNULL(ServerArtifactID, 0) AS varchar) +
	CAST(ISNULL(ArrivalRate, 0) AS varchar) +
	CAST(ISNULL(AVGConcurrency, 0) AS varchar)
	)

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_SystemLoadSummary]') AND type in (N'U'))
UPDATE [EDDSPerformance].[eddsdbo].[QoS_SystemLoadSummary]
SET RowHash = HASHBYTES('SHA1',
	CAST(ISNULL(QSLSID, 0) AS varchar) +
	CAST(ISNULL(ServerArtifactID, 0) AS varchar) +
	CAST(ISNULL(ServerTypeID, 0) AS varchar) +
	CAST(ISNULL(AvgRAMPagesePerSec, 0) AS varchar) +
	CAST(ISNULL(AvgCPUpct, 0) AS varchar) +
	CAST(ISNULL(AvgRAMpct, 0) AS varchar) +
	CAST(ISNULL(RAMPagesScore, 0) AS varchar) +
	CAST(ISNULL(RAMPctScore, 0) AS varchar) +
	CAST(ISNULL(CPUScore, 0) AS varchar) +
	CAST(ISNULL(QoSHourID, 0) AS varchar) +
	CAST(ISNULL(SummaryDayHour, 0) AS varchar)
	)

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_UptimeRatings]') AND type in (N'U'))
UPDATE [EDDSPerformance].[eddsdbo].[QoS_UptimeRatings]
SET RowHash = HASHBYTES('SHA1',
	CAST(ISNULL(UpRatingID, 0) AS varchar) +
	CAST(ISNULL(HoursDown, 0) AS varchar) +
	CAST(ISNULL(UptimeScore, 0) AS varchar) +
	CAST(ISNULL(SummaryDayHour, 0) AS varchar)
	)

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_UserXInstanceSummary]') AND type in (N'U'))
UPDATE [EDDSPerformance].[eddsdbo].[QoS_UserXInstanceSummary]
SET RowHash = HASHBYTES('SHA1',
	CAST(ISNULL(UserXID, 0) AS varchar) +
	CAST(ISNULL(ServerArtifactID, 0) AS varchar) +
	CAST(ISNULL(QoSHourID, 0) AS varchar) +
	CAST(ISNULL(SummaryDayHour, 0) AS varchar) +
	CAST(ISNULL(TotalExecutionTime, 0) AS varchar) +
	CAST(ISNULL(AVGConcurrency, 0) AS varchar) +
	CAST(ISNULL(ArrivalRate, 0) AS varchar) +
	CAST(ISNULL(AvgSQScorePerUser, 0) AS varchar) +
	CAST(ISNULL(AvgCQScorePerUser, 0) AS varchar)
	)