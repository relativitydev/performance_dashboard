namespace kCura.PDB.Service.Tests.Logic.Agent
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using Core.Interfaces.Repositories;
	using Core.Models;
	using kCura.PDB.Core.Interfaces.Agent;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Service.Agent;
	using Moq;
	using NUnit.Framework;
	using LogLevel = kCura.PDB.Core.Models.LogLevel;

	[TestFixture, Category("Unit")]
	public class DatabaseLoggerTests
	{
		[SetUp]
		public void Setup()
		{
			this.logRepository = new Mock<ILogRepository>();
			this.logService = new Mock<ILogService>();
			this.agentService = new Mock<IAgentService>();
			this.logContext = new Mock<ILogContext>();
			this.eventRepository = new Mock<IEventRepository>();
			this.logger = new DatabaseLogger(
				this.logRepository.Object,
				this.logService.Object,
				this.logContext.Object,
				this.eventRepository.Object,
				this.agentService.Object);
		}

		private Mock<ILogRepository> logRepository;
		private Mock<ILogService> logService;
		private Mock<IAgentService> agentService;
		private Mock<ILogContext> logContext;
		private Mock<IEventRepository> eventRepository;
		private DatabaseLogger logger;

		[Test]
		public void DatabaseLogger_LogAll()
		{
			// Arrange
			logRepository.Setup(l => l.Create(It.IsAny<LogEntry>()))
				.Returns(1234);
			logService.Setup(l => l.ShouldLog(It.IsAny<int>(), It.IsAny<IList<string>>()))
				.Returns(true);
			agentService.SetupGet(s => s.AgentID).Returns(10);

			// Act
			logger.LogCritical("", "");
			logger.LogCritical("", new List<string>());
			logger.LogError("", "");
			logger.LogError("", new Exception(), "");
			logger.LogError("", new List<string>());
			logger.LogInformation("", "");
			logger.LogInformation("", new List<string>());
			logger.LogVerbose("", "");
			logger.LogVerbose("", new List<string>());
			logger.LogWarning("", "");
			logger.LogWarning("", new List<string>());

			// Assert
			logRepository.Verify(l => l.Create(It.IsAny<LogEntry>()), Times.Exactly(11));
		}

		[Test]
		public void DatabaseLogger_LogNone()
		{
			// Arrange
			logRepository.Setup(l => l.Create(It.IsAny<LogEntry>()))
				.Returns(1234);
			logService.Setup(l => l.ShouldLog(It.IsAny<int>(), It.IsAny<IList<string>>()))
				.Returns(false);
			agentService.SetupGet(s => s.AgentID).Returns(10);

			// Act
			logger.LogCritical("", "");
			logger.LogCritical("", new List<string>());
			logger.LogError("", "");
			logger.LogError("", new Exception(), "");
			logger.LogError("", new List<string>());
			logger.LogInformation("", "");
			logger.LogInformation("", new List<string>());
			logger.LogVerbose("", "");
			logger.LogVerbose("", new List<string>());
			logger.LogWarning("", "");
			logger.LogWarning("", new List<string>());

			// Assert
			logRepository.Verify(l => l.Create(It.IsAny<LogEntry>()), Times.Exactly(0));
		}
	}
}
