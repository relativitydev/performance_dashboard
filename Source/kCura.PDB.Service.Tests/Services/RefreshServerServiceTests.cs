namespace kCura.PDB.Service.Tests.Services
{
	using System;
	using System.Collections.Generic;
	using System.Xml.Linq;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Service.Services;
	using kCura.PDB.Tests.Common;
	using Moq;
	using NUnit.Framework;

	[TestFixture, Category("Unit")]
	public class RefreshServerServiceTests
	{
		private RefreshServerService refreshServerService;
		private Mock<ILogger> loggerMock;
		private Mock<IResourceServerRepository> resourceServerRepository;

		[SetUp]
		public void SetUp()
		{
			this.loggerMock = TestUtilities.GetMockLogger();
			this.resourceServerRepository = new Mock<IResourceServerRepository>();
			this.refreshServerService = new RefreshServerService(this.loggerMock.Object, this.resourceServerRepository.Object);
		}

		/// <summary>
		/// Tests The consolidation of servers
		/// </summary>
		[Test]
		public void GetServerList_Success()
		{
			//arrange 
			var servers = new List<ResourceServer>()
			{
				new ResourceServer() { ArtifactID = 123, Name = "asdf1", ServerType = ServerType.WebBackground },
				new ResourceServer() { ArtifactID = 123, Name = "asdf1", ServerType = ServerType.WebApi },
				new ResourceServer() { ArtifactID = 123, Name = "asdf1", ServerType = ServerType.Web },
				new ResourceServer() { ArtifactID = 123, Name = "asdf4", ServerType = ServerType.Agent },
				new ResourceServer() { ArtifactID = 123, Name = "asdf5", ServerType = ServerType.Database },
				new ResourceServer() { ArtifactID = 123, Name = "asdf6", ServerType = ServerType.Invariant, Url = @"net.tcp://k12-Invariant-Server:6859/invariantapi" },
				new ResourceServer() { ArtifactID = 234, Name = "asdf7", ServerType = ServerType.Analytics, Url = @"http://k12-Analytics.milyli.net:8080/nexus/services"  },
				new ResourceServer() { ArtifactID = 345, Name = "asdf8", ServerType = ServerType.CacheLocation, Url = @"\\k12-Cache-Loc\repository\cache" }
			};
			this.resourceServerRepository.Setup(r => r.GetAllServers(It.IsAny<ILogger>())).Returns(servers);
			
			// Act
			var results = this.refreshServerService.GetServerList();

			// Assert
			Assert.That(results.Count, Is.EqualTo(6));
		}

		/// <summary>
		/// Tests The consolidation of servers
		/// </summary>
		[Test]
		public void UpdateServerList_List_Success()
		{
			//arrange 
			var servers = new List<ResourceServer>()
			{
				new ResourceServer() { ArtifactID = 123, Name = "asdf1", IP = "", ServerType = ServerType.WebBackground },
				new ResourceServer() { ArtifactID = 123, Name = "asdf1", IP = "", ServerType = ServerType.WebApi },
				new ResourceServer() { ArtifactID = 123, Name = "asdf1", IP = "", ServerType = ServerType.Web },
				new ResourceServer() { ArtifactID = 123, Name = "asdf4", IP = "", ServerType = ServerType.Agent },
				new ResourceServer() { ArtifactID = 123, Name = "CLUSTERED\\Instance", IP = "", ServerType = ServerType.Database },
				new ResourceServer() { ArtifactID = 123, Name = "asdf6", IP = "", ServerType = ServerType.Invariant, Url = @"net.tcp://k12-Invariant-Server:6859/invariantapi" },
				new ResourceServer() { ArtifactID = 234, Name = "asdf7", IP = "", ServerType = ServerType.Analytics, Url = @"http://k12-Analytics.milyli.net:8080/nexus/services"  },
				new ResourceServer() { ArtifactID = 345, Name = "asdf8", IP = "", ServerType = ServerType.CacheLocation, Url = @"\\k12-Cache-Loc\repository\cache" }
			};
			this.resourceServerRepository.Setup(r => r.MergeServerInformation(It.IsAny<XElement>()));
			this.loggerMock.Setup(l => l.LogError(It.IsAny<String>(), It.IsAny<String>()));

			//Act
			this.refreshServerService.UpdateServerList(servers);

			this.loggerMock.Verify(l => l.LogError(It.IsAny<String>(), It.IsAny<String>()), Times.Never);
			this.resourceServerRepository.VerifyAll();
			Assert.Pass();
		}

		/// <summary>
		/// Tests The consolidation of servers
		/// </summary>
		[Test,
			TestCase("asdf1", ServerType.WebBackground, "", "asdf1"),
			TestCase("asdf1", ServerType.WebApi, "", "asdf1"),
			TestCase("asdf1", ServerType.Web, "", "asdf1"),
			TestCase("asdf4", ServerType.Agent, "", "asdf4"),
			TestCase("asdf5", ServerType.Database, "", "asdf5"),
			TestCase("asdf6", ServerType.Invariant, @"net.tcp://k12-Invariant-Server:6859/invariantapi", "asdf6"),
			TestCase("asdf7", ServerType.Analytics, @"http://k12-Analytics.milyli.net:8080/nexus/services", "asdf7"),
			TestCase("asdf8", ServerType.CacheLocation, @"\\k12-Cache-Loc\repository\cache", "asdf8"),
			TestCase(@"CRLAREL-SQLCL01\INSTANCE01", ServerType.Database, null, @"CRLAREL-SQLCL01\INSTANCE01")]
		public void UpdateServerList_Success(string serverName, ServerType serverType, string url, string expectedServerName)
		{
			// Arrange 
			var servers = new List<ResourceServer>()
			{
				new ResourceServer() { ArtifactID = 123, Name = serverName, IP = "", ServerType = serverType, Url = url },
			};
			this.resourceServerRepository.Setup(r => r.MergeServerInformation(It.IsAny<XElement>()));
			this.loggerMock.Setup(l => l.LogError(It.IsAny<String>(), It.IsAny<String>()));
			this.loggerMock.Setup(l => l.LogVerbose(It.Is<string>(s => s.StartsWith($"UpdateServerList Called - Creating XML - ")), It.IsAny<string>()))
				.Callback<string, string>((m, c) => Console.WriteLine(m));

			// Act
			this.refreshServerService.UpdateServerList(servers);

			// Assert
			this.resourceServerRepository.Verify(r => r.MergeServerInformation(It.Is<XElement>(e => e.Element("Server").Attribute("Name").Value == expectedServerName)));
			Assert.Pass();
		}
	}
}
