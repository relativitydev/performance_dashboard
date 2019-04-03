namespace kCura.PDB.Service.Tests.Services
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Interfaces.Agent;
	using kCura.PDB.Service.Installation;
	using Moq;
	using NUnit.Framework;

	[TestFixture]
	[Category("Unit")]
	public class PreInstallServiceTests
	{
		private PreInstallService preInstallService;
		private Mock<IAgentManagerService> agentManagerServiceMock;

		[SetUp]
		public void Setup()
		{
			this.agentManagerServiceMock = new Mock<IAgentManagerService>();
			this.preInstallService = new PreInstallService(this.agentManagerServiceMock.Object);
		}

		[Test]
		public void RunOnce_NotImplemented()
		{
			Assert.Throws<NotImplementedException>(() => this.preInstallService.RunOnce());
		}

		[Test]
		public async Task RunEveryTimeAsync_Empty()
		{
			// Arrange
			var stoppedAgents = new List<int>();
			var runningAgents = new List<int>();
			this.agentManagerServiceMock.Setup(m => m.StopPerformanceDashboardAgentsAsync()).ReturnsAsync(stoppedAgents);

			// Act
			var result = await this.preInstallService.RunEveryTimeAsync();

			// Assert
			Assert.That(result.Success, Is.True);
			this.agentManagerServiceMock.VerifyAll();
		}

		[Test]
		public async Task RunEveryTimeAsync_AgentsStopped()
		{
			// Arrange
			var stoppedAgents = new List<int> { 123, 124 };
			this.agentManagerServiceMock.Setup(m => m.StopPerformanceDashboardAgentsAsync()).ReturnsAsync(stoppedAgents);

			// Act
			var result = await this.preInstallService.RunEveryTimeAsync();

			// Assert
			Assert.That(result.Success, Is.True);
			this.agentManagerServiceMock.VerifyAll();
		}

		[Test]
		public async Task RunEveryTimeAsync_StopAgentsThrows()
		{
			// Arrange
			var exceptionToThrow = new Exception("Hi");
			this.agentManagerServiceMock.Setup(m => m.StopPerformanceDashboardAgentsAsync()).ThrowsAsync(exceptionToThrow);

			// Act
			var result = await this.preInstallService.RunEveryTimeAsync();

			// Assert
			Assert.That(result.Success, Is.False);
			Assert.That(result.Message,
				Is.EqualTo(string.Format(Messages.Exception.PreInstallEventHandlerFailure, exceptionToThrow.ToString())));
			this.agentManagerServiceMock.VerifyAll();
		}
	}
}