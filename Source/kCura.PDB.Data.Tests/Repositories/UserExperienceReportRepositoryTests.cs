namespace kCura.PDB.Data.Tests.Repositories
{
	using System;
	using System.Linq;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.Reports;
	using kCura.PDB.Data.Repositories;
	using kCura.PDB.Data.Services;
	using kCura.PDB.Service.Services;
	using NUnit.Framework;
	using PDB.Tests.Common;

	[TestFixture, Category("Integration")]
	public class UserExperienceReportRepositoryTests
	{
		[OneTimeSetUp]
		public async Task OneTimeSetup()
		{
			connectionFactory = TestUtilities.GetIntegrationConnectionFactory();
			userExperienceReportRepository = new UserExperienceReportRepository(connectionFactory);
			var hourRepo = new HourRepository(connectionFactory);
			hour = await hourRepo.CreateAsync(new Hour { HourTimeStamp = DateTime.Now });
			var serverRepo = new ServerRepository(connectionFactory);
			server = await serverRepo.CreateAsync(new Server
			{
				ServerName = Environment.MachineName,
				CreatedOn = DateTime.Now,
				DeletedOn = null,
				ServerTypeId = 3,
				ServerIpAddress = "127.0.0.1",
				IgnoreServer = false,
				ResponsibleAgent = "",
				ArtifactId = 1234,
				LastChecked = null,
				UptimeMonitoringResourceHost = null,
				UptimeMonitoringResourceUseHttps = null,
				LastServerBackup = null,
				AdminScriptsVersion = null,
			});
		}

		private Hour hour;
		private Server server;
		private IConnectionFactory connectionFactory;
		private IUserExperienceReportRepository userExperienceReportRepository;

		[Test]
		public async Task CreateUserExperienceWorkspaceRecord()
		{
			// Arrange
			var searchAudit = new UserExperienceWorkspace
			{
				HourId = hour.Id,
				WorkspaceId = 3333,
				SearchId = 4444,
				TotalExecutionTime = 5555,
				TotalSearchAudits = 6666,
				IsComplex = false
			};
			var searchAudits = Enumerable
				.Range(0, 5000)
				.Select(i => searchAudit)
				.ToList();

			// Act
			await this.userExperienceReportRepository.CreateUserExperienceWorkspaceRecord(searchAudits);

			// Assert
			Assert.Pass("No result returned");
		}

		[Test]
		public async Task CreateUserExperienceSearchRecord()
		{
			// Arrange
			var searchAudit = new UserExperienceSearch
			{
				HourId = hour.Id,
				WorkspaceId = 3333,
				SearchId = 4444,
				TotalExecutionTime = 5555,
				TotalSearchAudits = 6666,
				MinAuditId = 7777,
				IsComplex = false,
			};

			var searchAudits = Enumerable
				.Range(0, 5000)
				.Select(i => searchAudit)
				.ToList();

			// Act
			await this.userExperienceReportRepository.CreateUserExperienceSearchRecord(searchAudits);

			// Assert
			Assert.Pass("No result returned");
		}

		[Test]
		public async Task CreateServerAuditRecord()
		{
			// Arrange
			var searchAudit = new UserExperienceServer
			{
				HourId = hour.Id,
				ServerId = server.ServerId,
				WorkspaceId = 3333,
				TotalAudits = 9876543210000,
				TotalUsers = 123
			};

			// Act
			await this.userExperienceReportRepository.CreateServerAuditRecord(searchAudit);

			// Assert
			Assert.Pass("No result returned");
		}

		[Test]
		public async Task UpdateSearchAuditRecord()
		{
			// Act
			await this.userExperienceReportRepository.UpdateSearchAuditRecord(hour, server);

			// Assert
			Assert.Pass("No result returned");
		}

		[Test]
		public async Task CreateVarscatOutput()
		{
			// Act
			await this.userExperienceReportRepository.CreateVarscatOutput(hour, server);

			// Assert
			Assert.Pass("No result returned");
		}

		[Test]
		public async Task CreateVarscatOutputDetails()
		{
			// Act
			await this.userExperienceReportRepository.CreateVarscatOutputDetails(hour, server);

			// Assert
			Assert.Pass("No result returned");
		}

		[Test]
		public async Task ZDeleteTempReportData()
		{
			// Act
			await this.userExperienceReportRepository.DeleteTempReportData(hour, server);

			// Assert
			Assert.Pass("No result returned");
		}
	}
}
