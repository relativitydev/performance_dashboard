namespace kCura.PDB.Service.Tests.Services
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.DatabaseDeployment;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.ScriptInstallation;
	using kCura.PDB.Service.Installation;
	using kCura.PDB.Service.Services;
	using kCura.PDB.Tests.Common;
	using Moq;
	using NUnit.Framework;

	[TestFixture]
	[Category("Unit")]
	public class AdministrationInstallationServiceTests
	{
		[SetUp]
		public void Setup()
		{
			_databaseDeployer = new Mock<IDatabaseDeployer>();
			this.connectionFactoryMock = new Mock<IConnectionFactory>();
			_refreshServerService = new Mock<IRefreshServerService>();
			_serverRepositoryMock = new Mock<IServerRepository>();
			this.administrationInstallationRepositoryMock = new Mock<IAdministrationInstallationRepository>();
			this.administrationInstallationService = new AdministrationInstallationService(
				_databaseDeployer.Object,
				_serverRepositoryMock.Object,
				_refreshServerService.Object,
				this.administrationInstallationRepositoryMock.Object);
		}

		private Mock<IDatabaseDeployer> _databaseDeployer;
		private Mock<IConnectionFactory> connectionFactoryMock;
		private Mock<IRefreshServerService> _refreshServerService;
		private Mock<IServerRepository> _serverRepositoryMock;
		private Mock<IAdministrationInstallationRepository> administrationInstallationRepositoryMock;
		private string _integrityPath = @"C:\Program Files\kCura Corporation\Relativity\EDDS\CustomPages\60a1d0a3-2797-4fb3-a260-614cbfd3fa0d\bin";

		private AdministrationInstallationService administrationInstallationService;

		[Test]
		public void CredentialsAreValid_RejectsInvalidCredentials()
		{
			//Arrange
			var creds = new GenericCredentialInfo();
			var serverName = "localhost";
			this.administrationInstallationRepositoryMock.Setup(x => x.HasDbccPermissions(serverName, creds)).Throws(new Exception("Bad Credentials"));
			_serverRepositoryMock.Setup(x => x.ReadAllActive())
				.Returns(new[] { new Server { ServerName = serverName, ServerType = ServerType.Database } });

			//Act + Assert
			Assert.Throws<Exception>(() => this.administrationInstallationService.CredentialsAreValid(creds));
		}

		[Test]
		public void CredentialsAreValid_AcceptsValidCredentials()
		{
			//Arrange
			var creds = new GenericCredentialInfo();
			var serverName = "localhost";
			this.administrationInstallationRepositoryMock.Setup(x => x.HasDbccPermissions(serverName, creds)).Returns(true);
			_serverRepositoryMock.Setup(x => x.ReadAllActive())
				.Returns(new[] { new Server { ServerName = serverName, ServerType = ServerType.Database } });
			//Act
			var results = this.administrationInstallationService.CredentialsAreValid(creds);

			//Assert
			Assert.IsTrue(results);
		}

		[Test]
		public void ValidateCredentials_ChecksEachServer()
		{
			//Arrange
			var creds = new GenericCredentialInfo();
			var serverNames = new[] {"localhost", "localhost2", "localhost3"};
			var servers = serverNames.Select(sn => new Server {ServerName = sn, ServerType = ServerType.Database}).ToList();
			this.administrationInstallationRepositoryMock.Setup(x => x.HasDbccPermissions(It.IsIn(serverNames), creds)).Returns(true);
			_serverRepositoryMock.Setup(x => x.ReadAllActive())
				.Returns(servers);

			//Act
			this.administrationInstallationService.CredentialsAreValid(creds);

			//Assert
			this.administrationInstallationRepositoryMock.Verify(x => x.HasDbccPermissions(It.IsIn(serverNames), creds), Times.Exactly(3));
		}

		[Test]
		public void MockDeploy_OneServer_InstallsPerServerScripts()
		{
			//Arrange
			var creds = new GenericCredentialInfo();
			var servers = new[] { new ResourceServer() { Name = "localhost" } };
			var deploymentResults = new MigrationResultSet() { Success = true, Messages = new List<LogMessage>() { new LogMessage(LogSeverity.Info, "abc") } };
			_serverRepositoryMock.Setup(x => x.ReadAllActive())
				.Returns(servers.Select(s => new Server { ServerName = s.Name, ServerType = ServerType.Database }).ToArray());
			_databaseDeployer.Setup(ds => ds.DeployResource(It.IsAny<Server>(), creds)).Returns(deploymentResults);

			_refreshServerService.Setup(rss => rss.GetServerList()).Returns(servers);
			_refreshServerService.Setup(rss => rss.UpdateServerList(servers));


			//Act
			var results = this.administrationInstallationService.InstallScripts(creds);

			//Assert
			Assert.IsTrue(results.Success, results.Messages.Select(m => m.Text).Join("{0}\r\n{1}"));
			_databaseDeployer.Verify(ds => ds.DeployResource(It.IsAny<Server>(), creds), Times.Once);
		}

		[Test]
		public void MockDeploy_MultipleServers_InstallsPerServerScripts()
		{
			//Arrange
			var creds = new GenericCredentialInfo();
			var servers = new[] { new ResourceServer() { Name = "localhost" }, new ResourceServer() { Name = "second-server" } };
			var deploymentResults = new MigrationResultSet() { Success = true, Messages = new List<LogMessage>() { new LogMessage(LogSeverity.Info, "abc") } };
			_serverRepositoryMock.Setup(x => x.ReadAllActive())
				.Returns(servers.Select(s => new Server { ServerName = s.Name, ServerType = ServerType.Database }).ToArray());
			_databaseDeployer.Setup(ds => ds.DeployResource(It.IsAny<Server>(), creds)).Returns(deploymentResults);
			_refreshServerService.Setup(rss => rss.GetServerList()).Returns(servers);
			_refreshServerService.Setup(rss => rss.UpdateServerList(servers));
			
			//Act
			var results = this.administrationInstallationService.InstallScripts(creds);

			//Assert
			Assert.IsTrue(results.Success, results.Messages.Select(m => m.Text).Join("{0}\r\n{1}"));
			_databaseDeployer.Verify(ds => ds.DeployResource(It.IsAny<Server>(), creds), Times.Exactly(2));
		}

		[Test]
		public void MockDeploy_UpdatesScriptsInstalledVersion()
		{
			//Arrange
			var creds = new GenericCredentialInfo();
			var servers = new[] { new ResourceServer() { Name = "localhost" } };
			var deploymentResults = new MigrationResultSet() { Success = true, Messages = new List<LogMessage>() { new LogMessage(LogSeverity.Info, "abc") } };
			_serverRepositoryMock.Setup(x => x.ReadAllActive())
				.Returns(servers.Select(s => new Server() { ServerName = s.Name, ServerType = ServerType.Database }).ToArray());
			_databaseDeployer.Setup(ds => ds.DeployResource(It.IsAny<Server>(), creds)).Returns(deploymentResults);
			_refreshServerService.Setup(rss => rss.GetServerList()).Returns(servers);
			_refreshServerService.Setup(rss => rss.UpdateServerList(servers));
			this.administrationInstallationRepositoryMock.Setup(x => x.UpdateAdminScriptsRun());
			
			//Act
			var results = this.administrationInstallationService.InstallScripts(creds);

			//Assert
			Assert.IsTrue(results.Success);
			this.administrationInstallationRepositoryMock.Verify(x => x.UpdateAdminScriptsRun(), Times.Exactly(1));
		}

		[Test]
		public void MockDeploy_PerServerScriptFailure_IndicatesDeploymentFailure()
		{
			//Arrange
			var creds = new GenericCredentialInfo();
			_serverRepositoryMock.Setup(x => x.ReadAllActive())
				.Returns(new[] 
				{
					new Server { ServerName = "localhost", ServerType = ServerType.Database },
					new Server { ServerName = "secret-base", ServerType = ServerType.Database }
				});
			

			//Act
			var results = this.administrationInstallationService.InstallScripts(creds);

			//Assert
			Assert.IsFalse(results.Success);
		}

		[Test]
		public void HandleDeploymentResponse_Success()
		{
			//Arrange
			var installResults = new ScriptInstallationResults();
			var deploymentResults = new MigrationResultSet() { Success = true, Messages = new List<LogMessage>() { new LogMessage(LogSeverity.Info, "abc") } };

			//Act
			AdministrationInstallationService.HandleDeploymentResponse(deploymentResults, installResults);

			//Assert
			Assert.That(installResults.Messages, Is.Not.Empty);
		}

	}
}
