namespace kCura.PDB.Data.Tests.Repositories
{
	using Core.Models;
	using kCura.PDB.Tests.Common;
	using System;
	using System.Linq;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Data.Repositories;
	using NUnit.Framework;

	[TestFixture]
	[Category("UnitPlatform")]
	[Category("Integration")] // For TeamCity to ignore this test
	public class ServerRepositoryTests
	{
		[OneTimeSetUp]
		public async Task Setup()
		{
			this.serverRepository = new ServerRepository(ConnectionFactorySetup.ConnectionFactory);
			server = await this.serverRepository.CreateAsync(new Server
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

			server.UptimeMonitoringResourceHost = UpdatedHost;
			await this.serverRepository.UpdateAsync(server);
		}

		[OneTimeTearDown]
		public async Task OneTimeTearDown()
		{
			await this.serverRepository.DeleteAsync(this.server);
		}

		private Server server;
		private static readonly string UpdatedHost = "example.com";
		private ServerRepository serverRepository;

		[Test]
		public async Task Server_ReadAll()
		{
			//Act
			var result = await serverRepository.ReadAllActiveAsync();

			//Assert
			Assert.That(result, Is.Not.Empty);
			Assert.That(result.Any(s => s.ServerId == server.ServerId), Is.True);
		}

		[Test]
		public void Server_CreateAsync()
		{
			//Assert
			Assert.That(server, Is.Not.Null);
			Assert.That(server.ServerId, Is.GreaterThan(0));
			// TODO Assert on the other columns that are expected to be created on insert
		}

		[Test]
		public async Task Server_ReadAsync()
		{
			//Act
			var result = await this.serverRepository.ReadAsync(server.ServerId);

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.ServerId, Is.EqualTo(server.ServerId));
		}

		[Test]
		public void Server_UpdateAsync()
		{
			//Assert
			Assert.That(server, Is.Not.Null);
			Assert.That(server.UptimeMonitoringResourceHost, Is.EqualTo(UpdatedHost));
		}

		[Test]
		public async Task Server_ReadServerPendingQosDeploymentAsync()
		{
			//Act
			var result = await this.serverRepository.ReadServerPendingQosDeploymentAsync();

			//Assert
			Assert.That(result, Is.Not.Null);
			// result may be empty if there are no pending servers needed qos deployed
		}

		[Test]
		public async Task Server_UpdateActiveServersPendingQosDeploymentAsync()
		{
			//Act
			await this.serverRepository.UpdateActiveServersPendingQosDeploymentAsync();
			var result = await this.serverRepository.ReadServerPendingQosDeploymentAsync();

			//Assert
			Assert.That(result, Is.Not.Empty);
		}

		[Test]
		public async Task Server_ZDeleteAsync()
		{
			//Arrange

			//Act
			await this.serverRepository.DeleteAsync(server);
			var result = await this.serverRepository.ReadAsync(server.ServerId);

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.DeletedOn, Is.Not.Null);
		}
	}

	[TestFixture]
	[Category("Integration")]
	public class ServerRepositoryIntegrationTests
	{
		[OneTimeSetUp]
		public async Task Setup()
		{
			connectionFactory = TestUtilities.GetIntegrationConnectionFactory();
			var repo = new ServerRepository(connectionFactory);
			server = await repo.CreateAsync(new Server
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

			server.UptimeMonitoringResourceHost = UpdatedHost;
			await repo.UpdateAsync(server);
		}


		private IConnectionFactory connectionFactory;
		private Server server;
		private static readonly string UpdatedHost = "example.com";

		[Test]
		public async Task Server_ReadAll()
		{
			//Arrange
			var repo = new ServerRepository(connectionFactory);

			//Act
			var result = await repo.ReadAllActiveAsync();

			//Assert
			Assert.That(result, Is.Not.Empty);
			Assert.That(result.Any(s => s.ServerId == server.ServerId), Is.True);
		}

		[Test]
		public void Server_CreateAsync()
		{
			//Assert
			Assert.That(server, Is.Not.Null);
			Assert.That(server.ServerId, Is.GreaterThan(0));
		}

		[Test]
		public async Task Server_ReadAsync()
		{
			//Arrange
			var repo = new ServerRepository(connectionFactory);

			//Act
			var result = await repo.ReadAsync(server.ServerId);

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.ServerId, Is.EqualTo(server.ServerId));
		}

		[Test]
		public void Server_UpdateAsync()
		{
			//Assert
			Assert.That(server, Is.Not.Null);
			Assert.That(server.UptimeMonitoringResourceHost, Is.EqualTo(UpdatedHost));
		}

		[Test]
		public async Task Server_ZDeleteAsync()
		{
			//Arrange
			var repo = new ServerRepository(connectionFactory);

			//Act
			await repo.DeleteAsync(server);
			var result = await repo.ReadAsync(server.ServerId);

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.DeletedOn, Is.Not.Null);
		}

		[Test]
		public async Task Server_ReadWorkspaceIds()
		{
			var repo = new ServerRepository(connectionFactory);
			var result = await repo.ReadServerWorkspaceIdsAsync(server.ServerId);
			
			Assert.Pass($"Workspaces on Server {server.ServerId}: Count={result.Count}, {string.Join(", ", result)}");
		}

		[Test]
		public async Task Server_ReadPrimaryStandaloneAsync()
		{
			// Arrange
			var repo = new ServerRepository(connectionFactory);

			// Act
			var result = await repo.ReadPrimaryStandaloneAsync();

			// Assert
			Assert.Pass($"Result may differ, current standalone server: {result}");
		}

		[Test]
		public async Task Server_ReadWorkspaceExistsAsync()
		{
			// Arrange
			var repo = new ServerRepository(connectionFactory);
			var workspaceId = Config.WorkSpaceId;

			// Act
			var result = await repo.ReadWorkspaceExistsAsync(workspaceId);

			// Assert
			Assert.Pass($"Result may differ, current existing status of workspaceId {workspaceId}: {result}");
		}
	}
}
