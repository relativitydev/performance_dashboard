namespace kCura.PDB.Service.Tests.Services
{
	using System;
	using System.Net.Mail;
	using kCura.PDB.Core.Interfaces;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Data.Repositories;
	using kCura.PDB.Data.Tests.Repositories;
	using kCura.PDB.Service.Services;
	using Moq;
	using NUnit.Framework;

	[TestFixture]
	[Category("UnitPlatform")]
	[Category("Integration")]
	public class EmailNotificationServiceTests
	{
		private Mock<ISmtpClientFactory> smtpClientFactory;
		private Mock<IPdbConfigurationService> pdbConfigurationService;
		private ConfigurationRepository configurationRepository;
		private ConfigurationAuditRepository configurationAuditRepository;
		private SqlServerRepository sqlServerRepository;
		private EmailNotificationService emailNotificationService;

		[SetUp]
		public void Setup()
		{
			var smtpClient = new Mock<ISmtpClient>();
			smtpClient.Setup(c => c.Send(It.IsAny<MailMessage>()));
			smtpClientFactory = new Mock<ISmtpClientFactory>();
			smtpClientFactory.Setup(f => f.CreateSmtpClient(It.IsAny<SmtpSettings>())).Returns(smtpClient.Object);

			var connectionFactory = ConnectionFactorySetup.ConnectionFactory;
			this.configurationRepository = new ConfigurationRepository(connectionFactory);
			this.configurationAuditRepository = new ConfigurationAuditRepository(connectionFactory);
			this.pdbConfigurationService = new Mock<IPdbConfigurationService>();
			this.sqlServerRepository = new SqlServerRepository(connectionFactory);
			this.emailNotificationService =
				new EmailNotificationService(this.sqlServerRepository, this.smtpClientFactory.Object, this.pdbConfigurationService.Object, this.configurationRepository, this.configurationAuditRepository);
		}

		[Test]
		public void Integration_SendDaily()
		{
			this.emailNotificationService.SendQuarterlyScoreAlerts();
		}

		[Test]
		public void Integration_SendWeekly()
		{
			this.emailNotificationService.SendQuarterlyScoreStatus();
		}

		[Test]
		public void Integration_SendBackupDBCCAlert()
		{
			this.emailNotificationService.SendRecoverabilityIntegrityAlerts();
		}

		[Test]
		public void Integration_SendReconfigurationNotice()
		{
			// Arrange
			var auditStart =
				DateTime.Parse(
					"2015-03-04 06:39:10.100"); //Change this to restrict the portion of the audit trail that's included in the email

			// Act
			this.emailNotificationService.SendConfigurationChangeAlerts(auditStart);
		}

		[Test]
		public void Integration_SendHourlyScoreAlert()
		{
			this.emailNotificationService.SendWeeklyScoreAlerts();
		}

		[Test]
		public void Integration_SendSystemLoadForecast()
		{
			this.emailNotificationService.SendInfrastructurePerformanceForecast();
		}

		[Test]
		public void Integration_SendUserExperienceForecast()
		{
			this.emailNotificationService.SendUserExperienceForecast();
		}
	}
}