namespace kCura.PDB.Service.Tests.ProcessControls
{
	using System;
	using System.Collections.Generic;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Service.ProcessControls.ProcessTasks;
	using Moq;
	using NUnit.Framework;

	[TestFixture]
	public class RunServerDataRefreshTaskTests
	{
		/// <summary>
		/// Tests The consolidation of servers
		/// </summary>
		[Test, Category("Unit")]
		public void Execute_Success()
		{
			//arrange 
			var resourceServers = new List<ResourceServer>()
			{
				new ResourceServer() { ArtifactID = 123, Name = "Test_asdf1", ServerType = ServerType.WebBackground },
				new ResourceServer() { ArtifactID = 123, Name = "Test_asdf1", ServerType = ServerType.WebApi },
				new ResourceServer() { ArtifactID = 123, Name = "Test_asdf1", ServerType = ServerType.Web },
				new ResourceServer() { ArtifactID = 123, Name = "Test_asdf4", ServerType = ServerType.Agent },
				new ResourceServer() { ArtifactID = 123, Name = "Test_asdf5", ServerType = ServerType.Database },
				new ResourceServer() { ArtifactID = 123, Name = "Test_asdf6", ServerType = ServerType.Invariant , Url = @"net.tcp://k12-Invariant-Server:6859/invariantapi" },
				new ResourceServer() { ArtifactID = 234, Name = "Test_asdf7", ServerType = ServerType.Analytics , Url = @"http://k12-Analytics.milyli.net:8080/nexus/services"  },
				new ResourceServer() { ArtifactID = 345, Name = "Test_asdf8", ServerType = ServerType.CacheLocation, Url = @"\\k12-Cache-Loc\repository\cache" },
				new ResourceServer() { ArtifactID = 123, Name = "Test_asdf9", ServerType = ServerType.Database },
			};
			var servers = new List<Server>()
			{
				new Server() { ArtifactId = 123, ServerName = "Test_asdf1", ServerType = ServerType.WebBackground },
				new Server() { ArtifactId = 123, ServerName = "Test_asdf1", ServerType = ServerType.WebApi },
				new Server() { ArtifactId = 123, ServerName = "Test_asdf1", ServerType = ServerType.Web },
				new Server() { ArtifactId = 123, ServerName = "Test_asdf4", ServerType = ServerType.Agent },
				new Server() { ArtifactId = 123, ServerName = "Test_asdf5", ServerType = ServerType.Database },
				new Server() { ArtifactId = 123, ServerName = "Test_asdf6", ServerType = ServerType.Invariant }, //, Url = @"net.tcp://k12-Invariant-Server:6859/invariantapi" },
				new Server() { ArtifactId = 234, ServerName = "Test_asdf7", ServerType = ServerType.Analytics }, //, Url = @"http://k12-Analytics.milyli.net:8080/nexus/services"  },
				new Server() { ArtifactId = 345, ServerName = "Test_asdf8", ServerType = ServerType.CacheLocation}, //, Url = @"\\k12-Cache-Loc\repository\cache" },
				new Server() { ArtifactId = 123, ServerName = "Test_asdf9", ServerType = ServerType.Database },
			};
			var logger = new Mock<ILogger>();
			var repo = new Mock<ISqlServerRepository>();
			var refreshSvc = new Mock<IRefreshServerService>();
			repo.Setup(r => r.PerformanceServerRepository.ReadAllActive()).Returns(servers);
			refreshSvc.Setup(rs => rs.GetServerList()).Returns(resourceServers);
			refreshSvc.Setup(rs => rs.UpdateServerList(resourceServers));

			//Act
			var task = new RunServerDataRefreshTask(logger.Object, repo.Object, 0, refreshSvc.Object);
			var results = task.Execute(null);

			Assert.That(results, Is.EqualTo(true));
			refreshSvc.Verify(rs => rs.UpdateServerList(resourceServers));
		}

		[Test, Category("Unit")]
		public void Execute_NoServers()
		{
			//arrange 
			var servers = new List<ResourceServer>();
			var logger = new Mock<ILogger>();
			var repo = new Mock<ISqlServerRepository>();
			var refreshSvc = new Mock<IRefreshServerService>();
			refreshSvc.Setup(rs => rs.GetServerList()).Returns(servers);
			logger.Setup(l => l.LogVerbose(It.Is<String>(s => s.Contains("RunServerTask Called - CurrentServers is null!")), It.IsAny<String>()));

			//Act
			var task = new RunServerDataRefreshTask(logger.Object, repo.Object, 0, refreshSvc.Object);
			var results = task.Execute(null);

			Assert.That(results, Is.EqualTo(true));
			refreshSvc.Verify(rs => rs.UpdateServerList(It.IsAny<List<ResourceServer>>()), Times.Never);
		}
	}
}
