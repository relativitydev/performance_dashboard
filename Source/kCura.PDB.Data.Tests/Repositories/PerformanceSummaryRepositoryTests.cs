namespace kCura.PDB.Data.Tests.Repositories
{
	using System;
	using Core.Interfaces.Services;
	using Data.Repositories;
	using Data.Services;
	using NUnit.Framework;
	using PDB.Tests.Common;
	using Service.Services;

	[TestFixture, Category("Integration")]
	public class PerformanceSummaryRepositoryTests
	{
		[OneTimeSetUp]
		public void OneTimeSetup()
		{
			connectionFatory = TestUtilities.GetIntegrationConnectionFactory();
			perfSummaryRepo = new PerformanceSummaryRepository(connectionFatory);
		}

		private IConnectionFactory connectionFatory;
		private PerformanceSummaryRepository perfSummaryRepo;

		[Test]
		public void LoadApplicationHealthSummary()
		{
			// Arrange
			var processExecDate = DateTime.UtcNow;

			// Act
			perfSummaryRepo.LoadApplicationHealthSummary(processExecDate);

			// Assert
			Assert.Pass("no returned result");
		}

		[Test]
		public void LoadErrorHealthDwData()
		{
			// Arrange
			var processExecDate = DateTime.UtcNow;

			// Act
			perfSummaryRepo.LoadErrorHealthDwData(processExecDate);

			// Assert
			Assert.Pass("no returned result");
		}

		[Test]
		public void LoadUserHealthDwData()
		{
			// Arrange
			var processExecDate = DateTime.UtcNow;

			// Act
			perfSummaryRepo.LoadUserHealthDwData(processExecDate);

			// Assert
			Assert.Pass("no returned result");
		}
	}
}
