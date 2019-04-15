namespace kCura.PDB.Core.Enumerations
{
	public enum ProcessControlId
	{
		ApplicationMetricsDWLoad = 1,
		ServerHealthSummary = 2,
		ServerInfoRefresh = 3,
		DataTableCleanup = 8,
		CollectWaitStatistics = 9,
		MonitorBackupDBCC = 10,
		QuarterlyScoreAlerts = 11,
		QuarterlyScoreStatus = 12,
		ConfigurationChangeAlerts = 13,
		WeeklyScoreAlerts = 14,
		RecoverabilityIntegrityAlerts = 16,
		InfrastructurePerformanceForecast = 18,
		UserExperienceForecast = 19,
		ReadErrorLog = 20,
		CycleErrorLog = 21,
		MonitorVirtualLogFiles = 22,
		EnvironmentCheckRelativityConfig = 23,
		EnvironmentCheckServerInfo = 24,
		EnvironmentCheckSqlConfig = 26,
		ApplicationPerformance = 27

		// Deprecated:
		// BISSummaryRefresh = 4,
		// InstallServerScripts = 5,
		// InstallWorkspaceScripts = 6,
		// RunLookingGlass = 7,
		// PopulateCasesToAudit = 17,
		// TrustDeliveryAlerts = 15,
	}
}
