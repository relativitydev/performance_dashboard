namespace kCura.PDB.Data.Tests.Repositories
{
	using System;
	using System.Threading.Tasks;
	using Core.Extensions;
	using Data.Repositories;
	using NUnit.Framework;

	[TestFixture]
	[Category("UnitPlatform")]
	[Category("Integration")] // For TeamCity to ignore this test
	public class UptimeRatingsRepositoryTests
	{
		[OneTimeSetUp]
		public void SetUp()
		{
			this.uptimeRatingsRepository = new UptimeRatingsRepository(ConnectionFactorySetup.ConnectionFactory);
		}

		private UptimeRatingsRepository uptimeRatingsRepository;

		[Test]
		public async Task UptimeRatings_Create()
		{
			// Act
			await this.uptimeRatingsRepository.Create(0.95m, DateTime.Now.NormilizeToHour(), true, false);

			// Assert
			Assert.Pass();
		}

		[Test]
		public async Task UptimeRatings_UpdateWeeklyScores()
		{
			// Act
			await this.uptimeRatingsRepository.UpdateWeeklyScores();

			// Assert
			Assert.Pass();
		}

		[Test]
		public async Task UptimeRatings_UpdateQuartlyScores()
		{
			// Act
			await this.uptimeRatingsRepository.UpdateQuartlyScores(DateTime.Now.NormilizeToHour());

			// Assert
			Assert.Pass();
		}
	}
}
