namespace kCura.PDB.Service.Tests.DatabaseDeployment
{
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Service.DatabaseDeployment;
	using kCura.PDB.Tests.Common;
	using Moq;
	using NUnit.Framework;

	[TestFixture, Category("Unit")]
	public class ExceptionMigrationResultHandlerTests
	{
		[SetUp]
		public void Setup()
		{
			this.logger = TestUtilities.GetMockLogger();
			this.handler = new ExceptionMigrationResultHandler(logger.Object);
		}

		private Mock<ILogger> logger;
		private ExceptionMigrationResultHandler handler;


		[Test]
		public void HandleDeploymentResponse()
		{
			var resultSet = new MigrationResultSet()
			{
				Success = true,
				Messages = new[]
				{
					new LogMessage(LogSeverity.Debug, "a"),
					new LogMessage(LogSeverity.Info, "b"),
					new LogMessage(LogSeverity.Warning, "c")
				}
			};

			var result = this.handler.HandleDeploymentResponse(resultSet);

			Assert.That(result, Is.Not.Null);
			Assert.That(result.Success, Is.True);
		}
	}
}
