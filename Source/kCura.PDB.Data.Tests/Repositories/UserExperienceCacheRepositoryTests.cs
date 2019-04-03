namespace kCura.PDB.Data.Tests.Repositories
{
	using System;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Models.MetricDataSources;
	using kCura.PDB.Data.Repositories;
	using NUnit.Framework;
	using PDB.Tests.Common;

	[TestFixture, Category("Integration")]
	public class UserExperienceCacheRepositoryTests
	{
		private UserExperienceCacheRepository userExperienceCacheRepository;

		[SetUp]
		public void Setup()
		{
			this.userExperienceCacheRepository = new UserExperienceCacheRepository(TestUtilities.GetIntegrationConnectionFactory());
		}

		[Test, Explicit("TODO"), Category("Explicit")]
		public async Task CreateAsync()
		{
			// Arrange
			var userExperience = new UserExperience
			{
				ActiveUsers = 123,
				ArrivalRate = 0.1337m,
				Concurrency = 0.1337m,
				HasPoisonWaits = false,
				HourId = 37715,
				ServerId = 51773
			};

			// Act
			var result = await this.userExperienceCacheRepository.CreateAsync(userExperience);

			// Assert
		}

		[Test, Explicit("TODO"), Category("Explicit")]
		public async Task ReadAsync()
		{
			// Arrange
			var serverId = 4;
			var start = DateTime.UtcNow.AddDays(-7);
			var end = DateTime.UtcNow;

			// Act
			var result = await this.userExperienceCacheRepository.ReadAsync(serverId, start, end);

			// Assert
		}
	}
}
