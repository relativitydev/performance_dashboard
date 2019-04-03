namespace kCura.PDB.Service.Tests.Testing
{
	using System.Threading;
	using System.Threading.Tasks;

	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Interfaces.Queuing;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Service.Testing;

	using Moq;

	using NUnit.Framework;

	[TestFixture]
	[Category("Unit")]
	public class TestAgentRunnerTests
	{
		private TestAgentRunner testAgentRunner;

		private Mock<IMetricSystemManagerService> metricSystemManagerServiceMock;
		private Mock<IMetricSystemWorkerService> metricSystemWorkerServiceMock;
		private Mock<IAppSettingsConfigurationService> configurationServiceMock;

		[SetUp]
		public void Setup()
		{
			this.metricSystemManagerServiceMock = new Mock<IMetricSystemManagerService>();
			this.metricSystemWorkerServiceMock = new Mock<IMetricSystemWorkerService>();
			this.configurationServiceMock = new Mock<IAppSettingsConfigurationService>();
			this.testAgentRunner = new TestAgentRunner(this.metricSystemManagerServiceMock.Object, this.metricSystemWorkerServiceMock.Object, this.configurationServiceMock.Object);
		}

		[Test]
		public void GetAgentExecutionTasks()
		{
			// Arrange
			var relativityManagerAgent = false.ToString();
			var relativityWorkerAgent = false.ToString();
			var testCancellationToken = new CancellationToken();

			this.configurationServiceMock.Setup(m => m.ContainsAppSettingsKey(Names.Configuration.ManagerAgentInRelativity))
				.Returns(true);
			this.configurationServiceMock.Setup(m => m.GetAppSetting(Names.Configuration.ManagerAgentInRelativity))
				.Returns(relativityManagerAgent);
			this.configurationServiceMock.Setup(m => m.ContainsAppSettingsKey(Names.Configuration.WorkerAgentInRelativity))
				.Returns(true);
			this.configurationServiceMock.Setup(m => m.GetAppSetting(Names.Configuration.WorkerAgentInRelativity))
				.Returns(relativityWorkerAgent);

			var managerTask = Task.FromResult(false);
			this.metricSystemManagerServiceMock.Setup(m => m.Execute(testCancellationToken)).Returns(managerTask);
			var workerTask = Task.FromResult(true);
			this.metricSystemWorkerServiceMock.Setup(m => m.Execute(testCancellationToken, null)).Returns(workerTask);

			// Act
			var result = this.testAgentRunner.GetAgentExecutionTasks(testCancellationToken);

			// Assert
			Assert.That(result.Count, Is.EqualTo(2));
			Assert.That(result, Contains.Item(managerTask));
			Assert.That(result, Contains.Item(workerTask));
			this.metricSystemManagerServiceMock.VerifyAll();
			this.metricSystemWorkerServiceMock.VerifyAll();
			this.configurationServiceMock.VerifyAll();
		}
	}
}
