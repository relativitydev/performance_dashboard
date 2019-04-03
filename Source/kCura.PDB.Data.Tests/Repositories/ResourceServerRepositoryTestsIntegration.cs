namespace kCura.PDB.Data.Tests.Repositories
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Xml.Linq;
	using Dapper;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Data.Repositories;
	using kCura.PDB.Tests.Common;
	using Moq;
	using NUnit.Framework;

	[TestFixture, Category("Integration")]
	public class ResourceServerRepositoryTestsIntegration
	{
		[SetUp]
		public void Setup()
		{
			logger = new Mock<ILogger>();
			this.connectionFactory = TestUtilities.GetIntegrationConnectionFactory();
			this.resourceServerRepository = new ResourceServerRepository(this.connectionFactory);
		}

		private Mock<ILogger> logger;
		private IConnectionFactory connectionFactory;
		private ResourceServerRepository resourceServerRepository;

		[Test]
		public void ReadResourceServers()
		{
			//Arrange

			//Act
			var result = this.resourceServerRepository.ReadResourceServers();

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result, Is.Not.Empty);
		}

		[Test]
		public void ReadFileServers()
		{
			//Arrange

			//Act
			var result = this.resourceServerRepository.ReadFileServers();

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result, Is.Not.Empty);
		}

		[Test]
		public void GetAllServers()
		{
			//Arrange

			//Act
			var result = this.resourceServerRepository.GetAllServers(logger.Object);

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result, Is.Not.Empty);
			Assert.That(result.All(r => String.IsNullOrEmpty(r.IP) == false), Is.True);
		}

		[Test]
		public void GetAllServersWithIps_List()
		{
			//Arrange
			var servers = new List<ResourceServer>()
				{
					new ResourceServer() { ArtifactID = 123, Name = Config.Server, ServerType = ServerType.Invariant },
					new ResourceServer() { ArtifactID = 234, Name = Config.Server, ServerType = ServerType.Database },
					new ResourceServer() { ArtifactID = 345, Name = $"{Config.Server},1433", ServerType = ServerType.Database },
				};

			//Act
			var result = this.resourceServerRepository.GetAllServersWithIps(servers, logger.Object);

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Count, Is.EqualTo(servers.Count));
			Assert.That(result.All(r => string.IsNullOrEmpty(r.IP) == false), Is.True);
		}

		[Test]
		public void GetAllServersWithIps(string serverName)
		{
			//Arrange
			var servers = new List<ResourceServer>()
				{
					new ResourceServer() { ArtifactID = 123, Name = Config.Server, ServerType = ServerType.Invariant },
					new ResourceServer() { ArtifactID = 234, Name = Config.Server, ServerType = ServerType.Database },
					new ResourceServer() { ArtifactID = 345, Name = $"{Config.Server},1433", ServerType = ServerType.Database },
				};

			//Act
			var result = this.resourceServerRepository.GetAllServersWithIps(servers, logger.Object);

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Count, Is.EqualTo(servers.Count));
			Assert.That(result.All(r => string.IsNullOrEmpty(r.IP) == false), Is.True);
		}

		[Test]
		public void MergeServerInformation()
		{
			//Arrange
			var servers = new List<ResourceServer>()
				{
					new ResourceServer() { ArtifactID = 123, Name = "Test_asdf1", IP = "", ServerType = ServerType.WebBackground },
					new ResourceServer() { ArtifactID = 123, Name = "Test_asdf1", IP = "", ServerType = ServerType.WebApi },
					new ResourceServer() { ArtifactID = 123, Name = "Test_asdf1", IP = "", ServerType = ServerType.Web },
					new ResourceServer() { ArtifactID = 123, Name = "Test_asdf4", IP = "", ServerType = ServerType.Agent },
					new ResourceServer() { ArtifactID = 123, Name = "Test_asdf5", IP = "", ServerType = ServerType.Database },
					new ResourceServer() { ArtifactID = 123, Name = "Test_asdf6", IP = "", ServerType = ServerType.Invariant, Url = @"net.tcp://k12-Invariant-Server:6859/invariantapi" },
					new ResourceServer() { ArtifactID = 234, Name = "Test_asdf7", IP = "", ServerType = ServerType.Analytics, Url = @"http://k12-Analytics.milyli.net:8080/nexus/services"  },
					new ResourceServer() { ArtifactID = 345, Name = "Test_asdf8", IP = "", ServerType = ServerType.CacheLocation, Url = @"\\k12-Cache-Loc\repository\cache" }
				};

			var xml = new XElement("ServerList",
								 from server in servers
								 where server.IP.Length <= 15
								 select new XElement("Server",
									 new XAttribute("Name", string.IsNullOrEmpty(server.ServerInstance)
										 ? server.Name
										 : string.Format("{0}\\{1}", server.Name, server.ServerInstance)),
									 new XAttribute("IP", server.IP),
									 new XAttribute("TypeID", (int)server.ServerType),
									 new XAttribute("ArtifactID", server.ArtifactID)
									 ));

			//Act
			this.resourceServerRepository.MergeServerInformation(xml);

			//cleanup
			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{
				conn.Execute("delete from eddsperformance.eddsdbo.[Server] where ServerName like 'Test_asdf%'");
			}

			//Assert
			Assert.Pass();
		}

		[TearDown]
		public void TearDown()
		{
			// Cleanup
			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{
				conn.Execute("delete from eddsperformance.eddsdbo.[Server] where ServerName like 'Test_asdf%'");
			}
		}
	}
}
