namespace kCura.PDB.Data.Tests.Testing
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;

	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.Testing;
	using kCura.PDB.Data.Repositories;
	using kCura.PDB.Data.Repositories.Testing;
	using kCura.PDB.Data.Testing;
	using kCura.PDB.Data.Tests.Repositories;

	using NUnit.Framework;

	[TestFixture]
	[Category("UnitPlatform")]
	[Category("Integration")]
	public class IntegrationTestNewHourRepositoryTests
	{
		private IntegrationTestNewHourRepository repository;
		private HourTestDataRepository hourTestDataRepository;

		private MockHour testHour;
		private Hour nonTestHour;

		[OneTimeSetUp]
		public async Task Setup()
		{
			var hourRepository = new HourRepository(ConnectionFactorySetup.ConnectionFactory);
			this.repository = new IntegrationTestNewHourRepository(ConnectionFactorySetup.ConnectionFactory, hourRepository);
			this.hourTestDataRepository = new HourTestDataRepository(ConnectionFactorySetup.ConnectionFactory, hourRepository);

			this.testHour = new MockHour { HourTimeStamp = DateTime.UtcNow.NormilizeToHour() };
			await this.hourTestDataRepository.CreateAsync(new List<MockHour> { this.testHour });
			var hourToCreate = new Hour { HourTimeStamp = DateTime.UtcNow.NormilizeToHour().AddHours(-4) };
			this.nonTestHour = await hourRepository.CreateAsync(hourToCreate);
		}

		[Test]
		public async Task ReadNextHourWithoutRatings()
		{
			// Act
			var result = await this.repository.ReadNextHourWithoutRatings();

			// Assert
			Assert.That(result.HourTimeStamp, Is.EqualTo(this.testHour.HourTimeStamp));
		}

		[OneTimeTearDown]
		public async Task TearDown()
		{
			await this.hourTestDataRepository.ClearAsync();
		}


	}
}
