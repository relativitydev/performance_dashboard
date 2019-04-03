namespace kCura.PDB.Service.Tests.Services
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Enumerations;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.BISSummary.Models;
	using kCura.PDB.Service.Services;
	using Moq;
	using NUnit.Framework;

	[TestFixture, Category("Unit")]
	public class PdbConfigurationServiceTests
	{
		[SetUp]
		public void Setup()
		{
			sqlRepo = new Mock<ISqlServerRepository>();
			this.configurationRepository = new Mock<IConfigurationRepository>();
			this.processControlRepository = new Mock<IProcessControlRepository>();
			this.pdbConfigurationService = new PdbConfigurationService(sqlRepo.Object, this.configurationRepository.Object, this.processControlRepository.Object);
		}

		private Mock<ISqlServerRepository> sqlRepo;
		private Mock<IConfigurationRepository> configurationRepository;
		private Mock<IProcessControlRepository> processControlRepository;
		private PdbConfigurationService pdbConfigurationService;

		[Test]
		public void SetConfiguration_Success()
		{
			//Arrange
			this.processControlRepository.Setup(x => x.ReadAll()).Returns(new List<ProcessControl>());
			this.configurationRepository.Setup(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.ConfigurationEditedBy)).Returns((string)null);
			this.configurationRepository.Setup(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.PartnerName)).Returns((string)null);
			this.configurationRepository.Setup(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.UseDbccViewMonitoring)).Returns((string)null);
			this.configurationRepository.Setup(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.UseDbccCommandMonitoring)).Returns((string)null);
			this.configurationRepository.Setup(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.ShowInvariantBackupDbccHistory)).Returns((string)null);
			this.configurationRepository.Setup(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.SendBackupDbccNotifications)).Returns((string)null);
			this.configurationRepository.Setup(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.SendWeeklyNotifications)).Returns((string)null);
			this.configurationRepository.Setup(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.SendDailyNotifications)).Returns((string)null);
			this.configurationRepository.Setup(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.SendHourlyNotifications)).Returns((string)null);
			this.configurationRepository.Setup(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.SendConfigurationChangeNotifications)).Returns((string)null);
			this.configurationRepository.Setup(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.SendSystemLoadForecast)).Returns((string)null);
			this.configurationRepository.Setup(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.SendUserExperienceForecast)).Returns((string)null);
			this.configurationRepository.Setup(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsRecipientsWeeklyScoreAlerts)).Returns((string)null);
			this.configurationRepository.Setup(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsRecipientsSystemLoadForecast)).Returns((string)null);
			this.configurationRepository.Setup(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsRecipientsUserExperienceForecast)).Returns((string)null);
			this.configurationRepository.Setup(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsRecipientsQuarterlyScoreAlerts)).Returns((string)null);
			this.configurationRepository.Setup(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsRecipientsQuarterlyScoreStatus)).Returns((string)null);
			this.configurationRepository.Setup(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsRecipientsBackupDBCCAlerts)).Returns((string)null);
			this.configurationRepository.Setup(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsRecipientsConfigurationChangeAlerts)).Returns((string)null);
			this.configurationRepository.Setup(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsThresholdWeeklyScore)).Returns((string)null);
			this.configurationRepository.Setup(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsThresholdQuarterlyScore)).Returns((string)null);
			this.configurationRepository.Setup(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsThresholdSystemLoadScore)).Returns((string)null);
			this.configurationRepository.Setup(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsThresholdUserExperienceScore)).Returns((string)null);

			var config = new PerformanceDashboardConfigurationSettings
			{
				NotificationSettings = new NotificationSettings(),
				BackupDbccSettings = new BackupDbccMonitoringConfigurationSettings
				{
					UseCommandBasedMonitoring = false,
					UseViewBasedMonitoring = false
				}
			};

			//Act
			this.pdbConfigurationService.SetConfiguration(config);

			//Assert
			this.sqlRepo.VerifyAll();
		}

		[Test]
		public void SetConfiguration_UpdatesAuditTrail()
		{
			//Arrange
			this.configurationRepository.Setup(x => x.ReadConfigurationValue(It.IsAny<string>(), ConfigurationKeys.SendWeeklyNotifications)).Returns("False");
			this.processControlRepository.Setup(x => x.ReadAll()).Returns(new List<ProcessControl>());

			var config = new PerformanceDashboardConfigurationSettings
			{
				NotificationSettings = new NotificationSettings(),
				BackupDbccSettings = new BackupDbccMonitoringConfigurationSettings
				{
					UseCommandBasedMonitoring = false,
					UseViewBasedMonitoring = false
				}
			};

			//Act
			this.pdbConfigurationService.SetConfiguration(config);

			//Assert
			sqlRepo.Verify(x => x.AuditConfigurationChanges(It.IsAny<IList<ConfigurationAudit>>(), It.IsAny<bool>()), Times.Exactly(1));
		}

		[Test]
		public void GetConfiguration_ReturnsExistingRecipients()
		{
			//Arrange

			this.configurationRepository.Setup(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsRecipientsWeeklyScoreAlerts)).Returns("test@test.com");
			this.configurationRepository.Setup(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsRecipientsSystemLoadForecast)).Returns("test@test.com");
			this.configurationRepository.Setup(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsRecipientsUserExperienceForecast)).Returns("test@test.com");
			this.configurationRepository.Setup(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsRecipientsQuarterlyScoreAlerts)).Returns("test@test.com");
			this.configurationRepository.Setup(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsRecipientsQuarterlyScoreStatus)).Returns("test@test.com");
			this.configurationRepository.Setup(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsRecipientsBackupDBCCAlerts)).Returns("test@test.com");
			this.configurationRepository.Setup(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsRecipientsConfigurationChangeAlerts)).Returns("test@test.com");
			this.processControlRepository.Setup(x => x.ReadAll()).Returns(new List<ProcessControl>());

			//Act
			var settings = this.pdbConfigurationService.GetConfiguration().NotificationSettings;

			//Assert
			Assert.AreEqual("test@test.com", settings.WeeklyScoreAlert.Recipients, "Incorrect weekly score alert recipients list");
			Assert.AreEqual("test@test.com", settings.SystemLoadForecast.Recipients, "Incorrect system load forecast recipients list");
			Assert.AreEqual("test@test.com", settings.UserExperienceForecast.Recipients, "Incorrect user experience forecast recipients list");
			Assert.AreEqual("test@test.com", settings.QuarterlyScoreAlert.Recipients, "Incorrect quarterly score alert recipients list");
			Assert.AreEqual("test@test.com", settings.QuarterlyScoreStatus.Recipients, "Incorrect quarterly score status recipients list");
			Assert.AreEqual("test@test.com", settings.BackupDBCCAlert.Recipients, "Incorrect backup/DBCC alert recipients list");
			Assert.AreEqual("test@test.com", settings.ConfigurationChangeAlert.Recipients, "Incorrect configuration change alert recipients list");

			this.configurationRepository.Verify(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsRecipientsWeeklyScoreAlerts), Times.Exactly(1));
			this.configurationRepository.Verify(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsRecipientsSystemLoadForecast), Times.Exactly(1));
			this.configurationRepository.Verify(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsRecipientsUserExperienceForecast), Times.Exactly(1));
			this.configurationRepository.Verify(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsRecipientsQuarterlyScoreAlerts), Times.Exactly(1));
			this.configurationRepository.Verify(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsRecipientsQuarterlyScoreStatus), Times.Exactly(1));
			this.configurationRepository.Verify(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsRecipientsBackupDBCCAlerts), Times.Exactly(1));
			this.configurationRepository.Verify(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsRecipientsConfigurationChangeAlerts), Times.Exactly(1));
		}

		[Test]
		public void GetConfiguration_ReturnsEmptyStringWhenNotSet()
		{
			//Arrange
			this.configurationRepository.Setup(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsRecipientsWeeklyScoreAlerts))
				.Returns((string)null);
			this.processControlRepository.Setup(x => x.ReadAll()).Returns(new List<ProcessControl>());

			//Act
			var param = this.pdbConfigurationService.GetConfiguration().NotificationSettings.WeeklyScoreAlert.Recipients;

			//Assert
			Assert.AreEqual(String.Empty, param);
			this.configurationRepository.Verify(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsRecipientsWeeklyScoreAlerts), Times.Exactly(1));
		}

		[Test]
		public void SetConfiguration_SetsRecipientValue()
		{
			//Arrange

			this.configurationRepository.Setup(x => x.SetConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsRecipientsWeeklyScoreAlerts, It.IsAny<string>())).Callback(() =>
					this.configurationRepository.Setup(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsRecipientsWeeklyScoreAlerts)).Returns("test@test.com"));
			this.processControlRepository.Setup(x => x.ReadAll()).Returns(new List<ProcessControl>());

			var config = new PerformanceDashboardConfigurationSettings
			{
				NotificationSettings = new NotificationSettings()
				{
					WeeklyScoreAlert = new AlertConfiguration
					{
						Enabled = true,
						Recipients = "test@test.com"
					}
				},
				BackupDbccSettings = new BackupDbccMonitoringConfigurationSettings
				{
					UseCommandBasedMonitoring = false,
					UseViewBasedMonitoring = false
				}
			};

			//Act
			this.pdbConfigurationService.SetConfiguration(config);
			var param = this.pdbConfigurationService.GetConfiguration().NotificationSettings.WeeklyScoreAlert.Recipients;

			//Assert
			Assert.AreEqual("test@test.com", param);
			this.configurationRepository.Verify(x => x.SetConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsRecipientsWeeklyScoreAlerts, "test@test.com"), Times.Exactly(1));
			this.configurationRepository.Verify(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsRecipientsWeeklyScoreAlerts), Times.Exactly(2)); //Once when we Set, again when we check the updated value
		}

		[Test]
		public void GetConfiguration_ReturnsExistingSendNotificationsPreference()
		{
			//Arrange

			this.configurationRepository.Setup(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.SendWeeklyNotifications)).Returns("True");
			this.processControlRepository.Setup(x => x.ReadAll()).Returns(new List<ProcessControl>());

			//Act
			var param = this.pdbConfigurationService.GetConfiguration().NotificationSettings.QuarterlyScoreStatus.Enabled;

			//Assert
			Assert.IsTrue(param);
			this.configurationRepository.Verify(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.SendWeeklyNotifications), Times.Exactly(1));
		}

		[Test]
		public void GetConfiguration_ReturnsSendNotificationsFalseByDefault()
		{
			//Arrange
			String dummy = null;

			this.configurationRepository.Setup(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.SendWeeklyNotifications)).Returns(dummy);
			this.processControlRepository.Setup(x => x.ReadAll()).Returns(new List<ProcessControl>());

			//Act
			var param = this.pdbConfigurationService.GetConfiguration().NotificationSettings.WeeklyScoreAlert.Enabled;

			//Assert
			Assert.IsFalse(param);
			this.configurationRepository.Verify(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.SendWeeklyNotifications), Times.Exactly(1));
		}

		[Test]
		public void SetConfiguration_SetsSendNotificationPreference()
		{
			//Arrange

			this.configurationRepository.Setup(x => x.SetConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.SendWeeklyNotifications, "True")).Callback(() =>
					this.configurationRepository.Setup(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.SendWeeklyNotifications)).Returns("True"));
			this.processControlRepository.Setup(x => x.ReadAll()).Returns(new List<ProcessControl>());

			var config = new PerformanceDashboardConfigurationSettings
			{
				NotificationSettings = new NotificationSettings()
				{
					QuarterlyScoreStatus = new AlertConfiguration
					{
						Enabled = true,
						Recipients = string.Empty
					}
				},
				BackupDbccSettings = new BackupDbccMonitoringConfigurationSettings
				{
					UseCommandBasedMonitoring = false,
					UseViewBasedMonitoring = false
				}
			};

			//Act
			this.pdbConfigurationService.SetConfiguration(config);
			var param = this.pdbConfigurationService.GetConfiguration().NotificationSettings.QuarterlyScoreStatus.Enabled;

			//Assert
			Assert.IsTrue(param);
			this.configurationRepository.Verify(x => x.SetConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.SendWeeklyNotifications, "True"), Times.Exactly(1));
			this.configurationRepository.Verify(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.SendWeeklyNotifications), Times.Exactly(2)); //Once when we Set, again when we check the updated value
		}

		[Test]
		public void GetConfiguration_PopulatesNotificationFrequencies()
		{
			//Arrange
			this.processControlRepository.Setup(x => x.ReadAll()).Returns(new List<ProcessControl>()
			{
				new ProcessControl { Id = ProcessControlId.QuarterlyScoreAlerts, Frequency = 1 },
				new ProcessControl { Id = ProcessControlId.QuarterlyScoreStatus, Frequency = 2 },
				new ProcessControl { Id = ProcessControlId.RecoverabilityIntegrityAlerts, Frequency = 3 },
				new ProcessControl { Id = ProcessControlId.InfrastructurePerformanceForecast, Frequency = 4 },
				new ProcessControl { Id = ProcessControlId.UserExperienceForecast, Frequency = 6 },
				new ProcessControl { Id = ProcessControlId.WeeklyScoreAlerts, Frequency = 7 }
			});
			this.configurationRepository.Setup(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.ConfigurationEditedBy)).Returns((string)null);
			this.configurationRepository.Setup(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.PartnerName)).Returns((string)null);
			this.configurationRepository.Setup(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.UseDbccViewMonitoring)).Returns((string)null);
			this.configurationRepository.Setup(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.UseDbccCommandMonitoring)).Returns((string)null);
			this.configurationRepository.Setup(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.ShowInvariantBackupDbccHistory)).Returns((string)null);
			this.configurationRepository.Setup(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.SendBackupDbccNotifications)).Returns((string)null);
			this.configurationRepository.Setup(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.SendWeeklyNotifications)).Returns((string)null);
			this.configurationRepository.Setup(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.SendDailyNotifications)).Returns((string)null);
			this.configurationRepository.Setup(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.SendHourlyNotifications)).Returns((string)null);
			this.configurationRepository.Setup(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.SendConfigurationChangeNotifications)).Returns((string)null);
			this.configurationRepository.Setup(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.SendSystemLoadForecast)).Returns((string)null);
			this.configurationRepository.Setup(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.SendUserExperienceForecast)).Returns((string)null);
			this.configurationRepository.Setup(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsRecipientsWeeklyScoreAlerts)).Returns((string)null);
			this.configurationRepository.Setup(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsRecipientsSystemLoadForecast)).Returns((string)null);
			this.configurationRepository.Setup(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsRecipientsUserExperienceForecast)).Returns((string)null);
			this.configurationRepository.Setup(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsRecipientsQuarterlyScoreAlerts)).Returns((string)null);
			this.configurationRepository.Setup(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsRecipientsQuarterlyScoreStatus)).Returns((string)null);
			this.configurationRepository.Setup(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsRecipientsBackupDBCCAlerts)).Returns((string)null);
			this.configurationRepository.Setup(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsRecipientsConfigurationChangeAlerts)).Returns((string)null);
			this.configurationRepository.Setup(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsThresholdWeeklyScore)).Returns((string)null);
			this.configurationRepository.Setup(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsThresholdQuarterlyScore)).Returns((string)null);
			this.configurationRepository.Setup(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsThresholdSystemLoadScore)).Returns((string)null);
			this.configurationRepository.Setup(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsThresholdUserExperienceScore)).Returns((string)null);

			//Act
			var config = this.pdbConfigurationService.GetConfiguration();

			//Assert
			Assert.AreEqual(1, config.NotificationSettings.QuarterlyScoreAlert.Frequency, "Incorrect quarterly score notification frequency");
			Assert.AreEqual(2, config.NotificationSettings.QuarterlyScoreStatus.Frequency, "Incorrect score status notification frequency");
			Assert.AreEqual(3, config.NotificationSettings.BackupDBCCAlert.Frequency, "Incorrect backup/DBCC notification frequency");
			Assert.AreEqual(4, config.NotificationSettings.SystemLoadForecast.Frequency, "Incorrect system load notification frequency");
			Assert.AreEqual(6, config.NotificationSettings.UserExperienceForecast.Frequency, "Incorrect user experience notification frequency");
			Assert.AreEqual(7, config.NotificationSettings.WeeklyScoreAlert.Frequency, "Incorrect weekly score notification frequency");
		}

		[Test]
		public void SetConfiguration_SetsNotificationFrequencies()
		{
			//Arrange
			sqlRepo.Setup(
				x => x.ConfigurationRepository.SetConfigurationValue(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));
			this.processControlRepository.Setup(x => x.ReadAll()).Returns(new List<ProcessControl>());
			this.configurationRepository.Setup(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.ConfigurationEditedBy)).Returns((string)null);
			this.configurationRepository.Setup(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.PartnerName)).Returns((string)null);
			this.configurationRepository.Setup(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.UseDbccViewMonitoring)).Returns((string)null);
			this.configurationRepository.Setup(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.UseDbccCommandMonitoring)).Returns((string)null);
			this.configurationRepository.Setup(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.ShowInvariantBackupDbccHistory)).Returns((string)null);
			this.configurationRepository.Setup(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.SendBackupDbccNotifications)).Returns((string)null);
			this.configurationRepository.Setup(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.SendWeeklyNotifications)).Returns((string)null);
			this.configurationRepository.Setup(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.SendDailyNotifications)).Returns((string)null);
			this.configurationRepository.Setup(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.SendHourlyNotifications)).Returns((string)null);
			this.configurationRepository.Setup(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.SendConfigurationChangeNotifications)).Returns((string)null);
			this.configurationRepository.Setup(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.SendSystemLoadForecast)).Returns((string)null);
			this.configurationRepository.Setup(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.SendUserExperienceForecast)).Returns((string)null);
			this.configurationRepository.Setup(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsRecipientsWeeklyScoreAlerts)).Returns((string)null);
			this.configurationRepository.Setup(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsRecipientsSystemLoadForecast)).Returns((string)null);
			this.configurationRepository.Setup(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsRecipientsUserExperienceForecast)).Returns((string)null);
			this.configurationRepository.Setup(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsRecipientsQuarterlyScoreAlerts)).Returns((string)null);
			this.configurationRepository.Setup(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsRecipientsQuarterlyScoreStatus)).Returns((string)null);
			this.configurationRepository.Setup(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsRecipientsBackupDBCCAlerts)).Returns((string)null);
			this.configurationRepository.Setup(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsRecipientsConfigurationChangeAlerts)).Returns((string)null);
			this.configurationRepository.Setup(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsThresholdWeeklyScore)).Returns((string)null);
			this.configurationRepository.Setup(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsThresholdQuarterlyScore)).Returns((string)null);
			this.configurationRepository.Setup(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsThresholdSystemLoadScore)).Returns((string)null);
			this.configurationRepository.Setup(x => x.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.NotificationsThresholdUserExperienceScore)).Returns((string)null);

			var config = new PerformanceDashboardConfigurationSettings
			{
				NotificationSettings = new NotificationSettings
				{
					BackupDBCCAlert = new AlertConfiguration { Frequency = 1 },
					QuarterlyScoreAlert = new AlertConfiguration { Frequency = 2 },
					QuarterlyScoreStatus = new AlertConfiguration { Frequency = 3 },
					SystemLoadForecast = new AlertConfiguration { Frequency = 4 },
					UserExperienceForecast = new AlertConfiguration { Frequency = 5 },
					WeeklyScoreAlert = new AlertConfiguration { Frequency = 7 }
				},
				BackupDbccSettings = new BackupDbccMonitoringConfigurationSettings()
			};

			//Act
			this.pdbConfigurationService.SetConfiguration(config);

			//Assert
			this.processControlRepository.Verify(x => x.SetProcessFrequency(ProcessControlId.RecoverabilityIntegrityAlerts, 1), Times.Exactly(1));
			this.processControlRepository.Verify(x => x.SetProcessFrequency(ProcessControlId.QuarterlyScoreAlerts, 2), Times.Exactly(1));
			this.processControlRepository.Verify(x => x.SetProcessFrequency(ProcessControlId.QuarterlyScoreStatus, 3), Times.Exactly(1));
			this.processControlRepository.Verify(x => x.SetProcessFrequency(ProcessControlId.InfrastructurePerformanceForecast, 4), Times.Exactly(1));
			this.processControlRepository.Verify(x => x.SetProcessFrequency(ProcessControlId.UserExperienceForecast, 5), Times.Exactly(1));
			this.processControlRepository.Verify(x => x.SetProcessFrequency(ProcessControlId.WeeklyScoreAlerts, 7), Times.Exactly(1));
		}

		[Test]
		public void Validate_AcceptsValidEmailForAllNotificationTypes()
		{
			//Arrange
			var config = new PerformanceDashboardConfigurationSettings
			{
				BackupDbccSettings = new BackupDbccMonitoringConfigurationSettings(),
				NotificationSettings = new NotificationSettings()
				{
					WeeklyScoreAlert = new AlertConfiguration
					{
						Enabled = true,
						Recipients = "test@email.com"
					},
					SystemLoadForecast = new AlertConfiguration
					{
						Enabled = true,
						Recipients = "test@email.com"
					},
					UserExperienceForecast = new AlertConfiguration
					{
						Enabled = true,
						Recipients = "test@email.com"
					},
					QuarterlyScoreAlert = new AlertConfiguration
					{
						Enabled = true,
						Recipients = "test@email.com"
					},
					QuarterlyScoreStatus = new AlertConfiguration
					{
						Enabled = true,
						Recipients = "test@email.com"
					},
					BackupDBCCAlert = new AlertConfiguration
					{
						Enabled = true,
						Recipients = "test@email.com"
					},
					ConfigurationChangeAlert = new AlertConfiguration
					{
						Enabled = true,
						Recipients = "test@email.com"
					}
				}
			};

			//Act
			var result = this.pdbConfigurationService.ValidateConfiguration(config);

			//Assert
			Assert.IsTrue(result.Valid);
		}

		[Test]
		public void Validate_WeeklyScoreAlert_RejectsInvalidEmail()
		{
			//Arrange
			var config = new PerformanceDashboardConfigurationSettings
			{
				BackupDbccSettings = new BackupDbccMonitoringConfigurationSettings(),
				NotificationSettings = new NotificationSettings()
				{
					WeeklyScoreAlert = new AlertConfiguration
					{
						Enabled = true,
						Recipients = "invalidemail"
					}
				}
			};

			//Act
			var result = this.pdbConfigurationService.ValidateConfiguration(config);

			//Assert
			Assert.IsFalse(result.Valid);
		}

		[Test]
		public void Validate_SystemLoadForecast_RejectsInvalidEmail()
		{
			//Arrange
			var config = new PerformanceDashboardConfigurationSettings
			{
				BackupDbccSettings = new BackupDbccMonitoringConfigurationSettings(),
				NotificationSettings = new NotificationSettings()
				{
					SystemLoadForecast = new AlertConfiguration
					{
						Enabled = true,
						Recipients = "invalidemail"
					}
				}
			};

			//Act
			var result = this.pdbConfigurationService.ValidateConfiguration(config);

			//Assert
			Assert.IsFalse(result.Valid);
		}

		[Test]
		public void Validate_UserExperienceForecast_RejectsInvalidEmail()
		{
			//Arrange
			var config = new PerformanceDashboardConfigurationSettings
			{
				BackupDbccSettings = new BackupDbccMonitoringConfigurationSettings(),
				NotificationSettings = new NotificationSettings()
				{
					UserExperienceForecast = new AlertConfiguration
					{
						Enabled = true,
						Recipients = "invalidemail"
					}
				}
			};

			//Act
			var result = this.pdbConfigurationService.ValidateConfiguration(config);

			//Assert
			Assert.IsFalse(result.Valid);
		}

		[Test]
		public void Validate_QuarterlyScoreAlert_RejectsInvalidEmail()
		{
			//Arrange
			var config = new PerformanceDashboardConfigurationSettings
			{
				BackupDbccSettings = new BackupDbccMonitoringConfigurationSettings(),
				NotificationSettings = new NotificationSettings()
				{
					QuarterlyScoreAlert = new AlertConfiguration
					{
						Enabled = true,
						Recipients = "invalidemail"
					}
				}
			};

			//Act
			var result = this.pdbConfigurationService.ValidateConfiguration(config);

			//Assert
			Assert.IsFalse(result.Valid);
		}

		[Test]
		public void Validate_QuarterlyScoreStatus_RejectsInvalidEmail()
		{
			//Arrange
			var config = new PerformanceDashboardConfigurationSettings
			{
				BackupDbccSettings = new BackupDbccMonitoringConfigurationSettings(),
				NotificationSettings = new NotificationSettings()
				{
					QuarterlyScoreStatus = new AlertConfiguration
					{
						Enabled = true,
						Recipients = "invalidemail"
					}
				}
			};

			//Act
			var result = this.pdbConfigurationService.ValidateConfiguration(config);

			//Assert
			Assert.IsFalse(result.Valid);
		}

		[Test]
		public void Validate_BackupDBCCAlert_RejectsInvalidEmail()
		{
			//Arrange
			var config = new PerformanceDashboardConfigurationSettings
			{
				BackupDbccSettings = new BackupDbccMonitoringConfigurationSettings(),
				NotificationSettings = new NotificationSettings()
				{
					BackupDBCCAlert = new AlertConfiguration
					{
						Enabled = true,
						Recipients = "invalidemail"
					}
				}
			};

			//Act
			var result = this.pdbConfigurationService.ValidateConfiguration(config);

			//Assert
			Assert.IsFalse(result.Valid);
		}

		[Test]
		public void Validate_ConfigurationChangeAlert_RejectsInvalidEmail()
		{
			//Arrange
			var config = new PerformanceDashboardConfigurationSettings
			{
				BackupDbccSettings = new BackupDbccMonitoringConfigurationSettings(),
				NotificationSettings = new NotificationSettings()
				{
					ConfigurationChangeAlert = new AlertConfiguration
					{
						Enabled = true,
						Recipients = "invalidemail"
					}
				}
			};

			//Act
			var result = this.pdbConfigurationService.ValidateConfiguration(config);

			//Assert
			Assert.IsFalse(result.Valid);
		}

		[Test]
		public void Validate_AcceptsMultipleValidRecipients()
		{
			//Arrange
			var config = new PerformanceDashboardConfigurationSettings
			{
				BackupDbccSettings = new BackupDbccMonitoringConfigurationSettings(),
				NotificationSettings = new NotificationSettings()
				{
					WeeklyScoreAlert = new AlertConfiguration
					{
						Enabled = true,
						Recipients = "valid@email.com; alsovalid@email.com, anothervalid@email.com"
					}
				}
			};

			//Act
			var result = this.pdbConfigurationService.ValidateConfiguration(config);

			//Assert
			Assert.IsTrue(result.Valid);
		}

		[Test]
		public void Validate_RejectsInvalidRecipientInList()
		{
			//Arrange
			var config = new PerformanceDashboardConfigurationSettings
			{
				BackupDbccSettings = new BackupDbccMonitoringConfigurationSettings(),
				NotificationSettings = new NotificationSettings()
				{
					WeeklyScoreAlert = new AlertConfiguration
					{
						Enabled = true,
						Recipients = "valid@email.com; notvalid"
					}
				}
			};

			//Act
			var result = this.pdbConfigurationService.ValidateConfiguration(config);

			//Assert
			Assert.IsFalse(result.Valid);
		}

		[Test]
		public void Validate_AllowsEnablingViewBasedMonitoring_WhenActiveTargetsExist()
		{
			//Arrange
			sqlRepo.Setup(x => x.ListDbccTargets()).Returns(new List<DbccTargetInfo>()
			{
				new DbccTargetInfo { IsActive = true }
			});

			var config = new PerformanceDashboardConfigurationSettings
			{
				NotificationSettings = new NotificationSettings(),
				BackupDbccSettings = new BackupDbccMonitoringConfigurationSettings()
				{
					UseViewBasedMonitoring = true
				}
			};

			//Act
			var result = this.pdbConfigurationService.ValidateConfiguration(config);

			//Assert
			Assert.IsTrue(result.Valid);
		}

		[Test]
		public void Validate_PreventsEnablingViewBasedMonitoring_WhenNoActiveTargetsExist()
		{
			//Arrange
			sqlRepo.Setup(x => x.ListDbccTargets()).Returns(new List<DbccTargetInfo>());
			var config = new PerformanceDashboardConfigurationSettings
			{
				NotificationSettings = new NotificationSettings(),
				BackupDbccSettings = new BackupDbccMonitoringConfigurationSettings()
				{
					UseViewBasedMonitoring = true
				}
			};

			//Act
			var result = this.pdbConfigurationService.ValidateConfiguration(config);

			//Assert
			Assert.IsFalse(result.Valid);
		}

		[Test]
		public void Validate_PreventsEnablingDbccMonitoring_WhenAllMonitoringMethodsDisabled()
		{
			//Arrange
			sqlRepo.Setup(x => x.ListDbccTargets()).Returns(new List<DbccTargetInfo>()
			{
				new DbccTargetInfo { IsActive = true }
			});

			var config = new PerformanceDashboardConfigurationSettings
			{
				NotificationSettings = new NotificationSettings(),
				BackupDbccSettings = new BackupDbccMonitoringConfigurationSettings()
				{
					EnableDbccMonitoring = true,
					UseViewBasedMonitoring = false,
					UseCommandBasedMonitoring = false
				}
			};

			//Act
			var result = this.pdbConfigurationService.ValidateConfiguration(config);

			//Assert
			Assert.IsFalse(result.Valid);
		}

		[Test]
		public void GetConfiguration_Success()
		{
			//Arrange
			//String
			var stringConfigs = new[] { ConfigurationKeys.ConfigurationEditedBy, ConfigurationKeys.PartnerName, ConfigurationKeys.NotificationsRecipientsWeeklyScoreAlerts, ConfigurationKeys.NotificationsRecipientsSystemLoadForecast, ConfigurationKeys.NotificationsRecipientsUserExperienceForecast, ConfigurationKeys.NotificationsRecipientsQuarterlyScoreAlerts, ConfigurationKeys.NotificationsRecipientsQuarterlyScoreStatus, ConfigurationKeys.NotificationsRecipientsBackupDBCCAlerts, ConfigurationKeys.NotificationsRecipientsConfigurationChangeAlerts };

			//Boolean
			var boolConfigs = new[] { ConfigurationKeys.UseDbccViewMonitoring, ConfigurationKeys.UseDbccCommandMonitoring, ConfigurationKeys.ShowInvariantBackupDbccHistory, ConfigurationKeys.SendBackupDbccNotifications, ConfigurationKeys.SendWeeklyNotifications, ConfigurationKeys.SendDailyNotifications, ConfigurationKeys.SendHourlyNotifications, ConfigurationKeys.SendConfigurationChangeNotifications, ConfigurationKeys.SendSystemLoadForecast, ConfigurationKeys.SendUserExperienceForecast };

			//int
			//var intConfigs = new[] { ConfigurationKeys.NotificationsThresholdWeeklyScore, ConfigurationKeys.NotificationsThresholdQuarterlyScore, ConfigurationKeys.NotificationsThresholdSystemLoadScore, ConfigurationKeys.NotificationsThresholdUserExperienceScore };

			sqlRepo.Setup(r => r.ConfigurationRepository.ReadConfigurationValue(ConfigurationKeys.Section, It.IsAny<String>()))
				.Returns<String, String>((s1, config) =>
					stringConfigs.Any(c => c == config)
					? "str valu"
					: boolConfigs.Any(c => c == config)
					? "true"
					: "123");

			this.processControlRepository.Setup(r => r.ReadAll())
				.Returns(new List<ProcessControl>()
				{
					new ProcessControl { Id = ProcessControlId.RecoverabilityIntegrityAlerts, Frequency = 123 },
					new ProcessControl { Id = ProcessControlId.QuarterlyScoreAlerts, Frequency = 123 },
					new ProcessControl { Id = ProcessControlId.QuarterlyScoreStatus, Frequency = 123 },
					new ProcessControl { Id = ProcessControlId.InfrastructurePerformanceForecast, Frequency = 123 },
					new ProcessControl { Id = ProcessControlId.UserExperienceForecast, Frequency = 123 },
					new ProcessControl { Id = ProcessControlId.WeeklyScoreAlerts, Frequency = 123 }
				});

			//Act
			var result = this.pdbConfigurationService.GetConfiguration();

			//Assert
			Assert.That(result, Is.Not.Null);
		}

		[Test]
		public void GetConfiguration_Defaults()
		{
			//Arrange
			sqlRepo.Setup(r => r.ConfigurationRepository.ReadConfigurationValue(ConfigurationKeys.Section, It.IsAny<String>()))
				.Returns((string)null);

			this.processControlRepository.Setup(r => r.ReadAll()).Returns(new List<ProcessControl>());

			//Act
			var result = this.pdbConfigurationService.GetConfiguration();

			//Assert
			Assert.That(result, Is.Not.Null);
		}

		[Test]
		public void ListChanges_Sucess()
		{
			//Arrange
			var previous = new PerformanceDashboardConfigurationSettings
			{
				BackupDbccSettings = new BackupDbccMonitoringConfigurationSettings(),
				NotificationSettings = new NotificationSettings
				{
					WeeklyScoreAlert = new AlertConfiguration(),
					SystemLoadForecast = new AlertConfiguration(),
					UserExperienceForecast = new AlertConfiguration(),
					QuarterlyScoreAlert = new AlertConfiguration(),
					QuarterlyScoreStatus = new AlertConfiguration(),
					ConfigurationChangeAlert = new AlertConfiguration(),
					BackupDBCCAlert = new AlertConfiguration()
				}
			};
			var current = new PerformanceDashboardConfigurationSettings
			{
				BackupDbccSettings = new BackupDbccMonitoringConfigurationSettings(),
				NotificationSettings = new NotificationSettings
				{
					WeeklyScoreAlert = new AlertConfiguration(),
					SystemLoadForecast = new AlertConfiguration(),
					UserExperienceForecast = new AlertConfiguration(),
					QuarterlyScoreAlert = new AlertConfiguration(),
					QuarterlyScoreStatus = new AlertConfiguration(),
					ConfigurationChangeAlert = new AlertConfiguration(),
					BackupDBCCAlert = new AlertConfiguration()
				}
			};
			previous.BackupDbccSettings.UseCommandBasedMonitoring = false;
			current.BackupDbccSettings.UseCommandBasedMonitoring = true;
			previous.BackupDbccSettings.UseViewBasedMonitoring = false;
			current.BackupDbccSettings.UseViewBasedMonitoring = true;
			previous.BackupDbccSettings.ShowInvariantHistory = false;
			current.BackupDbccSettings.ShowInvariantHistory = true;
			previous.NotificationSettings.WeeklyScoreAlert.Recipients = "abc";
			current.NotificationSettings.WeeklyScoreAlert.Recipients = "abc2";
			previous.NotificationSettings.SystemLoadForecast.Recipients = "abc";
			current.NotificationSettings.SystemLoadForecast.Recipients = "abc2";
			previous.NotificationSettings.UserExperienceForecast.Recipients = "abc";
			current.NotificationSettings.UserExperienceForecast.Recipients = "abc2";
			previous.NotificationSettings.QuarterlyScoreAlert.Recipients = "abc";
			current.NotificationSettings.QuarterlyScoreAlert.Recipients = "abc2";
			previous.NotificationSettings.QuarterlyScoreStatus.Recipients = "abc";
			current.NotificationSettings.QuarterlyScoreStatus.Recipients = "abc2";
			previous.NotificationSettings.BackupDBCCAlert.Recipients = "abc";
			current.NotificationSettings.BackupDBCCAlert.Recipients = "abc2";
			previous.NotificationSettings.ConfigurationChangeAlert.Recipients = "abc";
			current.NotificationSettings.ConfigurationChangeAlert.Recipients = "abc2";
			previous.NotificationSettings.BackupDBCCAlert.Enabled = false;
			current.NotificationSettings.BackupDBCCAlert.Enabled = true;
			previous.NotificationSettings.WeeklyScoreAlert.Enabled = false;
			current.NotificationSettings.WeeklyScoreAlert.Enabled = true;
			previous.NotificationSettings.QuarterlyScoreAlert.Enabled = false;
			current.NotificationSettings.QuarterlyScoreAlert.Enabled = true;
			previous.NotificationSettings.QuarterlyScoreStatus.Enabled = false;
			current.NotificationSettings.QuarterlyScoreStatus.Enabled = true;
			previous.NotificationSettings.ConfigurationChangeAlert.Enabled = false;
			current.NotificationSettings.ConfigurationChangeAlert.Enabled = true;
			previous.NotificationSettings.SystemLoadForecast.Enabled = false;
			current.NotificationSettings.SystemLoadForecast.Enabled = true;
			previous.NotificationSettings.UserExperienceForecast.Enabled = false;
			current.NotificationSettings.UserExperienceForecast.Enabled = true;
			previous.NotificationSettings.BackupDBCCAlert.Frequency = 1;
			current.NotificationSettings.BackupDBCCAlert.Frequency = 2;
			previous.NotificationSettings.QuarterlyScoreAlert.Frequency = 1;
			current.NotificationSettings.QuarterlyScoreAlert.Frequency = 2;
			previous.NotificationSettings.QuarterlyScoreStatus.Frequency = 1;
			current.NotificationSettings.QuarterlyScoreStatus.Frequency = 2;
			previous.NotificationSettings.SystemLoadForecast.Frequency = 1;
			current.NotificationSettings.SystemLoadForecast.Frequency = 2;
			previous.NotificationSettings.UserExperienceForecast.Frequency = 1;
			current.NotificationSettings.UserExperienceForecast.Frequency = 2;
			previous.NotificationSettings.WeeklyScoreAlert.Frequency = 1;
			current.NotificationSettings.WeeklyScoreAlert.Frequency = 2;
			previous.NotificationSettings.WeeklyScoreAlert.Threshold = 1;
			current.NotificationSettings.WeeklyScoreAlert.Threshold = 2;
			previous.NotificationSettings.QuarterlyScoreAlert.Threshold = 1;
			current.NotificationSettings.QuarterlyScoreAlert.Threshold = 2;
			previous.NotificationSettings.SystemLoadForecast.Threshold = 1;
			current.NotificationSettings.SystemLoadForecast.Threshold = 2;
			previous.NotificationSettings.UserExperienceForecast.Threshold = 1;
			current.NotificationSettings.UserExperienceForecast.Threshold = 2;
			current.LastModifiedBy = "12345";

			//Act
			var result = this.pdbConfigurationService.ListChanges(previous, current);

			//Assert
			Assert.That(result.Count, Is.EqualTo(27));
			Assert.That(result.All(a => a.CreatedOn != default(DateTime)), Is.True);
			Assert.That(result.All(a => a.UserId == 12345), Is.True);
			Assert.That(result.All(a => a.OldValue == "abc" || a.OldValue == "1" || a.OldValue == "False"), Is.True);
			Assert.That(result.All(a => a.NewValue == "abc2" || a.NewValue == "2" || a.NewValue == "True"), Is.True);
		}
	}
}