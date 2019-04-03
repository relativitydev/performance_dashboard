namespace kCura.PDB.Core.Constants
{
	public static class ConfigurationKeys
	{
		public const string Section = "kCura.PDB";

		public const string AssemblyFileVersion = "AssemblyFileVersion";
		public const string CreateBootstrapEventsInterval = "CreateBootstrapEventsInterval";
		public const string CreateAgentsOnInstall = "CreateAgentsOnInstall";
		public const string DataCleanupThresholdDays = "DataCleanupThresholdDays";
		public const string EnableLookingGlassLogging = "EnableLookingGlassLogging";
		public const string EnableInstantFileInitializationCheck = "EnableInstantFileInitializationCheck";
		public const string EnqueueTasksInterval = "EnqueueTasksInterval";
		public const string EventManagerRunInterval = "EventManagerRunInterval";
		public const string JobServerMaxExecution = "JobServerMaxExecution";
		public const string JobServerSleepTime = "JobServerSleepTime";
		public const string JobServerShutdownTimeoutSeconds = "JobServerShutdownTimeoutSeconds";
		public const string JobServerWorkerCount = "JobServerWorkerCount";
		public const string LaunchDebugger = "PDBLaunchDebugger";
		public const string LastQuarterlyScoreBelowThreshold = "LastQuarterlyScoreBelowThreshold";
		public const string LastWeeklyScoreBelowThreshold = "LastWeeklyScoreBelowThreshold";
		public const string LogFileSize = "LogFileSize";
		public const string ManagerMeterReportTimeout = "ManagerMeterReportTimeout";
		public const string NotificationsRecipientsWeeklyScoreAlerts = "NotificationsRecipientsWeeklyScoreAlerts";
		public const string NotificationsRecipientsSystemLoadForecast = "NotificationsRecipientsSystemLoadForecast";
		public const string NotificationsRecipientsUserExperienceForecast = "NotificationsRecipientsUserExperienceForecast";
		public const string NotificationsRecipientsQuarterlyScoreAlerts = "NotificationsRecipientsQuarterlyScoreAlerts";
		public const string NotificationsRecipientsQuarterlyScoreStatus = "NotificationsRecipientsQuarterlyScoreStatus";
		public const string NotificationsRecipientsBackupDBCCAlerts = "NotificationsRecipientsBackupDBCCAlerts";
		public const string NotificationsRecipientsConfigurationChangeAlerts = "NotificationsRecipientsConfigurationChangeAlerts";
		public const string NotificationsThresholdWeeklyScore = "NotificationsThresholdWeeklyScore";
		public const string NotificationsThresholdQuarterlyScore = "NotificationsThresholdQuarterlyScore";
		public const string NotificationsThresholdSystemLoadScore = "NotificationsThresholdSystemLoadScore";
		public const string NotificationsThresholdUserExperienceScore = "NotificationsThresholdUserExperienceScore";
		public const string PartnerName = "PartnerName";
		public const string PassScore = "PassScore";
		public const string PurgeBackupDBCCTables = "PurgeBackupDBCCTables";
		public const string QueuePollInterval = "QueuePollInterval";
		public const string ResolveOrphanedEventsInterval = "ResolveOrphanedEventsInterval";
		public const string RoundhouseTimeoutSeconds = "RHTimeoutSeconds";
		public const string SendBackupDbccNotifications = "SendBackupDbccNotifications";
		public const string SendConfigurationChangeNotifications = "SendConfigurationChangeNotifications";
		public const string SendDailyNotifications = "SendDailyNotifications";
		public const string SendHourlyNotifications = "SendHourlyNotifications";
		public const string SendSystemLoadForecast = "SendSystemLoadForecast";
		public const string SendUserExperienceForecast = "SendUserExperienceForecast";
		public const string SendWeeklyNotifications = "SendWeeklyNotifications";
		public const string ShowInvariantBackupDbccHistory = "ShowInvariantBackupDBCCHistory";
		public const string SuggestedWebUptimeUrl = "SuggestedWebUptimeUrl";
		public const string ConfigurationEditedBy = "ConfigurationLastEditedBy";
		public const string UseDbccCommandMonitoring = "UseDbccCommandMonitoring";
		public const string UseDbccViewMonitoring = "UseDbccViewMonitoring";
		public const string LogLevel = "LogLevel";
		public const string WebUptimeUserAgent = "WebUptimeUserAgent";
		public const string WarnScore = "WarnScore";
		public const string NumberOfEventsToEnqueue = "NumberOfEventsToEnqueue";

		public static class Edds
		{
			public const string SectionRelativityCore = "Relativity.Core";

			public const string AuditCountQueries = "AuditCountQueries";
			public const string AuditFullQueries = "AuditFullQueries";
			public const string AuditIdQueries = "AuditIdQueries";
		}
	}
}
