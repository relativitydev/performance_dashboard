namespace kCura.PDB.Data.Tests.Repositories
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Linq;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Data.Repositories;
	using kCura.PDB.Tests.Common;
	using Moq;
	using NUnit.Framework;

	[TestFixture, Category("Unit")]
	public class ResourceServerRepositoryTestsUnit
	{
		[SetUp]
		public void Setup()
		{
			this.connectionFactory = new Mock<IConnectionFactory>(MockBehavior.Strict);
			this.resourceServerRepository = new ResourceServerRepository(this.connectionFactory.Object);
			this.logger = TestUtilities.GetMockLogger();
		}

		private ResourceServerRepository resourceServerRepository;
		private Mock<IConnectionFactory> connectionFactory;
		private Mock<ILogger> logger;

		[Test]
		public void GetResourceServersFromDataSet()
		{
			//Arrange
			var data = new DataSet();
			var table = data.Tables.Add("results");
			table.Columns.Add("ArtifactID", typeof(int));
			table.Columns.Add("ServerName", typeof(string));
			table.Columns.Add("ServerType", typeof(string));
			table.Columns.Add("URL", typeof(string));

			var row = table.Rows.Add();
			row["ArtifactID"] = 1;
			row["ServerName"] = "abc";
			row["ServerType"] = "abc";
			row["URL"] = "abc";
			var row2 = table.Rows.Add();
			row2["ArtifactID"] = 2;
			row2["ServerName"] = "xyz";
			row2["ServerType"] = "xyz";
			row2["URL"] = "xyz";

			//Act
			var result = resourceServerRepository.GetResourceServersFromDataSet(data);

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Count(), Is.EqualTo(2));
		}

		[Test]
		public void GetServerName_List()
		{
			//Arrange
			var servers = new List<ResourceServer>()
				{
					new ResourceServer() { ArtifactID = 123, Name = "asdf", ServerType = ServerType.Invariant, Url = @"net.tcp://k12-Invariant-Server:6859/invariantapi" },
					new ResourceServer() { ArtifactID = 234, Name = "asdf", ServerType = ServerType.Analytics, Url = @"http://k12-Analytics.milyli.net:8080/nexus/services"  },
					new ResourceServer() { ArtifactID = 345, Name = "asdf", ServerType = ServerType.CacheLocation, Url = @"\\k12-Cache-Loc\repository\cache" }
				};

			//Act
			this.resourceServerRepository.UpdateAllServerNames(servers);

			//Assert
			Assert.That(servers, Is.Not.Null);
			Assert.That(servers, Is.Not.Empty);

			Assert.That(servers[0].Name, Is.EqualTo("k12-Invariant-Server"));
			Assert.That(servers[1].Name, Is.EqualTo("k12-Analytics.milyli.net"));
			Assert.That(servers[2].Name, Is.EqualTo("k12-Cache-Loc"));

			this.connectionFactory.VerifyAll(); // Verify not called
		}

		[Test,
			TestCase("asdf", "net.tcp://k12-Invariant-Server:6859/invariantapi", ServerType.Invariant, "k12-Invariant-Server"),
			TestCase("asdf", @"http://k12-Analytics.milyli.net:8080/nexus/services", ServerType.Analytics, "k12-Analytics.milyli.net"),
			TestCase("asdf", @"\\k12-Cache-Loc\repository\cache", ServerType.CacheLocation, "k12-Cache-Loc"),
			TestCase(@"CRLAREL-SQLCL01\INSTANCE01", null, ServerType.Database, @"CRLAREL-SQLCL01\INSTANCE01"),]
		public void GetServerName(string serverName, string url, ServerType serverType, string expectedServerName)
		{
			//Arrange			
			var servers = new List<ResourceServer>()
				{
					new ResourceServer() { ArtifactID = 123, Name = serverName, ServerType = serverType, Url = url },
				};

			//Act
			this.resourceServerRepository.UpdateAllServerNames(servers);

			//Assert
			Assert.That(servers, Is.Not.Null);
			Assert.That(servers, Is.Not.Empty);
			Assert.That(servers.First().Name, Is.EqualTo(expectedServerName));

			this.connectionFactory.VerifyAll();
		}

		[Test,
			TestCase("Default Cache Location", "Cache Location Server", @"\\files\T001\Files\cache", "files"),
			TestCase("SQLHA-INVARIANT01,24332", "SQL - Distributed", "", "SQLHA-INVARIANT01,24332"),
			TestCase("VM-T001CAGT002", "Worker Manager Server", @"VM-T001CAGT002", "VM-T001CAGT002"),
			TestCase("Analytics", "Analytics Server", @"https://vm-t001caat001.t001.relativityone.local:8443/", "vm-t001caat001.t001.relativityone.local"),
			TestCase(@"CRLAREL-SQLCL01\INSTANCE01", "SQL - Distributed", "", @"CRLAREL-SQLCL01\INSTANCE01"),]
		public void GetServerName_Withvalues(string serverName, string serverTypeName, string url, string expectedValue)
		{
			//Arrange
			var servers = new List<ResourceServer>()
				{
					new ResourceServer() { ArtifactID = 123, Name = serverName, ServerType = this.resourceServerRepository.GetResourceServerType(serverTypeName), Url = url },
				};

			//Act
			this.resourceServerRepository.UpdateAllServerNames(servers);

			//Assert
			Assert.That(servers, Is.Not.Null);
			Assert.That(servers, Is.Not.Empty);

			Assert.That(servers[0].Name, Is.EqualTo(expectedValue));

			this.connectionFactory.VerifyAll();
		}

		[Test,
		TestCase("Agent", ServerType.Agent),
		TestCase("Services", ServerType.Services),
		TestCase("SQL - Primary", ServerType.Database),
		TestCase("SQL - Distributed", ServerType.Database),
		TestCase("Web", ServerType.Web),
		TestCase("Web:Forms Authentication", ServerType.Web),
		TestCase("Web:AD Authentication", ServerType.Web),
		TestCase("Web - Distributed", ServerType.Web),
		TestCase("Web - Distributed:Forms Authentication", ServerType.Web),
		TestCase("Web - Distributed:AD Authentication", ServerType.Web),
		TestCase("WebAPI", ServerType.WebApi),
		TestCase("WebAPI:Forms Authentication", ServerType.WebApi),
		TestCase("WebAPI:AD Authentication", ServerType.WebApi),
		TestCase("Web Background Processing", ServerType.WebBackground),
		TestCase("Processing Server", ServerType.Processing),
		TestCase("Analytics Server", ServerType.Analytics),
		TestCase("Worker Manager Server", ServerType.Invariant),
		TestCase("Worker", ServerType.InvariantWorker),
		TestCase("Cache Location Server", ServerType.CacheLocation),
		TestCase("abcxyz", ServerType.Unrecognized)]
		public void GetResourceServerType(String codeName, ServerType expectedResult)
		{
			//Arrange

			//Act
			var result = this.resourceServerRepository.GetResourceServerType(codeName);

			//Assert
			Assert.That(result, Is.EqualTo(expectedResult));
			this.connectionFactory.VerifyAll();
		}

		[Test,
			TestCase(@"CRLAREL-SQLCL01\INSTANCE01", ServerType.Database, @"CRLAREL-SQLCL01\INSTANCE01"),
			TestCase(@"abc,123", ServerType.Database, @"abc,123"),
			TestCase(@"abc", ServerType.Database, @"abc"),]
		public void GetServerNameForIpAddress(string serverName, ServerType serverType, string expectedServerName)
		{
			// Act
			var result = ResourceServerRepository.GetServerNameForIpAddress(new ResourceServer { Name = serverName });

			// Assert
			Assert.That(result, Is.EqualTo(expectedServerName));
		}

		[Test]
		public void GetHostAddress_Failure()
		{
			//Arrange
			var serverName = @"this server doesn't exist 0ba24e40-fde8-49c7-b6bb-d094d2ad667a";
			logger.Setup(l => l.LogVerbose(It.IsAny<string>(), It.IsAny<string>()));

			//Act
			var result = ResourceServerRepository.GetHostAddress(serverName, logger.Object);

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result, Is.Empty);
			logger.Verify(l => l.LogVerbose(It.IsAny<string>(), It.IsAny<string>()));
		}
	}
}
