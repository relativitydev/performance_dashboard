namespace kCura.PDB.Service.Tests.ProcessControls.HealthPerformance
{
	using kCura.PDB.Core.Models;
	using kCura.PDB.Data.Repositories;
	using kCura.PDB.Service.ProcessControls.ProcessTasks;
	using kCura.PDB.Tests.Common;
	using NUnit.Framework;

	[TestFixture, Category("Integration")]
	public class LoadServerHealthSummaryTaskTests
	{
		private LoadServerHealthSummaryTask task;

		[SetUp]
		public void SetUp()
		{
			var logger = TestUtilities.GetMockLogger();
			var connectionFactory = TestUtilities.GetIntegrationConnectionFactory();
			var sqlRepo = new SqlServerRepository(connectionFactory);
			var agentId = 37715;

			this.task = new LoadServerHealthSummaryTask(logger.Object, sqlRepo, agentId);
		}

		[Test]
		public void Execute()
		{
			// Arrange
			var processControl = new ProcessControl();

			// Act
			var result = this.task.Execute(processControl);

			// Assert
			Assert.That(result, Is.True);
		}
	}
}
