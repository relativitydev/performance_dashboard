namespace kCura.PDB.Service.Tests.Logic.Hours
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Core.Extensions;
	using Core.Interfaces.Repositories;
	using Core.Models;
	using Moq;
	using NUnit.Framework;
	using System.Threading.Tasks;
	using Core.Constants;
	using Core.Interfaces.Services;
	using kCura.PDB.Core.Interfaces.Servers;
	using kCura.PDB.Service.Hours;
	using kCura.PDB.Tests.Common;

	[TestFixture]
	[Category("Unit")]
	public class HourServiceTests
	{
		[SetUp]
		public void Setup()
		{
			this.hourRepository = new Mock<IHourRepository>();
			this.eventRepository = new Mock<IEventRepository>();
			this.configurationRepository = new Mock<IConfigurationRepository>();
			this.databaseService = new Mock<IDatabaseService>();
			this.logger = TestUtilities.GetMockLogger();
			this.hourService = new HourService(
				hourRepository.Object,
				eventRepository.Object,
				this.configurationRepository.Object,
				this.databaseService.Object,
				logger.Object);
		}

		private Mock<IHourRepository> hourRepository;
		private Mock<IEventRepository> eventRepository;
		private Mock<IConfigurationRepository> configurationRepository;
		private Mock<IDatabaseService> databaseService;
		private Mock<ILogger> logger;
		private HourService hourService;


		[Test]
		public async Task HourService_CreateHours()
		{
			// Arrange
			var amount = 24 * -Defaults.BackfillDays + 1;
			this.hourRepository.Setup(r => r.ReadBackFillHoursAsync())
				.ReturnsAsync(new List<Hour>());
			this.hourRepository.Setup(r => r.Create(It.IsAny<IList<Hour>>()))
				.Returns(Enumerable.Range(0, amount).Select(i => new Hour()).ToList());

			// Act
			var result = await this.hourService.CreateNextHours();

			// Assert
			this.hourRepository.Verify(r => r.Create(It.Is<IList<Hour>>(h => h.Count == amount)));
			this.hourRepository.Verify(r => r.Create(It.Is<IList<Hour>>(h => h.Any(hr => hr.HourTimeStamp == DateTime.UtcNow.NormilizeToHour()))),
				 Times.Once(), "must include current hour");
			Assert.That(result.Count, Is.EqualTo(amount));
		}

		[Test]
		public async Task HourService_CreateHours_WithLatestCompletedHour()
		{
			// Arrange
			var latestHourAgo = 3;
			var amount = 24 * -Defaults.BackfillDays + 1;
			this.hourRepository.Setup(r => r.ReadBackFillHoursAsync())
				.ReturnsAsync(new List<Hour>());
			this.hourRepository.Setup(r => r.Create(It.IsAny<IList<Hour>>()))
				.Returns(Enumerable.Range(0, latestHourAgo).Select(i => new Hour()).ToList());
			this.hourRepository.Setup(r => r.ReadLatestCompletedHourAsync())
				.ReturnsAsync(new Hour { HourTimeStamp = DateTime.UtcNow.AddHours(-latestHourAgo) });

			// Act
			var result = await this.hourService.CreateNextHours();

			// Assert
			Assert.That(result.Count, Is.EqualTo(latestHourAgo));
			this.hourRepository.Verify(r => r.Create(It.Is<IList<Hour>>(h => h.Count == latestHourAgo)));
			this.hourRepository.Verify(r => r.Create(It.Is<IList<Hour>>(h => h.Any(hr => hr.HourTimeStamp == DateTime.UtcNow.NormilizeToHour()))),
				 Times.Once(), "must include current hour");
		}

		[Test]
		public async Task HourService_CreateHours_HourService_CreateHours_WithPreviousHours()
		{
			//Arrange
			var amount = 24 * -Defaults.BackfillDays + 1 - 5;
			this.hourRepository.Setup(r => r.ReadBackFillHoursAsync())
				.ReturnsAsync(Enumerable.Range(0, 5).Select(h => new Hour { HourTimeStamp = DateTime.UtcNow.AddHours(-2 - h).NormilizeToHour() }).ToList());
			this.hourRepository.Setup(r => r.Create(It.IsAny<IList<Hour>>()))
				.Returns(Enumerable.Range(0, amount).Select(i => new Hour()).ToList());

			//Act
			var result = await this.hourService.CreateNextHours();

			//Assert
			this.hourRepository.Verify(r => r.Create(It.Is<IList<Hour>>(h => h.Count == amount)));
			this.hourRepository.Verify(r => r.Create(It.Is<IList<Hour>>(h => h.Any(hr => hr.HourTimeStamp == DateTime.UtcNow.NormilizeToHour()))),
					Times.Once(), "must include current hour");
			Assert.That(result.Count, Is.EqualTo(amount));
		}

		[Test]
		public async Task CreateHours_NoNewHours()
		{
			//Arrange
			var amount = 24 * -Defaults.BackfillDays + 1;
			var backFillFirstHour = DateTime.UtcNow.AddDays(Defaults.BackfillDays).NormilizeToHour();
			this.hourRepository.Setup(r => r.ReadBackFillHoursAsync())
				.ReturnsAsync(Enumerable.Range(0, amount).Select(i => new Hour { HourTimeStamp = backFillFirstHour.AddHours(i) }).ToList());
			this.hourRepository.Setup(r => r.Create(It.Is<IList<Hour>>(h => h.Count == 0)))
				.Returns(new List<Hour>());

			//Act
			var result = await this.hourService.CreateNextHours();

			//Assert
			this.hourRepository.Verify(r => r.Create(It.IsAny<IList<Hour>>()), Times.Never);
			Assert.That(result, Is.Empty);
		}


		[Test]
		public async Task StartHour_Success()
		{
			//Arrange
			int hourId = 1234;
			this.hourRepository.Setup(r => r.ReadAsync(hourId)).ReturnsAsync(new Hour { Id = hourId });
			this.hourRepository.Setup(r => r.UpdateAsync(It.IsAny<Hour>())).Returns(Task.Delay(1));
			this.configurationRepository.Setup(
				r => r.SetConfigurationValueEdds(ConfigurationKeys.Edds.SectionRelativityCore, ConfigurationKeys.Edds.AuditCountQueries, "True"))
				.Returns(Task.FromResult(1));
			this.configurationRepository.Setup(
				r => r.SetConfigurationValueEdds(ConfigurationKeys.Edds.SectionRelativityCore, ConfigurationKeys.Edds.AuditFullQueries, "True"))
				.Returns(Task.FromResult(1));
			this.configurationRepository.Setup(
				r => r.SetConfigurationValueEdds(ConfigurationKeys.Edds.SectionRelativityCore, ConfigurationKeys.Edds.AuditIdQueries, "True"))
				.Returns(Task.FromResult(1));
			this.databaseService.Setup(s => s.UpdateTrackedDatabasesAsync()).Returns(Task.FromResult(1));

			//Act
			await this.hourService.StartHour(hourId);

			//Assert
			this.hourRepository.Verify(r => r.UpdateAsync(It.Is<Hour>(h => h.StartedOn != null)));
			this.configurationRepository.Verify(
				r => r.SetConfigurationValueEdds(ConfigurationKeys.Edds.SectionRelativityCore, ConfigurationKeys.Edds.AuditCountQueries, "True"));
			this.configurationRepository.Verify(
				r => r.SetConfigurationValueEdds(ConfigurationKeys.Edds.SectionRelativityCore, ConfigurationKeys.Edds.AuditFullQueries, "True"));
			this.configurationRepository.Verify(
				r => r.SetConfigurationValueEdds(ConfigurationKeys.Edds.SectionRelativityCore, ConfigurationKeys.Edds.AuditIdQueries, "True"));
		}

		[Test]
		[TestCase(true, true, true)]
		[TestCase(true, true, false)]
		[TestCase(true, false, false)]
		[TestCase(false, true, true)]
		[TestCase(false, true, false)]
		[TestCase(false, false, true)]
		[TestCase(false, false, false)]
		public async Task SetAuditQueryConfigurations_Success(bool auditCountConfigValue, bool auditFullConfigValue, bool auditIdConfigValue)
		{
			var auditCountConfig = new[] { new RelativityConfigurationInfo { Value = auditCountConfigValue.ToString() } };
			var auditFullConfig = new[] { new RelativityConfigurationInfo { Value = auditFullConfigValue.ToString() } };
			var auditIdConfig = new[] { new RelativityConfigurationInfo { Value = auditIdConfigValue.ToString() } };

			this.configurationRepository
				.Setup(
					m => m.ReadEddsConfigurationInfoAsync(
						ConfigurationKeys.Edds.SectionRelativityCore,
						ConfigurationKeys.Edds.AuditCountQueries)).ReturnsAsync(auditCountConfig);
			this.configurationRepository
				.Setup(
					m => m.ReadEddsConfigurationInfoAsync(
						ConfigurationKeys.Edds.SectionRelativityCore,
						ConfigurationKeys.Edds.AuditFullQueries)).ReturnsAsync(auditFullConfig);
			this.configurationRepository
				.Setup(
					m => m.ReadEddsConfigurationInfoAsync(
						ConfigurationKeys.Edds.SectionRelativityCore,
						ConfigurationKeys.Edds.AuditIdQueries)).ReturnsAsync(auditIdConfig);
			// Act
			await this.hourService.SetAuditQueryConfigurations();

			// Assert
			if (!auditCountConfigValue)
			{
				this.configurationRepository.Verify(
					r => r.SetConfigurationValueEdds(
						ConfigurationKeys.Edds.SectionRelativityCore,
						ConfigurationKeys.Edds.AuditCountQueries,
						"True"));
			}

			if (!auditFullConfigValue)
			{
				this.configurationRepository.Verify(
					r => r.SetConfigurationValueEdds(
						ConfigurationKeys.Edds.SectionRelativityCore,
						ConfigurationKeys.Edds.AuditFullQueries,
						"True"));
			}

			if (!auditIdConfigValue)
			{
				this.configurationRepository.Verify(
					r => r.SetConfigurationValueEdds(
						ConfigurationKeys.Edds.SectionRelativityCore,
						ConfigurationKeys.Edds.AuditIdQueries,
						"True"));
			}
		}


		[Test]
		public async Task CompleteHour_Success()
		{
			//Arrange
			int hourId = 1234;
			this.hourRepository.Setup(r => r.ReadAsync(hourId)).ReturnsAsync(new Hour { Id = hourId });
			this.hourRepository.Setup(r => r.UpdateAsync(It.IsAny<Hour>())).Returns(Task.Delay(1));
			this.eventRepository.Setup(
				r => r.ReadAnyRemainingHourEventsAsync(hourId, EventSourceType.CompleteHour, EventStatus.Pending, EventStatus.InProgress, EventStatus.PendingHangfire))
				.ReturnsAsync(false);

			//Act
			await this.hourService.CompleteHour(hourId);

			//Assert
			this.hourRepository.Verify(r => r.UpdateAsync(It.Is<Hour>(h => h.CompletedOn != null)));
		}

	}
}
