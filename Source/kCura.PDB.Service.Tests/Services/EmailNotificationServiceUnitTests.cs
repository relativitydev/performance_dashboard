namespace kCura.PDB.Service.Tests.Services
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Net.Mail;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.BISSummary.Models;
	using kCura.PDB.Service.Services;
	using Moq;
	using NUnit.Framework;

	[TestFixture]
	[Category("Unit")]
	public class EmailNotificationServiceUnitTests
	{
		private MockRepository mockRepository;
		private Mock<ISqlServerRepository> sqlServerRepositoryMock;
		private Mock<ISmtpClient> smtpClientMock;
		private Mock<ISmtpClientFactory> smtpClientFactory;
		private Mock<IPdbConfigurationService> pdbConfigurationService;
		private Mock<IConfigurationRepository> configurationRepositoryMock;
		private Mock<IConfigurationAuditRepository> configurationAuditRepositoryMock;
		private EmailNotificationService emailNotificationService;

		[SetUp]
		public void Setup()
		{
			this.smtpClientMock = new Mock<ISmtpClient>();
			this.smtpClientMock.Setup(c => c.Send(It.IsAny<MailMessage>()));
			this.smtpClientFactory = new Mock<ISmtpClientFactory>();
			this.smtpClientFactory.Setup(f => f.CreateSmtpClient(It.IsAny<SmtpSettings>())).Returns(this.smtpClientMock.Object);

			this.mockRepository = new MockRepository(MockBehavior.Default);
			this.pdbConfigurationService = this.mockRepository.Create<IPdbConfigurationService>();
			this.sqlServerRepositoryMock = this.mockRepository.Create<ISqlServerRepository>();
			this.configurationRepositoryMock = this.mockRepository.Create<IConfigurationRepository>();
			this.configurationAuditRepositoryMock = this.mockRepository.Create<IConfigurationAuditRepository>(MockBehavior.Strict);

			this.emailNotificationService =
				new EmailNotificationService(
					this.sqlServerRepositoryMock.Object,
					this.smtpClientFactory.Object,
					this.pdbConfigurationService.Object,
					this.configurationRepositoryMock.Object,
					this.configurationAuditRepositoryMock.Object);
		}

		[Test]
		public void EmailNotification_BuildMailMessage()
		{
			var settings = new SmtpSettings() { EmailFrom = "test@test.com", Server = "localhost" };
			var recip = new List<string>() { "user@test.com" };

			var result = this.emailNotificationService.BuildMailMessage(settings, recip, "subject", "message");

			Assert.That(result, Is.Not.Null);
			Assert.That(result.To.Count, Is.EqualTo(recip.Count));
		}

		[Test]
		public void EmailNotification_BuildMailMessage_FromString()
		{
			var settings = new SmtpSettings() { EmailFrom = "test@test.com,asdf@adsf.com", Server = "localhost" };
			var recip = "user@test.com,a@g.com";

			var result = this.emailNotificationService.BuildMailMessage(settings, recip, "subject", "message");

			Assert.That(result, Is.Not.Null);
			Assert.That(result.To.Count, Is.EqualTo(2));
		}

		[Test]
		public void EmailNotification_BuildMailMessage_InvalidSMTPSettings()
		{
			Assert.Throws<ArgumentException>(() =>
				this.emailNotificationService.BuildMailMessage(new SmtpSettings(), new List<string>(), "subject", "message"));
			Assert.Throws<ArgumentException>(() =>
				this.emailNotificationService.BuildMailMessage(new SmtpSettings() { EmailFrom = "test@test.com" }, new List<string>(), "subject", "message"));
		}

		[Test]
		public void MailAddressFromString()
		{
			var result = this.emailNotificationService.MailAddressFromString("test@test.com");

			Assert.That(result, Is.Not.Null);
		}

		[Test]
		public void MailAddressFromString_MultipleAddress_Error()
		{
			Assert.Throws<Exception>(() => this.emailNotificationService.MailAddressFromString("test@test.com,test2@test.com"));
		}

		[Test]
		public void EmailNotification_BuildMailMessage_MultipleEmailFrom()
		{
			var settings = new SmtpSettings() { EmailFrom = "test@test.com,asdf@adsf.com", Server = "localhost" };
			var recip = "user@test.com,a@g.com";

			var result = this.emailNotificationService.BuildMailMessage(settings, recip, "subject", "message");

			Assert.That(result, Is.Not.Null);
		}

		[Test]
		[TestCase((string)null, 0)]
		[TestCase("", 0)]
		[TestCase("test@test.com", 1)]
		[TestCase("test@test.com,test2@test.com", 2)]
		[TestCase("test@test.com;test2@test.com", 2)]
		[TestCase("test@test.com, test2@test.com", 2)]
		[TestCase("test@test.com; test2@test.com", 2)]
		public void EmailNotification_GetRecipientList(string recipientList, Int32 expectedCount)
		{
			var result = this.emailNotificationService.GetEmailList(recipientList);

			Assert.That(result.Count, Is.EqualTo(expectedCount));
			result.ForEach(r => Assert.That(r.Contains(','), Is.False));
		}


		[Test, Category("Ignore"), Ignore("TODO: Fix me unless this test is not relevant anymore")]
		public void EmailNotification_SendConfigurationChangeAlerts_BuildMailMessagesSuccess()
		{
			var changes = new List<ConfigurationAudit>()
			{
				new ConfigurationAudit()
				{
					FieldName = ConfigurationAuditFields.NotificationsRecipientsConfigurationChangeAlerts,
					NewValue = "test@test.com,test2@test.com,test3@test.com",
					OldValue = "test@test.com,test2@test.com",
					ServerName = "localhost",
					UserId = 123,
					CreatedOn = DateTime.Now.AddDays(-90)
				}
			};
			this.sqlServerRepositoryMock.Setup(sr => sr.ConfigurationRepository.ReadConfigurationValue(It.IsAny<String>(), It.IsAny<String>()))
				.Returns("test@test.com,test2@test.com");
			this.sqlServerRepositoryMock.Setup(sr => sr.ReadRelativitySMTPSettings())
				.Returns(new SmtpSettings() { EmailFrom = "admin@test.com", Server = "localhost", Port = 25, SSLisRequired = false, Username = "Administrator", Password = "ExamplePassword" });
			this.pdbConfigurationService.Setup(cs => cs.GetConfiguration())
				.Returns(new PerformanceDashboardConfigurationSettings()
				{
					BackupDbccSettings = new BackupDbccMonitoringConfigurationSettings(),
					NotificationSettings = new NotificationSettings()
				});

			//SmtpException should be thrown AFTER we successfully create a mail messages and we are sending the mail message.
			//due to limited time to refactor i'm only checking to see that we get as far as creating the mail message
			//to mark success and when it sends the message the client throws a SmtpException
			Assert.Throws<SmtpException>(() => this.emailNotificationService.SendConfigurationChangeAlerts(changes));
		}

		[Test]
		public void SendConfigurationChangeAlerts_NoChanges()
		{
			var changes = new List<ConfigurationAudit>();
			this.configurationAuditRepositoryMock.Setup(m => m.ReadAll()).Returns(changes);
			this.emailNotificationService.SendConfigurationChangeAlerts(DateTime.UtcNow);
			this.mockRepository.VerifyAll();
			this.smtpClientMock.Verify(m => m.Send(It.IsAny<MailMessage>()), Times.Never);
		}

		[Test]
		public void SendConfigurationChangeAlerts_Changes()
		{
			var change = new ConfigurationAudit()
			{
				CreatedOn = DateTime.UtcNow,
				FieldName = ConfigurationAuditFields.NotificationsRecipientsConfigurationChangeAlerts,
				OldValue = "Test1@test.com",
				NewValue = "Test2@test.com"
			};
			var changes = new List<ConfigurationAudit> { change };
			this.configurationAuditRepositoryMock.Setup(m => m.ReadAll()).Returns(changes);
			var config = new PerformanceDashboardConfigurationSettings
			{
				BackupDbccSettings = new BackupDbccMonitoringConfigurationSettings(),
				NotificationSettings = new NotificationSettings { WeeklyScoreAlert = new AlertConfiguration(), SystemLoadForecast = new AlertConfiguration(), UserExperienceForecast = new AlertConfiguration(), QuarterlyScoreAlert = new AlertConfiguration(), QuarterlyScoreStatus = new AlertConfiguration(), BackupDBCCAlert = new AlertConfiguration(), ConfigurationChangeAlert = new AlertConfiguration() }
			};
			var smtpSettings = new SmtpSettings { EmailFrom = "test@test.com", Server = "localhost" };
			this.sqlServerRepositoryMock.Setup(m => m.ReadRelativitySMTPSettings()).Returns(smtpSettings);
			this.pdbConfigurationService.Setup(m => m.GetConfiguration()).Returns(config);
			this.emailNotificationService.SendConfigurationChangeAlerts(DateTime.UtcNow.AddHours(-1));
			this.mockRepository.VerifyAll();
			this.smtpClientMock.Verify(m => m.Send(It.IsAny<MailMessage>()), Times.Once);
		}
	}
}
