namespace kCura.PDB.Data.Tests.Testing
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;

	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.Testing;
	using kCura.PDB.Data.Repositories;
	using kCura.PDB.Data.Repositories.Testing;
	using kCura.PDB.Data.Testing;
	using kCura.PDB.Data.Tests.Repositories;

	using NUnit.Framework;

	[TestFixture]
	[Category("UnitPlatform")]
	[Category("Integration")]
	public class IntegrationTestNewServerRepositoryTests
	{
		private IntegrationTestNewServerRepository repository;
		private ServerRepository serverRepository;
		private ServerTestDataRepository serverTestDataRepository;

		private MockServer testServer;
		private Server nonTestServer;


		[OneTimeSetUp]
		public async Task Setup()
		{
			this.repository = new IntegrationTestNewServerRepository(ConnectionFactorySetup.ConnectionFactory);
			this.serverRepository = new ServerRepository(ConnectionFactorySetup.ConnectionFactory);
			this.serverTestDataRepository = new ServerTestDataRepository(ConnectionFactorySetup.ConnectionFactory);

			this.testServer = new MockServer
				                  {
					                  ArtifactID = 1234,
					                  CreatedOn = DateTime.UtcNow.NormilizeToHour(),
					                  ServerName = "TestServerName",
					                  ServerTypeID = 3
				                  };
			await this.serverTestDataRepository.CreateAsync(new List<MockServer> { this.testServer });
			var badServer = new Server { CreatedOn = DateTime.UtcNow, ServerName = "BadServer" };
			this.nonTestServer = await this.serverRepository.CreateAsync(badServer);
		}

		[OneTimeTearDown]
		public async Task TearDown()
		{
			await this.serverTestDataRepository.ClearAsync();
		}

		[Test]
		public async Task ReadAllActiveAsync()
		{
			// Act
			var results = await this.repository.ReadAllActiveAsync();

			// Assert
			Assert.That(results.Count, Is.EqualTo(1));
			Assert.That(results, Does.Not.Contains(this.nonTestServer));
			var result = results.First();
			Assert.That(result.ArtifactId, Is.EqualTo(this.testServer.ArtifactID));
			Assert.That(result.CreatedOn, Is.EqualTo(this.testServer.CreatedOn));
			Assert.That(result.ServerName, Is.EqualTo(this.testServer.ServerName));
			Assert.That(result.ServerTypeId, Is.EqualTo(this.testServer.ServerTypeID));
		}

		[Test]
		public async Task ReadAsync()
		{
			// Arrange
			var results = await this.repository.ReadAllActiveAsync();
			var firstServer = results.First();
			
			// Act
			var result = await this.repository.ReadAsync(firstServer.ServerId);

			// Assert
			Assert.That(result.ServerId, Is.EqualTo(firstServer.ServerId));
			Assert.That(result.ArtifactId, Is.EqualTo(firstServer.ArtifactId));
			Assert.That(result.CreatedOn, Is.EqualTo(firstServer.CreatedOn));
			Assert.That(result.ServerName, Is.EqualTo(firstServer.ServerName));
			Assert.That(result.ServerTypeId, Is.EqualTo(firstServer.ServerTypeId));
			Assert.That(result.ServerIpAddress, Is.EqualTo(firstServer.ServerIpAddress));
			Assert.That(result.ServerType, Is.EqualTo(firstServer.ServerType));
			Assert.That(result.LastServerBackup, Is.EqualTo(firstServer.LastServerBackup));
			Assert.That(result.IgnoreServer, Is.EqualTo(firstServer.IgnoreServer));
			Assert.That(result.AdminScriptsVersion, Is.EqualTo(firstServer.AdminScriptsVersion));
			Assert.That(result.IsQoSDeployed, Is.EqualTo(firstServer.IsQoSDeployed));
		}
	}
}
