#pragma warning disable SA1519 // Braces must not be omitted from multi-line child statement

namespace kCura.PDB.Service.Services
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Enumerations;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.BISSummary.Models;
	using kCura.PDB.Core.Services;

	/// <summary>
	/// Handles operations related to configuration
	/// </summary>
	public class PdbConfigurationService : IPdbConfigurationService
	{
		private readonly ISqlServerRepository sqlServerRepository;
		private readonly IConfigurationRepository configurationRepository;
		private readonly IProcessControlRepository processControlRepository;

		public const string InvalidRecipientFormatMessage = @"The provided recipient list for {0} is invalid. Please enter valid email addresses separated by commas or semicolons.";

		public PdbConfigurationService(ISqlServerRepository database, IConfigurationRepository configurationRepository, IProcessControlRepository processControlRepository)
		{
			this.sqlServerRepository = database;
			this.configurationRepository = configurationRepository;
			this.processControlRepository = processControlRepository;
		}

		public PerformanceDashboardConfigurationSettings GetConfiguration()
		{
			var config = new PerformanceDashboardConfigurationSettings
			{
				BackupDbccSettings = new BackupDbccMonitoringConfigurationSettings(),
				NotificationSettings = new NotificationSettings()
			};

			var qualityLevels = new QualityIndicatorConfigurationService(this.configurationRepository).GetIndictatorConfiguration();

			// Populate last modified by
			config.LastModifiedBy = this.configurationRepository.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.ConfigurationEditedBy) ?? string.Empty;

			// Read DBCC settings
			config.BackupDbccSettings.UseViewBasedMonitoring = Convert.ToBoolean(this.configurationRepository.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.UseDbccViewMonitoring) ?? "False");
			config.BackupDbccSettings.UseCommandBasedMonitoring = Convert.ToBoolean(this.configurationRepository.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.UseDbccCommandMonitoring) ?? "False");
			config.BackupDbccSettings.ShowInvariantHistory = Convert.ToBoolean(this.configurationRepository.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.ShowInvariantBackupDbccHistory) ?? "False");

			// Read enabled settings
			config.NotificationSettings.BackupDBCCAlert.Enabled = Convert.ToBoolean(this.configurationRepository.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.SendBackupDbccNotifications) ?? "False");
			config.NotificationSettings.QuarterlyScoreStatus.Enabled = Convert.ToBoolean(this.configurationRepository.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.SendWeeklyNotifications) ?? "False");
			config.NotificationSettings.QuarterlyScoreAlert.Enabled = Convert.ToBoolean(this.configurationRepository.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.SendDailyNotifications) ?? "False");
			config.NotificationSettings.WeeklyScoreAlert.Enabled = Convert.ToBoolean(this.configurationRepository.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.SendHourlyNotifications) ?? "False");
			config.NotificationSettings.ConfigurationChangeAlert.Enabled = Convert.ToBoolean(this.configurationRepository.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.SendConfigurationChangeNotifications) ?? "False");
			config.NotificationSettings.SystemLoadForecast.Enabled = Convert.ToBoolean(this.configurationRepository.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.SendSystemLoadForecast) ?? "False");
			config.NotificationSettings.UserExperienceForecast.Enabled = Convert.ToBoolean(this.configurationRepository.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.SendUserExperienceForecast) ?? "False");

			// Read recipient settings
			config.NotificationSettings.WeeklyScoreAlert.Recipients = this.configurationRepository.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsRecipientsWeeklyScoreAlerts) ?? string.Empty;
			config.NotificationSettings.SystemLoadForecast.Recipients = this.configurationRepository.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsRecipientsSystemLoadForecast) ?? string.Empty;
			config.NotificationSettings.UserExperienceForecast.Recipients = this.configurationRepository.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsRecipientsUserExperienceForecast) ?? string.Empty;
			config.NotificationSettings.QuarterlyScoreAlert.Recipients = this.configurationRepository.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsRecipientsQuarterlyScoreAlerts) ?? string.Empty;
			config.NotificationSettings.QuarterlyScoreStatus.Recipients = this.configurationRepository.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsRecipientsQuarterlyScoreStatus) ?? string.Empty;
			config.NotificationSettings.BackupDBCCAlert.Recipients = this.configurationRepository.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsRecipientsBackupDBCCAlerts) ?? string.Empty;
			config.NotificationSettings.ConfigurationChangeAlert.Recipients = this.configurationRepository.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsRecipientsConfigurationChangeAlerts) ?? string.Empty;

			// Read threshold values
			config.NotificationSettings.WeeklyScoreAlert.Threshold = int.Parse(this.configurationRepository.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsThresholdWeeklyScore) ?? qualityLevels.PassScore.ToString());
			config.NotificationSettings.QuarterlyScoreAlert.Threshold = int.Parse(this.configurationRepository.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsThresholdQuarterlyScore) ?? qualityLevels.PassScore.ToString());
			config.NotificationSettings.SystemLoadForecast.Threshold = int.Parse(this.configurationRepository.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsThresholdSystemLoadScore) ?? qualityLevels.PassScore.ToString());
			config.NotificationSettings.UserExperienceForecast.Threshold = int.Parse(this.configurationRepository.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsThresholdUserExperienceScore) ?? qualityLevels.PassScore.ToString());

			// Read process control frequencies
			var processes = this.processControlRepository.ReadAll();
			var p = processes.FirstOrDefault(x => x.Id == ProcessControlId.RecoverabilityIntegrityAlerts);
			config.NotificationSettings.BackupDBCCAlert.Frequency = p != null
				? p.Frequency.GetValueOrDefault(1440)
				: 1440;
			p = processes.FirstOrDefault(x => x.Id == ProcessControlId.QuarterlyScoreAlerts);
			config.NotificationSettings.QuarterlyScoreAlert.Frequency = p != null
				? p.Frequency.GetValueOrDefault(1440)
				: 1440;
			p = processes.FirstOrDefault(x => x.Id == ProcessControlId.QuarterlyScoreStatus);
			config.NotificationSettings.QuarterlyScoreStatus.Frequency = p != null
				? p.Frequency.GetValueOrDefault(10080)
				: 10080;
			p = processes.FirstOrDefault(x => x.Id == ProcessControlId.InfrastructurePerformanceForecast);
			config.NotificationSettings.SystemLoadForecast.Frequency = p != null
				? p.Frequency.GetValueOrDefault(60)
				: 60;
			p = processes.FirstOrDefault(x => x.Id == ProcessControlId.UserExperienceForecast);
			config.NotificationSettings.UserExperienceForecast.Frequency = p != null
				? p.Frequency.GetValueOrDefault(60)
				: 60;
			p = processes.FirstOrDefault(x => x.Id == ProcessControlId.WeeklyScoreAlerts);
			config.NotificationSettings.WeeklyScoreAlert.Frequency = p != null
				? p.Frequency.GetValueOrDefault(60)
				: 60;

			return config;
		}

		public void SetConfiguration(PerformanceDashboardConfigurationSettings config)
		{
			//Preserve original configuration and compare with the new one
			var originalConfig = GetConfiguration();
			var changes = ListChanges(originalConfig, config);

			//General settings
			this.configurationRepository.SetConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.ConfigurationEditedBy, config.LastModifiedBy);
			this.configurationRepository.SetConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.UseDbccViewMonitoring, config.BackupDbccSettings.UseViewBasedMonitoring.ToString());
			this.configurationRepository.SetConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.UseDbccCommandMonitoring, config.BackupDbccSettings.UseCommandBasedMonitoring.ToString());
			this.configurationRepository.SetConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.ShowInvariantBackupDbccHistory, config.BackupDbccSettings.ShowInvariantHistory.ToString());

			//Recipient settings
			this.configurationRepository.SetConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsRecipientsWeeklyScoreAlerts, config.NotificationSettings.WeeklyScoreAlert.Recipients.Trim());
			this.configurationRepository.SetConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsRecipientsSystemLoadForecast, config.NotificationSettings.SystemLoadForecast.Recipients.Trim());
			this.configurationRepository.SetConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsRecipientsUserExperienceForecast, config.NotificationSettings.UserExperienceForecast.Recipients.Trim());
			this.configurationRepository.SetConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsRecipientsQuarterlyScoreAlerts, config.NotificationSettings.QuarterlyScoreAlert.Recipients.Trim());
			this.configurationRepository.SetConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsRecipientsQuarterlyScoreStatus, config.NotificationSettings.QuarterlyScoreStatus.Recipients.Trim());
			this.configurationRepository.SetConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsRecipientsBackupDBCCAlerts, config.NotificationSettings.BackupDBCCAlert.Recipients.Trim());
			this.configurationRepository.SetConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsRecipientsConfigurationChangeAlerts, config.NotificationSettings.ConfigurationChangeAlert.Recipients.Trim());

			//Enabled settings
			this.configurationRepository.SetConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.SendBackupDbccNotifications, config.NotificationSettings.BackupDBCCAlert.Enabled.ToString());
			this.configurationRepository.SetConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.SendWeeklyNotifications, config.NotificationSettings.QuarterlyScoreStatus.Enabled.ToString());
			this.configurationRepository.SetConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.SendDailyNotifications, config.NotificationSettings.QuarterlyScoreAlert.Enabled.ToString());
			this.configurationRepository.SetConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.SendHourlyNotifications, config.NotificationSettings.WeeklyScoreAlert.Enabled.ToString());
			this.configurationRepository.SetConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.SendConfigurationChangeNotifications, config.NotificationSettings.ConfigurationChangeAlert.Enabled.ToString());
			this.configurationRepository.SetConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.SendSystemLoadForecast, config.NotificationSettings.SystemLoadForecast.Enabled.ToString());
			this.configurationRepository.SetConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.SendUserExperienceForecast, config.NotificationSettings.UserExperienceForecast.Enabled.ToString());

			//Threshold value settings
			this.configurationRepository.SetConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsThresholdWeeklyScore, config.NotificationSettings.WeeklyScoreAlert.Threshold.ToString());
			this.configurationRepository.SetConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsThresholdQuarterlyScore, config.NotificationSettings.QuarterlyScoreAlert.Threshold.ToString());
			this.configurationRepository.SetConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsThresholdSystemLoadScore, config.NotificationSettings.SystemLoadForecast.Threshold.ToString());
			this.configurationRepository.SetConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsThresholdUserExperienceScore, config.NotificationSettings.UserExperienceForecast.Threshold.ToString());

			//Frequency settings
			this.processControlRepository.SetProcessFrequency(ProcessControlId.RecoverabilityIntegrityAlerts, config.NotificationSettings.BackupDBCCAlert.Frequency);
			this.processControlRepository.SetProcessFrequency(ProcessControlId.QuarterlyScoreAlerts, config.NotificationSettings.QuarterlyScoreAlert.Frequency);
			this.processControlRepository.SetProcessFrequency(ProcessControlId.WeeklyScoreAlerts, config.NotificationSettings.WeeklyScoreAlert.Frequency);
			this.processControlRepository.SetProcessFrequency(ProcessControlId.InfrastructurePerformanceForecast, config.NotificationSettings.SystemLoadForecast.Frequency);
			this.processControlRepository.SetProcessFrequency(ProcessControlId.UserExperienceForecast, config.NotificationSettings.UserExperienceForecast.Frequency);
			this.processControlRepository.SetProcessFrequency(ProcessControlId.QuarterlyScoreStatus, config.NotificationSettings.QuarterlyScoreStatus.Frequency);

			//Audit any changes that happened
			var triggerEmailAlert = originalConfig.NotificationSettings.ConfigurationChangeAlert.Enabled || config.NotificationSettings.ConfigurationChangeAlert.Enabled;
			this.sqlServerRepository.AuditConfigurationChanges(changes, triggerEmailAlert);
		}

		public List<ConfigurationAudit> ListChanges(PerformanceDashboardConfigurationSettings previous, PerformanceDashboardConfigurationSettings current)
		{
			var now = DateTime.UtcNow;
			int userId = -1;
			int.TryParse(current.LastModifiedBy, out userId);
			var changes = new List<ConfigurationAudit>();

			//Check DBCC settings
			if (!previous.BackupDbccSettings.UseCommandBasedMonitoring.Equals(current.BackupDbccSettings.UseCommandBasedMonitoring))
				changes.Add(new ConfigurationAudit
				{
					FieldName = ConfigurationAuditFields.EnableCommandBasedMonitoring,
					OldValue = previous.BackupDbccSettings.UseCommandBasedMonitoring.ToString(),
					NewValue = current.BackupDbccSettings.UseCommandBasedMonitoring.ToString(),
				});
			if (!previous.BackupDbccSettings.UseViewBasedMonitoring.Equals(current.BackupDbccSettings.UseViewBasedMonitoring))
				changes.Add(new ConfigurationAudit
				{
					FieldName = ConfigurationAuditFields.EnableViewBasedMonitoring,
					OldValue = previous.BackupDbccSettings.UseViewBasedMonitoring.ToString(),
					NewValue = current.BackupDbccSettings.UseViewBasedMonitoring.ToString(),
				});
			if (!previous.BackupDbccSettings.ShowInvariantHistory.Equals(current.BackupDbccSettings.ShowInvariantHistory))
				changes.Add(new ConfigurationAudit
				{
					FieldName = ConfigurationAuditFields.ShowInvariantHistory,
					OldValue = previous.BackupDbccSettings.ShowInvariantHistory.ToString(),
					NewValue = current.BackupDbccSettings.ShowInvariantHistory.ToString(),
				});

			//Check notifications recipients
			if (!previous.NotificationSettings.WeeklyScoreAlert.Recipients.Equals(current.NotificationSettings.WeeklyScoreAlert.Recipients))
				changes.Add(new ConfigurationAudit
				{
					FieldName = ConfigurationAuditFields.NotificationsRecipientsWeeklyScoreAlerts,
					OldValue = previous.NotificationSettings.WeeklyScoreAlert.Recipients,
					NewValue = current.NotificationSettings.WeeklyScoreAlert.Recipients,
				});
			if (!previous.NotificationSettings.SystemLoadForecast.Recipients.Equals(current.NotificationSettings.SystemLoadForecast.Recipients))
				changes.Add(new ConfigurationAudit
				{
					FieldName = ConfigurationAuditFields.NotificationsRecipientsSystemLoadForecast,
					OldValue = previous.NotificationSettings.SystemLoadForecast.Recipients,
					NewValue = current.NotificationSettings.SystemLoadForecast.Recipients,
				});
			if (!previous.NotificationSettings.UserExperienceForecast.Recipients.Equals(current.NotificationSettings.UserExperienceForecast.Recipients))
				changes.Add(new ConfigurationAudit
				{
					FieldName = ConfigurationAuditFields.NotificationsRecipientsUserExperienceForecast,
					OldValue = previous.NotificationSettings.UserExperienceForecast.Recipients,
					NewValue = current.NotificationSettings.UserExperienceForecast.Recipients,
				});
			if (!previous.NotificationSettings.QuarterlyScoreAlert.Recipients.Equals(current.NotificationSettings.QuarterlyScoreAlert.Recipients))
				changes.Add(new ConfigurationAudit
				{
					FieldName = ConfigurationAuditFields.NotificationsRecipientsQuarterlyScoreAlerts,
					OldValue = previous.NotificationSettings.QuarterlyScoreAlert.Recipients,
					NewValue = current.NotificationSettings.QuarterlyScoreAlert.Recipients,
				});
			if (!previous.NotificationSettings.QuarterlyScoreStatus.Recipients.Equals(current.NotificationSettings.QuarterlyScoreStatus.Recipients))
				changes.Add(new ConfigurationAudit
				{
					FieldName = ConfigurationAuditFields.NotificationsRecipientsQuarterlyScoreStatus,
					OldValue = previous.NotificationSettings.QuarterlyScoreStatus.Recipients,
					NewValue = current.NotificationSettings.QuarterlyScoreStatus.Recipients,
				});
			if (!previous.NotificationSettings.BackupDBCCAlert.Recipients.Equals(current.NotificationSettings.BackupDBCCAlert.Recipients))
				changes.Add(new ConfigurationAudit
				{
					FieldName = ConfigurationAuditFields.NotificationsRecipientsBackupDBCCAlerts,
					OldValue = previous.NotificationSettings.BackupDBCCAlert.Recipients,
					NewValue = current.NotificationSettings.BackupDBCCAlert.Recipients,
				});
			if (!previous.NotificationSettings.ConfigurationChangeAlert.Recipients.Equals(current.NotificationSettings.ConfigurationChangeAlert.Recipients))
				changes.Add(new ConfigurationAudit
				{
					FieldName = ConfigurationAuditFields.NotificationsRecipientsConfigurationChangeAlerts,
					OldValue = previous.NotificationSettings.ConfigurationChangeAlert.Recipients,
					NewValue = current.NotificationSettings.ConfigurationChangeAlert.Recipients,
				});

			//Enabled toggles
			if (!previous.NotificationSettings.BackupDBCCAlert.Enabled.Equals(current.NotificationSettings.BackupDBCCAlert.Enabled))
				changes.Add(new ConfigurationAudit
				{
					FieldName = ConfigurationAuditFields.SendBackupDbccNotifications,
					OldValue = previous.NotificationSettings.BackupDBCCAlert.Enabled.ToString(),
					NewValue = current.NotificationSettings.BackupDBCCAlert.Enabled.ToString(),
				});
			if (!previous.NotificationSettings.WeeklyScoreAlert.Enabled.Equals(current.NotificationSettings.WeeklyScoreAlert.Enabled))
				changes.Add(new ConfigurationAudit
				{
					FieldName = ConfigurationAuditFields.SendHourlyNotifications,
					OldValue = previous.NotificationSettings.WeeklyScoreAlert.Enabled.ToString(),
					NewValue = current.NotificationSettings.WeeklyScoreAlert.Enabled.ToString(),
				});
			if (!previous.NotificationSettings.QuarterlyScoreAlert.Enabled.Equals(current.NotificationSettings.QuarterlyScoreAlert.Enabled))
				changes.Add(new ConfigurationAudit
				{
					FieldName = ConfigurationAuditFields.SendDailyNotifications,
					OldValue = previous.NotificationSettings.QuarterlyScoreAlert.Enabled.ToString(),
					NewValue = current.NotificationSettings.QuarterlyScoreAlert.Enabled.ToString(),
					UserId = userId,
					CreatedOn = now
				});
			if (!previous.NotificationSettings.QuarterlyScoreStatus.Enabled.Equals(current.NotificationSettings.QuarterlyScoreStatus.Enabled))
				changes.Add(new ConfigurationAudit
				{
					FieldName = ConfigurationAuditFields.SendWeeklyNotifications,
					OldValue = previous.NotificationSettings.QuarterlyScoreStatus.Enabled.ToString(),
					NewValue = current.NotificationSettings.QuarterlyScoreStatus.Enabled.ToString(),
				});
			if (!previous.NotificationSettings.ConfigurationChangeAlert.Enabled.Equals(current.NotificationSettings.ConfigurationChangeAlert.Enabled))
				changes.Add(new ConfigurationAudit
				{
					FieldName = ConfigurationAuditFields.SendConfigurationChangeNotifications,
					OldValue = previous.NotificationSettings.ConfigurationChangeAlert.Enabled.ToString(),
					NewValue = current.NotificationSettings.ConfigurationChangeAlert.Enabled.ToString(),
				});
			if (!previous.NotificationSettings.SystemLoadForecast.Enabled.Equals(current.NotificationSettings.SystemLoadForecast.Enabled))
				changes.Add(new ConfigurationAudit
				{
					FieldName = ConfigurationAuditFields.SendSystemLoadForecast,
					OldValue = previous.NotificationSettings.SystemLoadForecast.Enabled.ToString(),
					NewValue = current.NotificationSettings.SystemLoadForecast.Enabled.ToString(),
				});
			if (!previous.NotificationSettings.UserExperienceForecast.Enabled.Equals(current.NotificationSettings.UserExperienceForecast.Enabled))
				changes.Add(new ConfigurationAudit
				{
					FieldName = ConfigurationAuditFields.SendUserExperienceForecast,
					OldValue = previous.NotificationSettings.UserExperienceForecast.Enabled.ToString(),
					NewValue = current.NotificationSettings.UserExperienceForecast.Enabled.ToString(),
				});

			//Frequency values
			if (!previous.NotificationSettings.BackupDBCCAlert.Frequency.Equals(current.NotificationSettings.BackupDBCCAlert.Frequency))
				changes.Add(new ConfigurationAudit
				{
					FieldName = ConfigurationAuditFields.BackupDbccNotificationFrequency,
					OldValue = previous.NotificationSettings.BackupDBCCAlert.Frequency.ToString(),
					NewValue = current.NotificationSettings.BackupDBCCAlert.Frequency.ToString(),
				});
			if (!previous.NotificationSettings.QuarterlyScoreAlert.Frequency.Equals(current.NotificationSettings.QuarterlyScoreAlert.Frequency))
				changes.Add(new ConfigurationAudit
				{
					FieldName = ConfigurationAuditFields.QuarterlyScoreNotificationFrequency,
					OldValue = previous.NotificationSettings.QuarterlyScoreAlert.Frequency.ToString(),
					NewValue = current.NotificationSettings.QuarterlyScoreAlert.Frequency.ToString(),
				});
			if (!previous.NotificationSettings.QuarterlyScoreStatus.Frequency.Equals(current.NotificationSettings.QuarterlyScoreStatus.Frequency))
				changes.Add(new ConfigurationAudit
				{
					FieldName = ConfigurationAuditFields.ScoreStatusNotificationFrequency,
					OldValue = previous.NotificationSettings.QuarterlyScoreStatus.Frequency.ToString(),
					NewValue = current.NotificationSettings.QuarterlyScoreStatus.Frequency.ToString(),
					UserId = userId,
					CreatedOn = now
				});
			if (!previous.NotificationSettings.SystemLoadForecast.Frequency.Equals(current.NotificationSettings.SystemLoadForecast.Frequency))
				changes.Add(new ConfigurationAudit
				{
					FieldName = ConfigurationAuditFields.SystemLoadNotificationFrequency,
					OldValue = previous.NotificationSettings.SystemLoadForecast.Frequency.ToString(),
					NewValue = current.NotificationSettings.SystemLoadForecast.Frequency.ToString(),
				});
			if (!previous.NotificationSettings.UserExperienceForecast.Frequency.Equals(current.NotificationSettings.UserExperienceForecast.Frequency))
				changes.Add(new ConfigurationAudit
				{
					FieldName = ConfigurationAuditFields.UserExperienceNotificationFrequency,
					OldValue = previous.NotificationSettings.UserExperienceForecast.Frequency.ToString(),
					NewValue = current.NotificationSettings.UserExperienceForecast.Frequency.ToString(),
				});
			if (!previous.NotificationSettings.WeeklyScoreAlert.Frequency.Equals(current.NotificationSettings.WeeklyScoreAlert.Frequency))
				changes.Add(new ConfigurationAudit
				{
					FieldName = ConfigurationAuditFields.WeeklyScoreNotificationFrequency,
					OldValue = previous.NotificationSettings.WeeklyScoreAlert.Frequency.ToString(),
					NewValue = current.NotificationSettings.WeeklyScoreAlert.Frequency.ToString(),
				});

			//Threshold values
			if (!previous.NotificationSettings.WeeklyScoreAlert.Threshold.Equals(current.NotificationSettings.WeeklyScoreAlert.Threshold))
				changes.Add(new ConfigurationAudit
				{
					FieldName = ConfigurationAuditFields.ThresholdsWeeklyScoreAlert,
					OldValue = previous.NotificationSettings.WeeklyScoreAlert.Threshold.ToString(),
					NewValue = current.NotificationSettings.WeeklyScoreAlert.Threshold.ToString(),
				});
			if (!previous.NotificationSettings.QuarterlyScoreAlert.Threshold.Equals(current.NotificationSettings.QuarterlyScoreAlert.Threshold))
				changes.Add(new ConfigurationAudit
				{
					FieldName = ConfigurationAuditFields.ThresholdsQuarterlyScoreAlert,
					OldValue = previous.NotificationSettings.QuarterlyScoreAlert.Threshold.ToString(),
					NewValue = current.NotificationSettings.QuarterlyScoreAlert.Threshold.ToString(),
				});
			if (!previous.NotificationSettings.SystemLoadForecast.Threshold.Equals(current.NotificationSettings.SystemLoadForecast.Threshold))
				changes.Add(new ConfigurationAudit
				{
					FieldName = ConfigurationAuditFields.ThresholdsSystemLoad,
					OldValue = previous.NotificationSettings.SystemLoadForecast.Threshold.ToString(),
					NewValue = current.NotificationSettings.SystemLoadForecast.Threshold.ToString(),
				});
			if (!previous.NotificationSettings.UserExperienceForecast.Threshold.Equals(current.NotificationSettings.UserExperienceForecast.Threshold))
				changes.Add(new ConfigurationAudit
				{
					FieldName = ConfigurationAuditFields.ThresholdsUserExperience,
					OldValue = previous.NotificationSettings.UserExperienceForecast.Threshold.ToString(),
					NewValue = current.NotificationSettings.UserExperienceForecast.Threshold.ToString(),
				});

			changes.ForEach(a => a.UserId = userId);
			changes.ForEach(a => a.CreatedOn = now);
			return changes;
		}

		public ValidationResult ValidateConfiguration(PerformanceDashboardConfigurationSettings config)
		{
			var result = new ValidationResult();
			this.sqlServerRepository.RefreshDbccTargets();

			if (!config.NotificationSettings.WeeklyScoreAlert.Valid)
			{
				result.Valid = false;
				result.Details = string.Format(InvalidRecipientFormatMessage, "Weekly Score Alerts");
			}
			else if (!config.NotificationSettings.SystemLoadForecast.Valid)
			{
				result.Valid = false;
				result.Details = string.Format(InvalidRecipientFormatMessage, "Infrastructure Performance Forecast");
			}
			else if (!config.NotificationSettings.UserExperienceForecast.Valid)
			{
				result.Valid = false;
				result.Details = string.Format(InvalidRecipientFormatMessage, "User Experience Forecast");
			}
			else if (!config.NotificationSettings.QuarterlyScoreAlert.Valid)
			{
				result.Valid = false;
				result.Details = string.Format(InvalidRecipientFormatMessage, "Quarterly Score Alerts");
			}
			else if (!config.NotificationSettings.QuarterlyScoreStatus.Valid)
			{
				result.Valid = false;
				result.Details = string.Format(InvalidRecipientFormatMessage, "Quarterly Score Status");
			}
			else if (!config.NotificationSettings.BackupDBCCAlert.Valid)
			{
				result.Valid = false;
				result.Details = string.Format(InvalidRecipientFormatMessage, "Recoverability/Integrity Alerts");
			}
			else if (!config.NotificationSettings.ConfigurationChangeAlert.Valid)
			{
				result.Valid = false;
				result.Details = string.Format(InvalidRecipientFormatMessage, "Configuration Change Alerts");
			}

			//Check thresholds for notifications
			else if (config.NotificationSettings.WeeklyScoreAlert.Threshold < 0 || config.NotificationSettings.WeeklyScoreAlert.Threshold > 100)
			{
				result.Valid = false;
				result.Details = "The provided weekly score threshold is invalid. Please enter an integer value (0-100).";
			}
			else if (config.NotificationSettings.QuarterlyScoreAlert.Threshold < 0 || config.NotificationSettings.QuarterlyScoreAlert.Threshold > 100)
			{
				result.Valid = false;
				result.Details = "The provided quarterly score threshold is invalid. Please enter an integer value (0-100).";
			}
			else if (config.NotificationSettings.SystemLoadForecast.Threshold < 0 || config.NotificationSettings.SystemLoadForecast.Threshold > 100)
			{
				result.Valid = false;
				result.Details = "The provided infrastructure performance score threshold is invalid. Please enter an integer value (0-100).";
			}
			else if (config.NotificationSettings.UserExperienceForecast.Threshold < 0 || config.NotificationSettings.UserExperienceForecast.Threshold > 100)
			{
				result.Valid = false;
				result.Details = "The provided user experience score threshold is invalid. Please enter an integer value (0-100).";
			}
			//Check DBCC settings
			else if (config.BackupDbccSettings.EnableDbccMonitoring && !config.BackupDbccSettings.UseCommandBasedMonitoring && !config.BackupDbccSettings.UseViewBasedMonitoring)
			{
				//If the overall toggle for DBCC monitoring is on, but neither method has been enabled, reject settings
				result.Valid = false;
				result.Details = "Please activate one or more DBCC monitoring methods to enable DBCC monitoring.";
			}
			else if (config.BackupDbccSettings.UseViewBasedMonitoring && !this.sqlServerRepository.ListDbccTargets().Any(x => x.IsActive))
			{
				//If view-based DBCC monitoring is being turned on, at least one target must be active
				result.Valid = false;
				result.Details = "No DBCC targets are currently active. To enable view-based DBCC monitoring, you must activate at least one target." +
					"<br/><br/><a href='/Relativity/CustomPages/60a1d0a3-2797-4fb3-a260-614cbfd3fa0d/DbccTarget.aspx'>Click here to configure targets.</a>";
			}

			//All checks passed - settings are valid
			return result;
		}

		public bool ElevatedScriptsInstalled() => this.sqlServerRepository.AdminScriptsInstalled();
	}
}
#pragma warning restore SA1519 // Braces must not be omitted from multi-line child statement