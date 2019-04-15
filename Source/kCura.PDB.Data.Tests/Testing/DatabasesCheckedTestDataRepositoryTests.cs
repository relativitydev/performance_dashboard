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
	public class DatabasesCheckedTestDataRepositoryTests
	{
		private DatabasesCheckedTestDataRepository databasesCheckedTestDataRepository;

		[SetUp]
		public void Setup()
		{
			this.databasesCheckedTestDataRepository = new DatabasesCheckedTestDataRepository(ConnectionFactorySetup.ConnectionFactory);
		}

		[Test]
		public async Task CreateAsync()
		{
			var testData = new MockDatabaseChecked
				               {
					               Server = "ServerNameTest",
					               Database = "DatabaseNameTest",
								   CreatedOn = DateTime.UtcNow.NormilizeToHour()
				               };
			var testDataList = new List<MockDatabaseChecked> { testData };
			await this.databasesCheckedTestDataRepository.CreateAsync(testDataList);
			Assert.Pass("No return results");
		}

		[Test]
		public async Task ClearAsync()
		{
			await this.databasesCheckedTestDataRepository.ClearAsync();
			Assert.Pass("No return results");
		}
	}
}
