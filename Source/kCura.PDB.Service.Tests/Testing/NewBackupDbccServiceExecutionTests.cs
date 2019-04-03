namespace kCura.PDB.Service.Tests.Testing
{
	using System;
	using System.Collections.Generic;
	using System.Threading;
	using System.Threading.Tasks;

	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Interfaces.Testing.Services;
	using kCura.PDB.Service.Testing;
	using kCura.PDB.Tests.Common.Extensions;

	using Moq;

	using NUnit.Framework;

	[TestFixture]
	[Category("Unit")]
	public class NewBackupDbccServiceExecutionTests
	{
		private NewBackupDbccServiceExecution newBackupDbccServiceExecution;
		private Mock<ITestAgentRunner> testAgentRunnerMock;
		private Mock<ITestPollingService> testPollingServiceMock;
		private Mock<ITestEventPrimer> testEventPrimerMock;
		private Mock<ICancellationTokenSourceFactory> cancellationTokenSourceFactoryMock;

		[SetUp]
		public void Setup()
		{
			this.testAgentRunnerMock = new Mock<ITestAgentRunner>();
			this.testPollingServiceMock = new Mock<ITestPollingService>();
			this.testEventPrimerMock = new Mock<ITestEventPrimer>();
			this.cancellationTokenSourceFactoryMock = new Mock<ICancellationTokenSourceFactory>();
			this.newBackupDbccServiceExecution = new NewBackupDbccServiceExecution(
				this.testAgentRunnerMock.Object,
				this.testPollingServiceMock.Object,
				this.testEventPrimerMock.Object,
				this.cancellationTokenSourceFactoryMock.Object);
		}

		[Test]
		public async Task ExecuteAsync()
		{
			// Arrange
			var testCancellationToken = new CancellationToken();
			var timeoutCancellationTokenSource = new CancellationTokenSource();
			var combinedCancellationTokenSource = new CancellationTokenSource();
			this.cancellationTokenSourceFactoryMock.Setup(m => m.GetTimeoutCancellationTokenSource(It.IsAny<TimeSpan>()))
				.Returns(timeoutCancellationTokenSource);
			this.cancellationTokenSourceFactoryMock
				.Setup(m => m.CreateLinkedTokenSource(testCancellationToken, timeoutCancellationTokenSource.Token))
				.Returns(combinedCancellationTokenSource);

			this.testEventPrimerMock.Setup(m => m.CreateEventDataAsync()).ReturnsAsyncDefault();
			var tasks = new List<Task>();
			this.testAgentRunnerMock.Setup(m => m.GetAgentExecutionTasks(combinedCancellationTokenSource.Token)).Returns(tasks);
			var pollingTask = Task.Delay(10, combinedCancellationTokenSource.Token);
			this.testPollingServiceMock.Setup(m => m.WaitUntilEventCompletionAsync(combinedCancellationTokenSource.Token))
				.Returns(pollingTask);

			// Act
			await this.newBackupDbccServiceExecution.ExecuteAsync(testCancellationToken);

			// Assert
			this.testAgentRunnerMock.VerifyAll();
			this.testPollingServiceMock.VerifyAll();
			this.testEventPrimerMock.VerifyAll();
			this.cancellationTokenSourceFactoryMock.VerifyAll();
		}
	}
}
