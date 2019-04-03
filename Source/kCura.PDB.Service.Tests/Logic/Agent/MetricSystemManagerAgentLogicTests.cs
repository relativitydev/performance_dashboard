namespace kCura.PDB.Service.Tests.Logic.Agent
{
	using System.Collections.Generic;
	using System.Threading;
	using System.Threading.Tasks;
	using Core.Interfaces.Queuing;
	using Core.Interfaces.Services;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Interfaces.Agent;
	using kCura.PDB.Core.Interfaces.DatabaseDeployment;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Data.Repositories;
	using kCura.PDB.Service.Agent;
	using kCura.PDB.Tests.Common;
	using Moq;
	using NUnit.Framework;

	[TestFixture]
	public class MetricSystemManagerAgentLogicTests
	{
		[SetUp]
		public void Setup()
		{
			this.queuingConfiguration = new Mock<IQueuingConfiguration>();
			this.eventSourceService = new Mock<IEventSourceService>();
			this.configurationRepository = new Mock<IConfigurationRepository>();
			this.logger = TestUtilities.GetMockLogger();
			this.agentSerivce = new Mock<IAgentService>();
			this.agentRepository = new Mock<IAgentRepository>();
			this.eventOrphanService = new Mock<IEventOrphanService>();
			this.metricManagerStatsRepository = new Mock<IMetricManagerStatsRepository>();
			this.managerLogic = new MetricSystemManagerAgentLogic(
				this.queuingConfiguration.Object,
				this.eventSourceService.Object,
				this.configurationRepository.Object,
				this.logger.Object,
				this.agentSerivce.Object,
				this.agentRepository.Object,
				this.eventOrphanService.Object,
				this.metricManagerStatsRepository.Object);
		}

		private Mock<IQueuingConfiguration> queuingConfiguration;
		private Mock<IEventSourceService> eventSourceService;
		private Mock<IConfigurationRepository> configurationRepository;
		private Mock<ILogger> logger;
		private Mock<IAgentService> agentSerivce;
		private Mock<IAgentRepository> agentRepository;
		private Mock<IEventOrphanService> eventOrphanService;
		private MetricSystemManagerAgentLogic managerLogic;
		private Mock<IMetricManagerStatsRepository> metricManagerStatsRepository;

		// Note, these tests are time-based, so debugging with breakpoints will skew results
		[Test]
		[Category("Unit")]
		[Category("Explicit")]
		[Explicit("Runs too long for TeamCity")]
		[TestCase(5, 2, 500, 20, 2, 5, 10)]
		[TestCase(1, 2, 100, 20, 1, 2, 2)]
		public async Task MetricSystemManagerAgentLogic_RunLogic(
			int bootstrapInterval, // bootstrap interval
			int bootstrapTimesCalled, // expected number of times bootstrap is called
			int enqueueTasksInterval,
			int enqueueTasksTimesCalled, // expected number of times enqueue tasks is called
			int resolveOrphanedEventsInterval,
			int resolveOrphanedEventsCalled, // expected number of times resolved orphaned events is called
			int eventManagerRunInterval) // Total expected time the manager will run
		{
			// Arrange
			var cancellationToken = new CancellationToken(false);
			var agentId = 123;
			this.agentSerivce.Setup(m => m.AgentID).Returns(agentId);
			this.agentRepository.Setup(m => m.ReadAgentEnabled(agentId)).Returns(true);

			this.eventSourceService.Setup(s => s.EnqueueTasksForPendingEvents()).Returns(Task.Delay(1));
			this.queuingConfiguration.Setup(s => s.ConfigureSystem());
			this.eventOrphanService.Setup(s => s.ResolveOrphanedEventLocks()).Returns(Task.Delay(1));
			this.eventOrphanService.Setup(s => s.ResolveTimedOutEvents()).Returns(Task.Delay(1));

			this.configurationRepository.Setup(m => m.ReadValueAsync<int>(ConfigurationKeys.CreateBootstrapEventsInterval)).ReturnsAsync(bootstrapInterval); // Seconds
			this.configurationRepository.Setup(m => m.ReadValueAsync<int>(ConfigurationKeys.EnqueueTasksInterval)).ReturnsAsync(enqueueTasksInterval); // Milliseconds
			this.configurationRepository.Setup(m => m.ReadValueAsync<int>(ConfigurationKeys.EventManagerRunInterval)).ReturnsAsync(eventManagerRunInterval); // Seconds
			this.configurationRepository.Setup(m => m.ReadValueAsync<int>(ConfigurationKeys.ResolveOrphanedEventsInterval)).ReturnsAsync(resolveOrphanedEventsInterval); // Seconds

			this.metricManagerStatsRepository.Setup(r => r.CreateAsync(It.IsAny<IList<MetricManagerExecutionStat>>()))
				.Returns(Task.FromResult(1));

			// Act
			await this.managerLogic.Execute(cancellationToken);

			// Assert
			this.eventSourceService.Verify(m => m.CreateHourProcessingEvents(), Times.Between(bootstrapTimesCalled - 1, bootstrapTimesCalled + 1, Range.Inclusive));
			this.eventSourceService.Verify(m => m.EnqueueTasksForPendingEvents(), Times.Between(enqueueTasksTimesCalled - 1, enqueueTasksTimesCalled + 1, Range.Inclusive));
			this.eventOrphanService.Verify(s => s.ResolveOrphanedEventLocks(), Times.Between(resolveOrphanedEventsCalled - 1, resolveOrphanedEventsCalled + 1, Range.Inclusive));
			this.eventOrphanService.Verify(s => s.ResolveTimedOutEvents(), Times.Between(resolveOrphanedEventsCalled - 1, resolveOrphanedEventsCalled + 1, Range.Inclusive));
		}

		[Test]
		[Category("Unit")]
		[Explicit("Runs too long for TeamCity")]
		public async Task MetricSystemManagerAgentLogic_DisabledAgent()
		{
			// Arrange
			var cancellationToken = new CancellationToken(false);
			var agentId = 123;
			this.agentSerivce.Setup(m => m.AgentID).Returns(agentId);
			this.agentRepository.Setup(m => m.ReadAgentEnabled(agentId)).Returns(false);

			this.eventSourceService.Setup(s => s.EnqueueTasksForPendingEvents()).Returns(Task.Delay(10));
			this.queuingConfiguration.Setup(s => s.ConfigureSystem());

			// Act
			await this.managerLogic.Execute(cancellationToken);

			// Assert
			this.eventSourceService.Verify(m => m.CreateHourProcessingEvents(), Times.Once);
			this.eventSourceService.Verify(m => m.EnqueueTasksForPendingEvents(), Times.Once);

		}
	}
}
