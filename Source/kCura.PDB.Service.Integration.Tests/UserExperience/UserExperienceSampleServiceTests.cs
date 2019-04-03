namespace kCura.PDB.Service.Integration.Tests.UserExperience
{
	using System.Linq;
	using System.Threading.Tasks;
	using kCura.AuditUI2.Services.ExternalAuditLog;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.Audits;
	using kCura.PDB.Data.Repositories;
	using kCura.PDB.Service.CategoryScoring;
	using kCura.PDB.Service.DataGrid.Interfaces;
	using kCura.PDB.Tests.Common;
	using Moq;
	using Ninject;
	using NUnit.Framework;

	[TestFixture]
	[Category("Integration")]
	public class UserExperienceSampleServiceTests
	{
		private UserExperienceSampleService userExperienceSampleService;

		[SetUp]
		public void Setup()
		{
			var kernel = IntegrationSetupFixture.CreateNewKernel(true);
			var helper = TestUtilities.GetMockHelper(Config.RelativityServiceUrl, Config.RelativityRestUrl, Config.RSAPIUsername, Config.RSAPIPassword);
			kernel.Rebind<global::Relativity.API.IHelper>().ToConstant(helper.Object);
			//var mockToggleProvider = new Mock<IToggleProvider>

			var mockFactory = new Mock<IAuditLogObjectManagerFactory>();
			mockFactory.Setup(m => m.GetManager()).Returns(() => new ApiClientHelper().GetKeplerServiceReference<IExternalAuditLogObjectManager>().Value);
			kernel.Rebind<IAuditLogObjectManagerFactory>().ToConstant(mockFactory.Object);

			this.userExperienceSampleService = kernel.Get<UserExperienceSampleService>();
		}

		[Test]
		public async Task GetPastWeekUserExperienceMetricDataAsync()
		{
			var connectionFactory = TestUtilities.GetIntegrationConnectionFactory();
			var serverRepository = new ServerRepository(connectionFactory);
			var servers = await serverRepository.ReadAllActiveAsync();
			var server = servers.First(s => s.ServerType == ServerType.Database);
			var hourRepository = new HourRepository(connectionFactory);
			var hour = await hourRepository.ReadNextHourWithoutRatings();

			var result = await this.userExperienceSampleService.GetPastWeekUserExperienceMetricDataAsync(server.ServerId, hour);

			Assert.Pass();
		}

		[Test]
		public async Task CalculateSample()
		{
			var connectionFactory = TestUtilities.GetIntegrationConnectionFactory();
			var serverRepository = new ServerRepository(connectionFactory);
			var servers = await serverRepository.ReadAllActiveAsync();
			var server = servers.First(s => s.ServerType == ServerType.Database);
			var hourRepository = new HourRepository(connectionFactory);
			var hour = await hourRepository.ReadNextHourWithoutRatings() ?? await hourRepository.ReadLatestCompletedHourAsync();

			var result = await this.userExperienceSampleService.CalculateSample(server.ServerId, hour.Id);

			Assert.Pass();
		}

		[Test]
		public async Task UpdateSample()
		{
			var sample = new PastWeekEligibleSample();

			await this.userExperienceSampleService.UpdateCurrentSample(sample);

			Assert.Pass();
		}
	}
}
