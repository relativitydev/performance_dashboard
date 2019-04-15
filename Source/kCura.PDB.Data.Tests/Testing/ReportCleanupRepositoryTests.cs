namespace kCura.PDB.Data.Tests.Testing
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using kCura.PDB.Data.Testing;
	using kCura.PDB.Data.Tests.Repositories;
	using NUnit.Framework;

	[TestFixture]
	[Category("UnitPlatform")]
	[Category("Integration")]
	public class ReportCleanupRepositoryTests
	{
		private ReportCleanupRepository reportCleanupRepository;

		[SetUp]
		public void Setup()
		{
			this.reportCleanupRepository = new ReportCleanupRepository(ConnectionFactorySetup.ConnectionFactory);
		}

		[Test]
		public async Task ClearReportData()
		{
			var testData = new List<DateTime>();
			await this.reportCleanupRepository.ClearReportData(testData);
			Assert.Pass("No return results");
		}

		[Test]
		public async Task ClearExistingBackupHistory()
		{
			await this.reportCleanupRepository.ClearExistingBackupHistory();
			Assert.Pass("No return results");
		}

		[Test]
		public async Task ClearExistingDbccServerResults()
		{
			await this.reportCleanupRepository.ClearExistingDbccHistory();
			Assert.Pass("No return results");
		}
	}
}
