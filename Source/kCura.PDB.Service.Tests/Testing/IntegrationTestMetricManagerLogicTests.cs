namespace kCura.PDB.Service.Tests.Testing
{
	using System.Threading;
	using System.Threading.Tasks;

	using kCura.PDB.Core.Interfaces.Queuing;
	using kCura.PDB.Service.Testing;
	using kCura.PDB.Tests.Common.Extensions;

	using Moq;

	using NUnit.Framework;

	[TestFixture]
	[Category("Unit")]
	public class IntegrationTestMetricManagerLogicTests
	{
		private IntegrationTestMetricManagerLogic logic;
		private Mock<IEventSourceService> eventSourceServiceMock;
		private Mock<IQueuingConfiguration> queuingConfigurationMock;

		[SetUp]
		public void Setup()
		{
			this.eventSourceServiceMock = new Mock<IEventSourceService>();
			this.queuingConfigurationMock = new Mock<IQueuingConfiguration>();
			this.logic = new IntegrationTestMetricManagerLogic(this.eventSourceServiceMock.Object, this.queuingConfigurationMock.Object);
		}

		[Test]
		[Explicit("Too long for TeamCity")]
		public async Task Execute()
		{
			this.queuingConfigurationMock.Setup(m => m.ConfigureSystem()).Verifiable();
			this.eventSourceServiceMock.Setup(m => m.EnqueueTasksForPendingEvents()).ReturnsAsyncDefault();
			this.eventSourceServiceMock.Setup(m => m.CreateHourProcessingEvents()).ReturnsAsyncDefault();

			var testCancellationTokenSource = new CancellationTokenSource(2000);
			await this.logic.Execute(testCancellationTokenSource.Token);

			this.queuingConfigurationMock.Verify(m => m.ConfigureSystem(), Times.Once);
			this.eventSourceServiceMock.Verify(m => m.EnqueueTasksForPendingEvents(), Times.AtLeastOnce);
			this.eventSourceServiceMock.Verify(m => m.CreateHourProcessingEvents(), Times.AtLeastOnce);
		}
	}
}
