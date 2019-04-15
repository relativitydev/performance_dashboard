namespace kCura.PDB.Data.Tests.Testing
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;

	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Models.Testing;
	using kCura.PDB.Data.Testing;
	using kCura.PDB.Data.Tests.Repositories;
	using NUnit.Framework;

	[TestFixture]
	[Category("UnitPlatform")]
	[Category("Integration")]
	public class BackupTestDataRepositoryTests
	{
		private BackupTestDataRepository backupTestDataRepository;

		[SetUp]
		public void Setup()
		{
			this.backupTestDataRepository = new BackupTestDataRepository(ConnectionFactorySetup.ConnectionFactory);
		}

		[Test]
		public async Task CreateAsync()
		{
			var testData = new MockBackupSet
				               {
					               Server = "ServerNameTest",
					               Database = "DatabaseNameTest",
					               BackupStartDate = DateTime.UtcNow.NormilizeToHour(),
					               BackupEndDate = DateTime.UtcNow.NormilizeToHour().AddSeconds(3),
					               BackupType = "D"
				               };
			var testDataList = new List<MockBackupSet> { testData };

			await this.backupTestDataRepository.CreateAsync(testDataList);

			Assert.Pass("No return results");
		}

		[Test]
		public async Task ClearAsync()
		{
			await this.backupTestDataRepository.ClearAsync();

			Assert.Pass("No return results");
		}
	}
}
