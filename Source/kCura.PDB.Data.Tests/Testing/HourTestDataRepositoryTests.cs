namespace kCura.PDB.Data.Tests.Testing
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;

	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Models.Testing;
	using kCura.PDB.Data.Repositories;
	using kCura.PDB.Data.Testing;
	using kCura.PDB.Data.Tests.Repositories;
	using NUnit.Framework;

	[TestFixture]
	[Category("UnitPlatform")]
	[Category("Integration")]
	public class HourTestDataRepositoryTests
	{
		private HourTestDataRepository hourTestDataRepository;

		[SetUp]
		public void Setup()
		{
			this.hourTestDataRepository = new HourTestDataRepository(ConnectionFactorySetup.ConnectionFactory, new HourRepository(ConnectionFactorySetup.ConnectionFactory));
		}

		[Test]
		public async Task CreateAsync()
		{
			var testData = new MockHour { HourTimeStamp = DateTime.UtcNow.NormilizeToHour() };
			var testDataList = new List<MockHour> { testData };
			await this.hourTestDataRepository.CreateAsync(testDataList);
			Assert.Pass("No return results");
		}

		[Test]
		public async Task ClearAsync()
		{
			await this.hourTestDataRepository.ClearAsync();
			Assert.Pass("No return results");
		}
	}
}
