namespace kCura.PDB.Data.Tests.Testing
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;

	using kCura.PDB.Core.Models.Testing;
	using kCura.PDB.Data.Testing;
	using kCura.PDB.Data.Tests.Repositories;
	using NUnit.Framework;

	[TestFixture]
	[Category("UnitPlatform")]
	[Category("Integration")]
	public class DbccTestDataRepositoryTests
	{
		private DbccTestDataRepository dbccTestDataRepository;

		[SetUp]
		public void Setup()
		{
			this.dbccTestDataRepository = new DbccTestDataRepository(ConnectionFactorySetup.ConnectionFactory);
		}

		[Test]
		public async Task CreateAsync()
		{
			var testData = new MockDbccServerResults
				               {
					               Server = "ServerNameTest",
					               Database = "DatabaseNameTest",
								   LastCleanDBCCDate = DateTime.UtcNow, // shouldn't be nullable?
				               };
			var testDataList = new List<MockDbccServerResults> { testData };
			await this.dbccTestDataRepository.CreateAsync(testDataList);
			Assert.Pass("No return results");
		}

		[Test]
		public async Task ClearAsync()
		{
			await this.dbccTestDataRepository.ClearAsync();
			Assert.Pass("No return results");
		}
	}
}
