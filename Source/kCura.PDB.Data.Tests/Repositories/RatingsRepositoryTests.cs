namespace kCura.PDB.Data.Tests.Repositories
{
	using System;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Data.Repositories;
	using NUnit.Framework;
	using PDB.Tests.Common;

	[TestFixture]
	[Category("UnitPlatform")]
	[Category("Integration")] // For TeamCity to ignore this test
	public class RatingsRepositoryTests
	{
		[SetUp]
		public void SetUp()
		{
			hourRepository = new HourRepository(ConnectionFactorySetup.ConnectionFactory);
			ratingsRepository = new RatingsRepository(ConnectionFactorySetup.ConnectionFactory);
		}

		private RatingsRepository ratingsRepository;
		private HourRepository hourRepository;

		[Test]
		public async Task Exists_NewHour()
		{
			// Arrange
			var hour = await hourRepository.CreateAsync(new Hour
			{
				HourTimeStamp = DateTime.UtcNow.AddYears(-100),
				Score = 110.0m,
				InSample = true,
			});

			// Act
			var result = await this.ratingsRepository.Exists(hour.Id);

			// Assert
			Assert.That(result, Is.False); // Newly created hour should not have QoSRating entry
		}
	}
}
