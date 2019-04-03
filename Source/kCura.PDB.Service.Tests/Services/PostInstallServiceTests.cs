namespace kCura.PDB.Service.Tests.Services
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Interfaces.Agent;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Service.Installation;
	using kCura.PDB.Tests.Common;
	using Moq;
	using NUnit.Framework;

	[TestFixture]
	[Category("Unit")]
	public class PostInstallServiceTests
	{
		private Mock<IAgentManagerService> agentManagerServiceMock;
		private Mock<IRefreshServerService> refreshServerServiceMock;
		private Mock<IServerRepository> serverRepositoryMock;
		private Mock<IConfigurationRepository> configurationRepositoryMock;
		private Mock<ILogger> loggerMock;
		private PostInstallService postInstallService;

		[SetUp]
		public void Setup()
		{
			this.agentManagerServiceMock = new Mock<IAgentManagerService>();
			this.refreshServerServiceMock = new Mock<IRefreshServerService>();
			this.serverRepositoryMock = new Mock<IServerRepository>();
			this.configurationRepositoryMock = new Mock<IConfigurationRepository>();
			this.loggerMock = TestUtilities.GetMockLogger();
			this.postInstallService = new PostInstallService(
				this.agentManagerServiceMock.Object,
				this.refreshServerServiceMock.Object,
				this.serverRepositoryMock.Object,
				this.configurationRepositoryMock.Object,
				this.loggerMock.Object);
		}

		[Test]
		public void RunOnce_NotImplemented()
		{
			Assert.Throws<NotImplementedException>(() => this.postInstallService.RunOnce());
		}

		[Test]
		public async Task RunEveryTimeAsync_EmptyServerList()
		{
			// Arrange
			var serverList = new List<ResourceServer>();
			this.refreshServerServiceMock.Setup(m => m.GetServerList()).Returns(serverList);

			this.serverRepositoryMock.Setup(m => m.UpdateActiveServersPendingQosDeploymentAsync()).Returns(Task.Delay(1));
			this.agentManagerServiceMock.Setup(m => m.StartPerformanceDashboardAgentsAsync()).Returns(Task.Delay(1));

			var configs = new List<RelativityConfigurationInfo>();
			this.configurationRepositoryMock.Setup(m =>
					m.ReadEddsConfigurationInfoAsync(ConfigurationKeys.Section, ConfigurationKeys.CreateAgentsOnInstall))
				.ReturnsAsync(configs);

			// Act
			var result = await this.postInstallService.RunEveryTimeAsync();

			// Assert
			Assert.That(result.Success, Is.True);
			this.refreshServerServiceMock.VerifyAll();
			this.serverRepositoryMock.VerifyAll();
			this.agentManagerServiceMock.VerifyAll();
		}

		[Test]
		public async Task RunEveryTimeAsync_Success_NoConfig()
		{
			// Arrange
			var serverList = new List<ResourceServer> { new ResourceServer() };
			this.refreshServerServiceMock.Setup(m => m.GetServerList()).Returns(serverList);
			this.refreshServerServiceMock.Setup(m => m.UpdateServerList(serverList)).Verifiable();

			this.serverRepositoryMock.Setup(m => m.UpdateActiveServersPendingQosDeploymentAsync()).Returns(Task.Delay(1));
			this.agentManagerServiceMock.Setup(m => m.StartPerformanceDashboardAgentsAsync()).Returns(Task.Delay(1));
			var configInfo = new List<RelativityConfigurationInfo>();
			this.configurationRepositoryMock
				.Setup(m => m.ReadEddsConfigurationInfoAsync(ConfigurationKeys.Section, ConfigurationKeys.CreateAgentsOnInstall))
				.ReturnsAsync(configInfo);

			// Act
			var result = await this.postInstallService.RunEveryTimeAsync();

			// Assert
			Assert.That(result.Success, Is.True);
			this.refreshServerServiceMock.VerifyAll();
			this.serverRepositoryMock.VerifyAll();
			this.agentManagerServiceMock.VerifyAll();
		}

		[Test]
		public async Task RunEveryTimeAsync_Success_ConfigSet()
		{
			// Arrange
			var serverList = new List<ResourceServer> { new ResourceServer() };
			this.refreshServerServiceMock.Setup(m => m.GetServerList()).Returns(serverList);
			this.refreshServerServiceMock.Setup(m => m.UpdateServerList(serverList)).Verifiable();

			this.serverRepositoryMock.Setup(m => m.UpdateActiveServersPendingQosDeploymentAsync()).Returns(Task.Delay(1));
			this.agentManagerServiceMock.Setup(m => m.StartPerformanceDashboardAgentsAsync()).Returns(Task.Delay(1));
			var machineName = "TestServer";
			var configInfo = new List<RelativityConfigurationInfo> { new RelativityConfigurationInfo { MachineName = machineName, Name = ConfigurationKeys.CreateAgentsOnInstall, Section = ConfigurationKeys.Section, Value = "true" } };
			this.configurationRepositoryMock
				.Setup(m => m.ReadEddsConfigurationInfoAsync(ConfigurationKeys.Section, ConfigurationKeys.CreateAgentsOnInstall))
				.ReturnsAsync(configInfo);
			var createdAgents = new List<int>();
			this.agentManagerServiceMock.Setup(m => m.CreatePerformanceDashboardAgents(machineName)).ReturnsAsync(createdAgents);

			// Act
			var result = await this.postInstallService.RunEveryTimeAsync();

			// Assert
			Assert.That(result.Success, Is.True);
			this.refreshServerServiceMock.VerifyAll();
			this.serverRepositoryMock.VerifyAll();
			this.agentManagerServiceMock.VerifyAll();
		}

		[Test]
		public async Task RunEveryTimeAsync_Success_ConfigInvalid()
		{
			// Arrange
			var serverList = new List<ResourceServer> { new ResourceServer() };
			this.refreshServerServiceMock.Setup(m => m.GetServerList()).Returns(serverList);
			this.refreshServerServiceMock.Setup(m => m.UpdateServerList(serverList)).Verifiable();

			this.serverRepositoryMock.Setup(m => m.UpdateActiveServersPendingQosDeploymentAsync()).Returns(Task.Delay(1));
			this.agentManagerServiceMock.Setup(m => m.StartPerformanceDashboardAgentsAsync()).Returns(Task.Delay(1));
			var machineName = "";
			var configInfo = new List<RelativityConfigurationInfo> { new RelativityConfigurationInfo { MachineName = machineName, Name = ConfigurationKeys.CreateAgentsOnInstall, Section = ConfigurationKeys.Section, Value = "true" } };
			this.configurationRepositoryMock
				.Setup(m => m.ReadEddsConfigurationInfoAsync(ConfigurationKeys.Section, ConfigurationKeys.CreateAgentsOnInstall))
				.ReturnsAsync(configInfo);

			// Act
			var result = await this.postInstallService.RunEveryTimeAsync();

			// Assert
			Assert.That(result.Success, Is.True);
			this.loggerMock.Verify(m=>m.LogWarning(Messages.Warning.PostInstallAgentCreateWarning, (string)null));
			this.refreshServerServiceMock.VerifyAll();
			this.serverRepositoryMock.VerifyAll();
			this.agentManagerServiceMock.VerifyAll();
		}

		[Test]
		public async Task RunEveryTimeAsync_CreateAgentsThrows()
		{
			// Arrange
			var serverList = new List<ResourceServer> { new ResourceServer() };
			this.refreshServerServiceMock.Setup(m => m.GetServerList()).Returns(serverList);
			this.refreshServerServiceMock.Setup(m => m.UpdateServerList(serverList)).Verifiable();

			this.serverRepositoryMock.Setup(m => m.UpdateActiveServersPendingQosDeploymentAsync()).Returns(Task.Delay(1));
			this.agentManagerServiceMock.Setup(m => m.StartPerformanceDashboardAgentsAsync()).Returns(Task.Delay(1));
			var machineName = "TestServer";
			var configInfo = new List<RelativityConfigurationInfo> { new RelativityConfigurationInfo { MachineName = machineName, Name = ConfigurationKeys.CreateAgentsOnInstall, Section = ConfigurationKeys.Section, Value = "true" } };
			this.configurationRepositoryMock
				.Setup(m => m.ReadEddsConfigurationInfoAsync(ConfigurationKeys.Section, ConfigurationKeys.CreateAgentsOnInstall))
				.ReturnsAsync(configInfo);

			var exceptionToThrow = new Exception("Hi");
			this.agentManagerServiceMock.Setup(m => m.CreatePerformanceDashboardAgents(machineName)).ThrowsAsync(exceptionToThrow);

			// Act
			var result = await this.postInstallService.RunEveryTimeAsync();

			// Assert
			Assert.That(result.Success, Is.False);
			Assert.That(result.Message, Is.EqualTo(string.Format(Messages.Exception.PostInstallAgentCreateFailure, exceptionToThrow.ToString())));
			this.refreshServerServiceMock.VerifyAll();
			this.serverRepositoryMock.VerifyAll();
			this.agentManagerServiceMock.VerifyAll();
		}

		[Test]
		public async Task RunEveryTimeAsync_ConfigThrows()
		{
			// Arrange
			var serverList = new List<ResourceServer> { new ResourceServer() };
			this.refreshServerServiceMock.Setup(m => m.GetServerList()).Returns(serverList);
			this.refreshServerServiceMock.Setup(m => m.UpdateServerList(serverList)).Verifiable();

			this.serverRepositoryMock.Setup(m => m.UpdateActiveServersPendingQosDeploymentAsync()).Returns(Task.Delay(1));
			this.agentManagerServiceMock.Setup(m => m.StartPerformanceDashboardAgentsAsync()).Returns(Task.Delay(1));
			
			var exceptionToThrow = new Exception("Hi");
			this.configurationRepositoryMock
				.Setup(m => m.ReadEddsConfigurationInfoAsync(ConfigurationKeys.Section, ConfigurationKeys.CreateAgentsOnInstall))
				.Throws(exceptionToThrow);
			
			// Act
			var result = await this.postInstallService.RunEveryTimeAsync();

			// Assert
			Assert.That(result.Success, Is.False);
			Assert.That(result.Message, Is.EqualTo(string.Format(Messages.Exception.PostInstallAgentCreateFailure, exceptionToThrow.ToString())));
			this.refreshServerServiceMock.VerifyAll();
			this.serverRepositoryMock.VerifyAll();
			this.agentManagerServiceMock.VerifyAll();
		}

		[Test]
		public async Task RunEveryTimeAsync_AgentThrows()
		{
			// Arrange
			var serverList = new List<ResourceServer> { new ResourceServer() };
			this.refreshServerServiceMock.Setup(m => m.GetServerList()).Returns(serverList);
			this.refreshServerServiceMock.Setup(m => m.UpdateServerList(serverList)).Verifiable();

			this.serverRepositoryMock.Setup(m => m.UpdateActiveServersPendingQosDeploymentAsync()).Returns(Task.Delay(1));

			var exceptionToThrow = new Exception("Hi");
			this.agentManagerServiceMock.Setup(m => m.StartPerformanceDashboardAgentsAsync()).Throws(exceptionToThrow);

			// Act
			var result = await this.postInstallService.RunEveryTimeAsync();

			// Assert
			Assert.That(result.Success, Is.False);
			Assert.That(result.Message, Is.EqualTo(string.Format(Messages.Exception.PostInstallAgentEnableFailure, exceptionToThrow.ToString())));
			this.refreshServerServiceMock.VerifyAll();
			this.serverRepositoryMock.VerifyAll();
			this.agentManagerServiceMock.VerifyAll();
		}

		[Test]
		public async Task RunEveryTimeAsync_UpdatePendingQoSThrows()
		{
			// Arrange
			var serverList = new List<ResourceServer> { new ResourceServer() };
			this.refreshServerServiceMock.Setup(m => m.GetServerList()).Returns(serverList);
			this.refreshServerServiceMock.Setup(m => m.UpdateServerList(serverList)).Verifiable();

			var exceptionToThrow = new Exception("Hi");
			this.serverRepositoryMock.Setup(m => m.UpdateActiveServersPendingQosDeploymentAsync()).Throws(exceptionToThrow);

			// Act
			var result = await this.postInstallService.RunEveryTimeAsync();

			// Assert
			Assert.That(result.Success, Is.False);
			Assert.That(result.Message, Is.EqualTo(string.Format(Messages.Exception.PostInstallUpdateServerQoSFailure, exceptionToThrow.ToString())));
			this.refreshServerServiceMock.VerifyAll();
			this.serverRepositoryMock.VerifyAll();
			this.agentManagerServiceMock.VerifyAll();
		}

		[Test]
		public async Task RunEveryTimeAsync_UpdateServerListThrows()
		{
			// Arrange
			var serverList = new List<ResourceServer> { new ResourceServer() };
			this.refreshServerServiceMock.Setup(m => m.GetServerList()).Returns(serverList);

			var exceptionToThrow = new Exception("Hi");
			this.refreshServerServiceMock.Setup(m => m.UpdateServerList(serverList)).Throws(exceptionToThrow);

			// Act
			var result = await this.postInstallService.RunEveryTimeAsync();

			// Assert
			Assert.That(result.Success, Is.False);
			Assert.That(result.Message, Is.EqualTo(string.Format(Messages.Exception.PostInstallServerRefreshFailure, exceptionToThrow.ToString())));
			this.refreshServerServiceMock.VerifyAll();
			this.serverRepositoryMock.VerifyAll();
			this.agentManagerServiceMock.VerifyAll();
		}

		[Test]
		public async Task RunEveryTimeAsync_GetServerListThrows()
		{
			// Arrange
			var exceptionToThrow = new Exception("Hi");
			this.refreshServerServiceMock.Setup(m => m.GetServerList()).Throws(exceptionToThrow);

			// Act
			var result = await this.postInstallService.RunEveryTimeAsync();

			// Assert
			Assert.That(result.Success, Is.False);
			Assert.That(result.Message, Is.EqualTo(string.Format(Messages.Exception.PostInstallServerRefreshFailure, exceptionToThrow.ToString())));
			this.refreshServerServiceMock.VerifyAll();
			this.serverRepositoryMock.VerifyAll();
			this.agentManagerServiceMock.VerifyAll();
		}
	}
}
