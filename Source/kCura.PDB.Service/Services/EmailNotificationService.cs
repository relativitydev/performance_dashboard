namespace kCura.PDB.Service.Services
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Net.Mail;
	using System.Text;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Interfaces;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.BISSummary.Models;
	using kCura.PDB.Core.Services;
	using kCura.PDB.Service.BISSummary;
	using kCura.PDB.Service.Properties;

	/// <summary>
	/// This service is responsible for sending out daily and weekly score reports.
	/// Relativity's SMTP settings (found in EDDS.eddsdbo.Configuration under the kCura.Notification section) must be used.
	/// TODO -- Refactor cleanup to do less
	/// </summary>
	public class EmailNotificationService : IEmailNotificationService
	{
		public ISqlServerRepository Database;
		public BestInServiceReportingService ReportingService;
		private readonly IPdbConfigurationService configurationService;
		private readonly ISmtpClientFactory smtpClientFactory;
		private readonly IConfigurationRepository configurationRepository;
		private readonly IConfigurationAuditRepository configurationAuditRepository;

		public EmailNotificationService(ISqlServerRepository repository, ISmtpClientFactory smtpClientFactory, IPdbConfigurationService pdbConfigurationService, IConfigurationRepository configurationRepository, IConfigurationAuditRepository configurationAuditRepository)
		{
			Database = repository;
			ReportingService = new BestInServiceReportingService(repository);
			this.configurationService = pdbConfigurationService;
			this.smtpClientFactory = smtpClientFactory;
			this.configurationRepository = configurationRepository;
			this.configurationAuditRepository = configurationAuditRepository;
		}

		/// <summary>
		/// Sends a weekly score notification to the configured recipient if automated notifications are enabled
		/// The caller must handle exceptions
		/// </summary>
		public void SendQuarterlyScoreStatus()
		{
			//Gatekeeper #1: We will only try to send notifications if they're turned on and ratings exist
			var sendNotifications = Convert.ToBoolean(this.configurationRepository.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.SendWeeklyNotifications) ?? "False");
			var ratingsExist = ReportingService.LookingGlassHasRun();
			if (!sendNotifications || !ratingsExist)
				return;

			//Gatekeeper #2: If there's no one to send to, don't bother
			var recipients = this.configurationRepository.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsRecipientsQuarterlyScoreStatus);
			if (string.IsNullOrEmpty(recipients))
				return;

			var scores = ReportingService.GetOverallScores();

			//Prepare and send the notice
			var settings = this.GetSmtpConfiguration();
			var instanceName = Database.ReadInstanceName();
			var message = BuildQuarterlyScoreStatus(scores, instanceName);
			var client = this.smtpClientFactory.CreateSmtpClient(settings);
			var mailMessage = BuildMailMessage(settings, recipients, "Performance Dashboard - Quarterly Score Status", message);
			client.Send(mailMessage);
		}

		/// <summary>
		/// Sends a daily score notification to the configured recipient if automated notifications are enabled and we're below a threshold score
		/// The caller must handle exceptions
		/// </summary>
		public void SendQuarterlyScoreAlerts()
		{
			//Gatekeeper #1: We will only try to send notifications if they're turned on and ratings exist
			var sendNotifications = Convert.ToBoolean(this.configurationRepository.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.SendDailyNotifications) ?? "False");
			var ratingsExist = ReportingService.LookingGlassHasRun();
			if (!sendNotifications || !ratingsExist)
				return;

			//Gatekeeper #2: If there's no one to send to, don't bother
			var recipients = this.configurationRepository.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsRecipientsQuarterlyScoreAlerts);
			if (string.IsNullOrEmpty(recipients))
				return;

			var scores = ReportingService.GetOverallScores();

			//Gatekeeper #3: Only send a notice if we're below the threshold or we were last time
			var defaultThreshold = new QualityIndicatorConfigurationService(this.configurationRepository).GetIndictatorConfiguration();
			int threshold;
			int.TryParse(this.configurationRepository.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsThresholdQuarterlyScore) ?? defaultThreshold.PassScore.ToString(), out threshold);
			var belowThreshold = scores.QuarterlyScore < threshold;
			var belowThresholdLast = Convert.ToBoolean(this.configurationRepository.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.LastQuarterlyScoreBelowThreshold) ?? "False");
			if (!belowThreshold && !belowThresholdLast)
				return;
			this.configurationRepository.SetConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.LastQuarterlyScoreBelowThreshold, belowThreshold.ToString());

			//Prepare and send the notice
			var settings = this.GetSmtpConfiguration();
			var instanceName = Database.ReadInstanceName();
			var message = BuildQuarterlyScoreAlert(scores, instanceName, belowThreshold);
			var client = this.smtpClientFactory.CreateSmtpClient(settings);
			var mailMessage = BuildMailMessage(settings, recipients, "Performance Dashboard - Quarterly Score Alert", message);
			client.Send(mailMessage);
		}

		/// <summary>
		/// Sends a reconfiguration notification to the specified recipients
		/// We MUST attempt to send even if this notification type is now disabled - at the time it was triggered, the notifications were enabled!
		/// The caller must handle exceptions
		/// </summary>
		/// <param name="auditStartTime">the start time for the audit</param>
		public void SendConfigurationChangeAlerts(DateTime auditStartTime)
		{
			var changes = this.configurationAuditRepository.ReadAll().Where(x => x.CreatedOn >= auditStartTime).ToList();
			if (changes.Any())
			{
				// Only send if there have been changes
				SendConfigurationChangeAlerts(changes);
			}
		}

		public void SendConfigurationChangeAlerts(IList<ConfigurationAudit> changes)
		{
			//Look at what has changed since the agent's last report
			var history = new StringBuilder();

			//We will always include the current recipient regardless of other field changes
			var allRecipients = new List<string>();
			var currentRecipients = this.configurationRepository.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsRecipientsConfigurationChangeAlerts) ?? string.Empty;
			allRecipients.AddRange(GetEmailList(currentRecipients)); 

			//Add prior recipients from the trail - this handles the case where the recipient changes multiple times before the agent sends out the notice
			var oldRecipients =
				changes.Where(x => x.FieldName == ConfigurationAuditFields.NotificationsRecipientsConfigurationChangeAlerts)
					.SelectMany(x => GetEmailList(x.OldValue));
			var newRecipients =
				changes.Where(x => x.FieldName == ConfigurationAuditFields.NotificationsRecipientsConfigurationChangeAlerts)
					.SelectMany(x => GetEmailList(x.NewValue));
			allRecipients.AddRange(oldRecipients);
			allRecipients.AddRange(newRecipients);
			var recipients = allRecipients
				.Where(x => !string.IsNullOrEmpty(x))
				.Select(x => x.Trim().ToLower())
				.Distinct().ToList();

			//Send if there's someone to receive
			if (recipients.Any())
			{
				foreach (var audit in changes)
				{
					var oldValOrBlank = string.IsNullOrEmpty(audit.OldValue)
						? "(blank)"
						: audit.OldValue;
					var newValOrBlank = string.IsNullOrEmpty(audit.NewValue)
						? "(blank)"
						: audit.NewValue;

					history.AppendLine(
						string.IsNullOrEmpty(audit.ServerName)
							? $"{audit.FieldName} was changed from {oldValOrBlank} to {newValOrBlank} by user account {audit.UserId} ({audit.CreatedOn:g} UTC)"
							: $"{audit.FieldName} for server {audit.ServerName} was changed from {oldValOrBlank} to {newValOrBlank} by user account {audit.UserId} ({audit.CreatedOn:g} UTC)");
				}

				var config = configurationService.GetConfiguration();
				var settings = this.GetSmtpConfiguration();
				var instanceName = Database.ReadInstanceName();
				var message = BuildConfigurationChangeAlert(config, instanceName, history.ToString());
				var mailMessage = BuildMailMessage(settings, recipients, "Performance Dashboard - Configuration Change Alert", message);
				var client = this.smtpClientFactory.CreateSmtpClient(settings);
				client.Send(mailMessage);
			}
		}

		public void SendWeeklyScoreAlerts()
		{
			//Gatekeeper #1: We will only try to send notifications if they're turned on and ratings exist
			var sendNotifications = Convert.ToBoolean(this.configurationRepository.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.SendHourlyNotifications) ?? "False");
			var ratingsExist = ReportingService.LookingGlassHasRun();
			if (!sendNotifications || !ratingsExist)
				return;

			//Gatekeeper #2: Only send an alert if there's a recipient address
			var recipients = this.configurationRepository.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsRecipientsWeeklyScoreAlerts);
			if (string.IsNullOrEmpty(recipients))
				return;

			//Gatekeeper #3: Only send the alert if we're below the threshold now or we were last time
			var overallScores = ReportingService.GetOverallScores();
			int threshold;
			var thresholdConfig = this.configurationRepository.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsThresholdWeeklyScore) ?? "90";
			int.TryParse(thresholdConfig, out threshold);
			var belowThreshold = overallScores.WeeklyScore < threshold;
			var belowThresholdLast = Convert.ToBoolean(this.configurationRepository.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.LastWeeklyScoreBelowThreshold) ?? "False");
			if (!belowThreshold && !belowThresholdLast)
				return;

			//We're going to send the notification, so collect all weekly scores
			var scores = ReportingService.GetQualityOfServiceScores();
			this.configurationRepository.SetConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.LastWeeklyScoreBelowThreshold, belowThreshold.ToString());

			//Prepare and send the notice
			var settings = this.GetSmtpConfiguration();
			var instanceName = Database.ReadInstanceName();
			var message = BuildWeeklyScoreAlert(scores, instanceName, belowThreshold);
			if (string.IsNullOrEmpty(message))
			{
				return;
			}

			var client = this.smtpClientFactory.CreateSmtpClient(settings);
			var mailMessage = BuildMailMessage(settings, recipients, "Performance Dashboard - Weekly Score Alert", message);

			client.Send(mailMessage);
		}

		public void SendInfrastructurePerformanceForecast()
		{
			//Gatekeeper #1: We will only try to send notifications if they're turned on
			var sendNotifications = Convert.ToBoolean(this.configurationRepository.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.SendSystemLoadForecast) ?? "False");
			if (!sendNotifications)
				return;

			//Gatekeeper #2: We will only send if we have a recipient address
			var recipients = this.configurationRepository.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsRecipientsSystemLoadForecast);
			if (string.IsNullOrEmpty(recipients))
				return;

			//Gatekeeper #3: If no servers had poor performance, don't send an alert
			var forecast = Database.SystemLoadForecast();
			var scoreThreshold = Convert.ToInt32(this.configurationRepository.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsThresholdSystemLoadScore) ?? "90");
			if (!forecast.Any(x => x.RAMScore < scoreThreshold) &&
				!forecast.Any(x => x.CPUScore < scoreThreshold) &&
				!forecast.Any(x => x.SQLMemoryScore < scoreThreshold) &&
				!forecast.Any(x => x.SQLWaitsScore < scoreThreshold) &&
				!forecast.Any(x => x.SQLVirtualLogFilesScore < scoreThreshold))
				return;

			//Prepare and send the notice
			var settings = this.GetSmtpConfiguration();
			var instanceName = Database.ReadInstanceName();
			var message = BuildInfrastructurePerformanceForecast(instanceName, scoreThreshold, forecast);
			var client = this.smtpClientFactory.CreateSmtpClient(settings);
			var mailMessage = BuildMailMessage(settings, recipients, "Performance Dashboard - Infrastructure Performance Forecast", message);

			client.Send(mailMessage);
		}

		public void SendUserExperienceForecast()
		{
			//Gatekeeper #1: We will only try to send notifications if they're turned on
			var sendNotifications = Convert.ToBoolean(this.configurationRepository.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.SendUserExperienceForecast) ?? "False");
			if (!sendNotifications)
				return;

			//Gatekeeper #2: We will only send if we have a recipient address
			var recipients = this.configurationRepository.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsRecipientsUserExperienceForecast);
			if (string.IsNullOrEmpty(recipients))
				return;

			//Gatekeeper #3: If no servers had poor performance, don't send an alert
			//We need to look at every SQL server's VARSCAT data
			var servers = Database.GetRegisteredSQLServers();
			var forecasts = new List<KeyValuePair<string, int>>();

			foreach (var server in servers)
			{
				var score = Database.UserExperienceForecastForServer(server.Name);
				forecasts.Add(new KeyValuePair<string, int>(server.Name.ToUpper(), score));
			}

			var scoreThreshold = Convert.ToInt32(this.configurationRepository.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsThresholdUserExperienceScore) ?? "90");
			if (!forecasts.Any(x => x.Value < scoreThreshold))
				return;

			//Prepare and send the notice
			var settings = this.GetSmtpConfiguration();
			var instanceName = Database.ReadInstanceName();
			var message = BuildUserExperienceForecast(instanceName, scoreThreshold, forecasts);
			var client = this.smtpClientFactory.CreateSmtpClient(settings);
			var mailMessage = BuildMailMessage(settings, recipients, "Performance Dashboard - User Experience Forecast", message);

			client.Send(mailMessage);
		}

		/// <summary>
		/// Sends an alert indicating databases at risk due to late backups/DBCCs
		/// </summary>
		public void SendRecoverabilityIntegrityAlerts()
		{
			// Gatekeeper #1: We will only try to send notifications if they're turned on
			var sendNotifications = Convert.ToBoolean(this.configurationRepository.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.SendBackupDbccNotifications) ?? "False");
			if (!sendNotifications)
				return;

			// Gatekeeper #2: We will only send if we have a recipient address
			var recipients = this.configurationRepository.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsRecipientsBackupDBCCAlerts);
			if (string.IsNullOrEmpty(recipients))
				return;

			// Gatekeeper #3: We will only send if at least one database is nearing violation
			var dbInfo = ReportingService.DatabasesNearingViolation();
			if (!dbInfo.Backups.Any() && !dbInfo.DBCC.Any())
				return;

			// Prepare and send the notice
			var settings = this.GetSmtpConfiguration();
			var instanceName = Database.ReadInstanceName();
			var monitoringFailedServers = ReportingService.ListMonitoringFailedServers();
			var monitoringFailedDatabases = ReportingService.ListMonitoringFailedDatabases();
			var message = BuildRecoverabilityIntegrityAlert(instanceName, dbInfo, monitoringFailedServers, monitoringFailedDatabases);
			var mailMessage = BuildMailMessage(settings, recipients, "Performance Dashboard - Recoverability/Integrity Alert", message);
			var client = this.smtpClientFactory.CreateSmtpClient(settings);
			client.Send(mailMessage);
		}

		public IList<string> GetEmailList(string recipients)
		{
			if (string.IsNullOrEmpty(recipients))
			{
				return new List<string>();
			}

			return recipients.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
		}

		public MailAddress MailAddressFromString(string address)
		{
			try
			{
				return new MailAddress(address);
			}
			catch (Exception ex)
			{
				throw new Exception($"Invalid email address: {address}", ex);
			}
		}

		public MailMessage BuildMailMessage(SmtpSettings settings, string recipientList, string subject, string message)
		{
			var recipients = GetEmailList(recipientList);

			return BuildMailMessage(settings, recipients, subject, message);
		}

		public MailMessage BuildMailMessage(SmtpSettings settings, IList<string> recipients, string subject, string message)
		{
			if (string.IsNullOrEmpty(settings.EmailFrom) || string.IsNullOrEmpty(settings.Server))
				throw new ArgumentException("Relativity SMTP settings have not been configured.");

			var mailMessage = new MailMessage();
			var emailFroms = GetEmailList(settings.EmailFrom);

			mailMessage.From = MailAddressFromString(emailFroms.First());

			foreach (var recipient in recipients)
			{
				var address = MailAddressFromString(recipient);
				mailMessage.To.Add(address);
			}

			mailMessage.Body = message;
			mailMessage.BodyEncoding = Encoding.UTF8;
			mailMessage.Subject = subject;
			mailMessage.SubjectEncoding = Encoding.UTF8;
			return mailMessage;
		}

		public string BuildInfrastructurePerformanceForecast(string instanceName, int scoreThreshold, IList<SystemLoadForecast> forecasts)
		{
			var bodyTemplate = Resources.Forecast;
			var message = new StringBuilder();

			var badCpu = forecasts.Where(x => x.CPUScore < scoreThreshold).ToList();
			if (badCpu.Any())
			{
				message.AppendLine("High CPU utilization:");
				foreach (var server in badCpu)
				{
					message.AppendLine($" * {server.ServerName} ({server.ServerTypeName} Estimated Score: {server.CPUScore})");
				}
			}

			message.AppendLine();

			var badRam = forecasts.Where(x => x.RAMScore < scoreThreshold).ToList();
			if (badRam.Any())
			{
				message.AppendLine("High RAM utilization:");
				foreach (var server in badRam)
				{
					message.AppendLine($" * {server.ServerName} ({server.ServerTypeName} Estimated Score: {server.RAMScore})");
				}
			}

			message.AppendLine();

			var badMemory = forecasts.Where(x => x.ServerTypeId == 3 && x.SQLMemoryScore < scoreThreshold).ToList();
			if (badMemory.Any())
			{
				message.AppendLine("SQL memory signal:");
				foreach (var server in badMemory)
				{
					message.AppendLine($" * {server.ServerName} ({server.ServerTypeName} Estimated Score: {server.SQLMemoryScore})");
				}
			}

			message.AppendLine();

			var badWaits = forecasts.Where(x => x.ServerTypeId == 3 && x.SQLWaitsScore < scoreThreshold).ToList();
			if (badWaits.Any())
			{
				message.AppendLine("SQL waits:");
				foreach (var server in badWaits)
				{
					message.AppendLine($" * {server.ServerName} ({server.ServerTypeName} Estimated Score: {server.SQLWaitsScore})");
				}
			}

			var badVLF = forecasts.Where(x => x.ServerTypeId == 3 && x.SQLVirtualLogFilesScore < scoreThreshold).ToList();
			if (badVLF.Any())
			{
				message.AppendLine("SQL virtual log files:");
				foreach (var server in badVLF)
				{
					message.AppendLine($" * {server.ServerName} ({server.ServerTypeName} Estimated Score: {server.SQLVirtualLogFilesScore})");
				}
			}

			return string.Format(bodyTemplate, instanceName, message.ToString());
		}

		public string BuildUserExperienceForecast(string instanceName, int scoreThreshold, IList<KeyValuePair<string, int>> forecasts)
		{
			var bodyTemplate = Resources.Forecast;
			var message = new StringBuilder();

			message.AppendLine("High occurrence of long-running simple searches:");
			var badServers = forecasts.Where(x => x.Value < scoreThreshold);
			foreach (var server in badServers)
			{
				message.AppendLine($" * {server.Key} (Estimated Score: {server.Value})");
			}

			return string.Format(bodyTemplate, instanceName, message.ToString());
		}

		/// <summary>
		/// Reads Relativity's notification settings from EDDS.eddsdbo.Configuration
		/// </summary>
		/// <returns>Smtp settings</returns>
		private SmtpSettings GetSmtpConfiguration()
		{
			return this.Database.ReadRelativitySMTPSettings();
		}

		/// <summary>
		/// Produces the body of the weekly score report.
		/// Notifications need to include the quarterly and weekly scores in all areas.
		/// </summary>
		/// <param name="scores">The score summary</param>
		/// <param name="instanceName">The name of the relativity instance</param>
		/// <returns>The weekly email body</returns>
		private static string BuildQuarterlyScoreStatus(ScoreSummary scores, string instanceName)
		{
			return string.Format(Resources.Weekly,
				instanceName,
				scores.QuarterlyScore,
				scores.QuarterlyUserExperienceScore,
				scores.QuarterlySystemLoadScore,
				scores.QuarterlyIntegrityScore,
				scores.QuarterlyUptimeScore,
				scores.WeeklyScore,
				scores.WeeklyUserExperienceScore,
				scores.WeeklySystemLoadScore,
				scores.WeeklyIntegrityScore,
				scores.WeeklyUptimeScore);
		}

		/// <summary>
		/// Produces the body of the daily score report.
		/// Daily alerts should indicate why they're being sent (because the quarterly score dipped below the "good" threshold).
		/// Notifications need to include the quarterly and weekly scores in all areas.
		/// </summary>
		/// <param name="scores">The score summary</param>
		/// <param name="instanceName">The name of the relativity instance</param>
		/// <param name="scoreBelowThreshold">Indicates if the score is below the threshold</param>
		/// <returns>The quarterly email body</returns>
		private static string BuildQuarterlyScoreAlert(ScoreSummary scores, string instanceName, bool scoreBelowThreshold)
		{
			var scoreStatusMessage = scoreBelowThreshold
				? "is below the expected threshold. Please review the score report below and take action to ensure Best in Service eligibility."
				: "is now within the expected threshold.";

			return string.Format(Resources.Daily,
				instanceName,
				scoreStatusMessage,
				scores.QuarterlyScore,
				scores.QuarterlyUserExperienceScore,
				scores.QuarterlySystemLoadScore,
				scores.QuarterlyIntegrityScore,
				scores.QuarterlyUptimeScore,
				scores.WeeklyScore,
				scores.WeeklyUserExperienceScore,
				scores.WeeklySystemLoadScore,
				scores.WeeklyIntegrityScore,
				scores.WeeklyUptimeScore);
		}

		/// <summary>
		/// Produces the body of reconfiguration notifications.
		/// These need to indicate the ID of the user who modified the configuration and all settings.
		/// </summary>
		/// <param name="configuration">Configuration settings with alert info</param>
		/// <param name="instanceName">The name of the relativity instance</param>
		/// /// <param name="auditTrail">The audit trail</param>
		/// <returns>the alert email body</returns>
		private static string BuildConfigurationChangeAlert(PerformanceDashboardConfigurationSettings configuration, string instanceName, string auditTrail) =>
$@"Performance Dashboard configuration settings for {instanceName} have been updated (User ID: {configuration.LastModifiedBy}). Please review the settings below and take action as needed.

[DBCC Monitoring]
Enable View-Based Monitoring: {configuration.BackupDbccSettings.UseViewBasedMonitoring}
Enable Command-Based Monitoring: {configuration.BackupDbccSettings.UseCommandBasedMonitoring}

[Email Notifications]
Weekly Score Alerts
  Frequency: {configuration.NotificationSettings.WeeklyScoreAlert.Frequency}
  Threshold: {configuration.NotificationSettings.WeeklyScoreAlert.Threshold}
  Recipients: {configuration.NotificationSettings.WeeklyScoreAlert.Recipients}
  Enabled: {configuration.NotificationSettings.WeeklyScoreAlert.Enabled}

Infrastructure Performance Forecast
  Frequency: {configuration.NotificationSettings.SystemLoadForecast.Frequency}
  Threshold: {configuration.NotificationSettings.SystemLoadForecast.Threshold}
  Recipients: {configuration.NotificationSettings.SystemLoadForecast.Recipients}
  Enabled: {configuration.NotificationSettings.SystemLoadForecast.Enabled}

User Experience Forecast
  Frequency: {configuration.NotificationSettings.UserExperienceForecast.Frequency}
  Threshold: {configuration.NotificationSettings.UserExperienceForecast.Threshold}
  Recipients: {configuration.NotificationSettings.UserExperienceForecast.Recipients}
  Enabled: {configuration.NotificationSettings.UserExperienceForecast.Enabled}

Quarterly Score Alerts
  Frequency: {configuration.NotificationSettings.QuarterlyScoreAlert.Frequency}
  Threshold: {configuration.NotificationSettings.QuarterlyScoreAlert.Threshold}
  Recipients: {configuration.NotificationSettings.QuarterlyScoreAlert.Recipients}
  Enabled: {configuration.NotificationSettings.QuarterlyScoreAlert.Enabled}

Quarterly Score Status
  Frequency: {configuration.NotificationSettings.QuarterlyScoreStatus.Frequency}
  Recipients: {configuration.NotificationSettings.QuarterlyScoreStatus.Recipients}
  Enabled: {configuration.NotificationSettings.QuarterlyScoreStatus.Enabled}

Recoverability/Integrity Alerts
  Frequency: {configuration.NotificationSettings.BackupDBCCAlert.Frequency}
  Recipients: {configuration.NotificationSettings.BackupDBCCAlert.Recipients}
  Enabled: {configuration.NotificationSettings.BackupDBCCAlert.Enabled}

Configuration Change Alerts
  Recipients: {configuration.NotificationSettings.ConfigurationChangeAlert.Recipients}
  Enabled: {configuration.NotificationSettings.ConfigurationChangeAlert.Enabled}

Since the last report, the following changes have been made:

{auditTrail}

This is a reconfiguration notice from Performance Dashboard. Automatic notifications can be disabled via Performance Dashboard's configuration page.";

		private static string BuildWeeklyScoreAlert(IList<QualityOfServiceModel> scores, string instanceName, bool scoreBelowThreshold)
		{
			var scoreStatusMessage = scoreBelowThreshold
				? "are below the expected threshold. Please review the server breakdown below for details."
				: "are now within the expected threshold.";

			var servers = scores.OrderBy(x => x.WeeklyScore).ToList();
			var sb = new StringBuilder();
			foreach (var server in servers)
			{
				sb.AppendLine(string.Format("[{0}]{6}Weekly Score: {1:N2}{6}User Experience: {2:N2}{6}Infrastructure Performance: {3:N2}{6}Recoverability/Integrity: {4:N2}{6}Uptime: {5:N2}{6}",
					server.ServerName,
					server.WeeklyScore.GetValueOrDefault(100),
					server.WeeklyUserExperienceScore.GetValueOrDefault(100),
					server.WeeklySystemLoadScore.GetValueOrDefault(100),
					server.WeeklyIntegrityScore.GetValueOrDefault(100),
					server.WeeklyUptimeScore.GetValueOrDefault(100),
					Environment.NewLine));
				sb.AppendLine();
			}

			return string.Format(Resources.Hourly_ScoreAlert,
				instanceName,
				scoreStatusMessage,
				sb.ToString());
		}

		private static string BuildRecoverabilityIntegrityAlert(string instanceName, BackupDBCCNearingViolation dbInfo, IList<string> monitoringFailedServers, IList<string> monitoringFailedDatabases)
		{
			var bodyTemplate = Resources.BackupDBCC;
			var dangerBackups = new StringBuilder();
			var dangerDbcc = new StringBuilder();
			var failedMonitoring = new StringBuilder();
			if (dbInfo.Backups.Any())
			{
				dangerBackups.AppendLine();
				dangerBackups.AppendLine("Databases without recent backups:");
				dangerBackups.AppendLine();
				foreach (var backup in dbInfo.Backups)
					dangerBackups.AppendLine($"  * {backup.Database} ({backup.DaysMissed} days ago)");
			}

			if (dbInfo.DBCC.Any())
			{
				dangerDbcc.AppendLine();
				dangerDbcc.AppendLine("Databases without recent consistency checks:");
				dangerDbcc.AppendLine();
				foreach (var dbcc in dbInfo.DBCC)
					dangerDbcc.AppendLine($"  * {dbcc.Database} ({dbcc.DaysMissed} days ago)");
			}

			if (monitoringFailedServers.Count > 0)
			{
				failedMonitoring.AppendLine();
				failedMonitoring.Append("Some servers could not be monitored for backups and consistency checks. ");
				failedMonitoring.Append("The servers below may have been inaccessible, or the monitoring procedure may have failed on the server due to an error. ");
				failedMonitoring.AppendLine("Please review the QoS_GlassRunLog table in EDDSPerformance for details.");
				failedMonitoring.AppendLine();
				foreach (var server in monitoringFailedServers)
				{
					failedMonitoring.AppendLine($"  * {server}");
				}
			}

			if (monitoringFailedServers.Count > 0)
			{
				failedMonitoring.AppendLine();
				failedMonitoring.Append("Database-level errors occurred during command-based DBCC monitoring of the following databases. ");
				failedMonitoring.Append("This may indicate that the database was inaccessible or missing. ");
				failedMonitoring.AppendLine("Please review the QoS_FailedDatabases table in EDDSPerformance for details.");
				failedMonitoring.AppendLine();
				foreach (var db in monitoringFailedDatabases)
				{
					failedMonitoring.AppendLine($"  * {db}");
				}
			}

			return string.Format(bodyTemplate, instanceName, dangerBackups, dangerDbcc, failedMonitoring);
		}
	}
}