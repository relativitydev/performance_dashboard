namespace kCura.PDB.Service.Tests.Logic.Agent
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using Core.Interfaces.Repositories;
	using Core.Models;
	using kCura.PDB.Core.Interfaces.Agent;
	using kCura.PDB.Service.Agent;
	using Moq;
	using NUnit.Framework;

	[TestFixture, Category("Unit")]
	public class AgentLoggerTests
	{
		[SetUp]
		public void Setup()
		{
			agentService = new Mock<IAgentService>();
		}

		private Mock<IAgentService> agentService;

		[Test]
		public void AgentLogger_LogAll()
		{
			// Arrange
			this.agentService.Setup(s=>s.RaiseError(It.IsAny<string>(), string.Empty));
			this.agentService.Setup(s=>s.RaiseWarning(It.IsAny<string>(), string.Empty));
			this.agentService.Setup(s=>s.RaiseMessage(It.IsAny<string>(), It.IsAny<int>()));
			var logger = new AgentLogger(agentService.Object);

			// Act
			logger.LogCritical("", "");
			logger.LogCritical("", new List<string>());
			logger.LogError("", "");
			logger.LogError("", new List<string>());
			logger.LogError("", new Exception(), "");
			logger.LogInformation("", "");
			logger.LogInformation("", new List<string>());
			logger.LogVerbose("", "");
			logger.LogVerbose("", new List<string>());
			logger.LogWarning("", "");
			logger.LogWarning("", new List<string>());

			// Assert
			this.agentService.Verify(s => s.RaiseError(It.IsAny<string>(), string.Empty), Times.Exactly(5));
			this.agentService.Verify(s => s.RaiseWarning(It.IsAny<string>(), string.Empty), Times.Exactly(2));
			this.agentService.Verify(s => s.RaiseMessage(It.IsAny<string>(), It.IsAny<int>()), Times.Exactly(4));
		}
	}
}
