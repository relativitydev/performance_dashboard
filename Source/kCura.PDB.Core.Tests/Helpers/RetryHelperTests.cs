namespace kCura.PDB.Core.Tests.Helpers
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Enumerations;
	using kCura.PDB.Core.Exception;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Tests.Common;
	using Moq;
	using NUnit.Framework;

	[TestFixture, Category("Unit")]
	public class RetryHelperTests
	{
		private Mock<ILogger> loggerMock;
		private Mock<IAuditRepository> testAuditMock;

		[SetUp]
		public void SetUp()
		{
			this.loggerMock = TestUtilities.GetMockLogger();
			this.testAuditMock = new Mock<IAuditRepository>();
		}

		[Test]
		[TestCase(3, 3)]
		[TestCase(5, 3)]
		public async Task RetryCall(int timesToCall, int expectedTimesCalled)
		{
			var workspaceId = 1;
			var startTime = DateTime.UtcNow;
			var endTime = DateTime.UtcNow;
			var actionIds = new List<AuditActionId>();

			this.testAuditMock.SetupSequence(m => m.ReadAnyAuditsAsync(workspaceId, startTime, endTime, actionIds))
				.ThrowsAsync(new RetryException(new Exception()))
				.ThrowsAsync(new RetryException(new Exception()))
				.ReturnsAsync(true);

			await TimeSpan.FromMilliseconds(2).RetryCall(timesToCall, loggerMock.Object, () => this.testAuditMock.Object.ReadAnyAuditsAsync(workspaceId, startTime, endTime, actionIds));

			this.testAuditMock.Verify(m => m.ReadAnyAuditsAsync(workspaceId, startTime, endTime, actionIds), Times.Exactly(expectedTimesCalled));
		}

		[Test]
		public void RetryCall_NonRetryException()
		{
			// Arrange
			var workspaceId = 1;
			var startTime = DateTime.UtcNow;
			var endTime = DateTime.UtcNow;
			var actionIds = new List<AuditActionId>();

			this.testAuditMock.Setup(m => m.ReadAnyAuditsAsync(workspaceId, startTime, endTime, actionIds)).ThrowsAsync(new Exception());

			// Act
			// Assert
			var result = Assert.ThrowsAsync<Exception>(() =>
					TimeSpan.FromMilliseconds(2).RetryCall(2, loggerMock.Object,
						() => this.testAuditMock.Object.ReadAnyAuditsAsync(workspaceId, startTime, endTime, actionIds)));

			this.testAuditMock.Verify(m => m.ReadAnyAuditsAsync(workspaceId, startTime, endTime, actionIds), Times.Once);
			Assert.That(result.GetType(), Is.Not.EqualTo(typeof(RetryException)), "Original exception should be thrown not a retry exception");
		}

		[Test]
		[TestCase(1)]
		[TestCase(2)]
		[TestCase(3)]
		public void RetryCall_ThrowException(int timesToCall)
		{
			// Arrange
			var workspaceId = 1;
			var startTime = DateTime.UtcNow;
			var endTime = DateTime.UtcNow;
			var actionIds = new List<AuditActionId>();

			this.testAuditMock.Setup(m => m.ReadAnyAuditsAsync(workspaceId, startTime, endTime, actionIds)).ThrowsAsync(new RetryException(new Exception()));

			// Act
			// Assert
			var result = Assert.ThrowsAsync<Exception>(() =>
					TimeSpan.FromMilliseconds(2).RetryCall(timesToCall, loggerMock.Object,
						() => this.testAuditMock.Object.ReadAnyAuditsAsync(workspaceId, startTime, endTime, actionIds)));

			this.testAuditMock.Verify(m => m.ReadAnyAuditsAsync(workspaceId, startTime, endTime, actionIds), Times.Exactly(timesToCall));
			Assert.That(result.GetType(), Is.Not.EqualTo(typeof(RetryException)), "Inner exception should be thrown after max retires not the retry exception");
		}
	}
}
