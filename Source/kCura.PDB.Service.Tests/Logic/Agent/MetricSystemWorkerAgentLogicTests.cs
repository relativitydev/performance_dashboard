namespace kCura.PDB.Service.Tests.Logic.Agent
{
	using System.Threading;
	using System.Threading.Tasks;
	using Core.Interfaces.Queuing;
	using Core.Interfaces.Services;
	using global::Relativity.Toggles;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Toggles;
	using kCura.PDB.Service.Agent;
	using Moq;
	using Ninject;
	using NUnit.Framework;

	[TestFixture]
	public class MetricSystemWorkerAgentLogicTests
	{
		[SetUp]
		public void Setup()
		{
			this.queuingConfiguration = new Mock<IQueuingConfiguration>();
			this.eventWorkerService = new Mock<IEventWorkerService>();
			this.eventOrphanService = new Mock<IEventOrphanService>();
			this.jobServer = new Mock<IJobServer>();
		    this.loggerMock = new Mock<ILogger>();

			this.eventWorkerService.Setup(s => s.CreateWorker(null)).Returns(Task.FromResult(new EventWorker()));
			this.eventWorkerService.Setup(s => s.RemoveCurrentWorker()).Returns(Task.Delay(1));
			this.eventOrphanService.Setup(s => s.MarkOrphanedEventsErrored(It.IsAny<EventWorker>())).Returns(Task.Delay(1));
		}

		private Mock<IQueuingConfiguration> queuingConfiguration;
		private Mock<IEventWorkerService> eventWorkerService;
		private Mock<IEventOrphanService> eventOrphanService;
		private Mock<IJobServer> jobServer;
        private Mock<ILogger> loggerMock;

		[Test]
		[Category("Unit")]
		public async Task MetricSystemWorkerAgentLogic_RunLogic_Unit()
		{
			// Arrange
			var cancellationToken = new CancellationToken(false);
			queuingConfiguration.Setup(qc => qc.ConfigureSystem());
			using (var kernel = new StandardKernel())
			{
				kernel.Bind<IJobServer>().ToConstant(this.jobServer.Object);
				this.queuingConfiguration.Setup(s => s.ConfigureSystem());
				this.jobServer.Setup(s => s.WaitTillProcessesAreDone(cancellationToken)).Returns(Task.Delay(1, cancellationToken));

				var logic = new MetricSystemWorkerAgentLogic(kernel, this.queuingConfiguration.Object, this.eventWorkerService.Object, this.eventOrphanService.Object, this.loggerMock.Object);

				// Act
				await logic.Execute(cancellationToken);
			}

			// Assert
			jobServer.Verify(s => s.WaitTillProcessesAreDone(cancellationToken));
		}

	}
}
