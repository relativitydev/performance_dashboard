namespace kCura.PDB.Data.Tests.Repositories
{
	using System.Threading.Tasks;
	using kCura.PDB.Data.Repositories;
	using NUnit.Framework;
	using PDB.Tests.Common;

	[TestFixture]
	[Category("UnitPlatform")]
	[Category("Integration")] // For TeamCity to ignore this test
	public class UserExperienceRatingsRepositoryTests
	{
		[OneTimeSetUp]
		public void OneTimeSetUp()
		{
			this.userExperienceRatingsRepository = new UserExperienceRatingsRepository(ConnectionFactorySetup.ConnectionFactory);
			
		}

		private UserExperienceRatingsRepository userExperienceRatingsRepository;

		[Test]
		public async Task Create()
		{
			// Arrange
			var testServerArtifactId = 0;
			var testArrivalRate = 66.6m;
			var testConcurrency = 66.6m;
			var testHourId = 1;

			// Act
			await this.userExperienceRatingsRepository.CreateAsync(testServerArtifactId, testArrivalRate, testConcurrency, testHourId);

			// Assert
			Assert.Pass("Not return result");
		}

	}
}
