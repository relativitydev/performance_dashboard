namespace kCura.PDB.Core.Constants
{
	public static class ConfigurationAuditFields
	{
		//General configuration settings
		public const string Partner = "Partner";
		public const string SendScoresAutomatically = "Send Scores Automatically";
		public const string EnableViewBasedMonitoring = "Enable View-Based Monitoring";
		public const string EnableCommandBasedMonitoring = "Enable Command-Based Monitoring";
		public const string ShowInvariantHistory = "Show Invariant History";

		//View-based monitoring settings
		public const string MonitoringTargetDatabase = "Monitoring Target Database";
		public const string MonitoringEnabled = "Server Monitoring Enabled";

		//Recipient fields
		public const string NotificationsRecipientsWeeklyScoreAlerts = "Weekly Score Alert Recipients";
		public const string NotificationsRecipientsSystemLoadForecast = "Infrastructure Performance Forecast Recipients";
		public const string NotificationsRecipientsUserExperienceForecast = "User Experience Forecast Recipients";
		public const string NotificationsRecipientsQuarterlyScoreAlerts = "Quarterly Score Alert Recipients";
		public const string NotificationsRecipientsQuarterlyScoreStatus = "Quarterly Score Status Recipients";
		public const string NotificationsRecipientsBackupDBCCAlerts = "Recoverability/Integrity Alert Recipients";
		public const string NotificationsRecipientsConfigurationChangeAlerts = "Configuration Change Alert Recipients";

		//Enabled toggles
		public const string SendBackupDbccNotifications = "Send Recoverability/Integrity Alerts";
		public const string SendHourlyNotifications = "Send Hourly Alerts for Weekly Score";
		public const string SendDailyNotifications = "Send Daily Alerts for Quarterly Score";
		public const string SendWeeklyNotifications = "Send Weekly Score Report";
		public const string SendConfigurationChangeNotifications = "Send Configuration Change Alerts";
		public const string SendSystemLoadForecast = "Send Infrastructure Performance Forecast";
		public const string SendUserExperienceForecast = "Send User Experience Forecast";

		//Notification frequencies
		public const string WeeklyScoreNotificationFrequency = "Weekly Score Notification Frequency";
		public const string SystemLoadNotificationFrequency = "Infrastructure Performance Notification Frequency";
		public const string UserExperienceNotificationFrequency = "User Experience Notification Frequency";
		public const string QuarterlyScoreNotificationFrequency = "Quarterly Score Notification Frequency";
		public const string ScoreStatusNotificationFrequency = "Score Status Notification Frequency";
		public const string BackupDbccNotificationFrequency = "Recoverability/Integrity Notification Frequency";

		//Threshold values
		public const string ThresholdsWeeklyScoreAlert = "Weekly Score Alert Threshold";
		public const string ThresholdsQuarterlyScoreAlert = "Quarterly Score Alert Threshold";
		public const string ThresholdsSystemLoad = "Infrastructure Performance Forecast Threshold";
		public const string ThresholdsUserExperience = "User Experience Forecast Threshold";
	}
}
