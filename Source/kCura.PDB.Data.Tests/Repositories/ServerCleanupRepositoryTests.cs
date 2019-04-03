namespace kCura.PDB.Data.Tests.Repositories
{
	using System.Threading.Tasks;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Data.Repositories;
	using kCura.PDB.Data.Services;
	using kCura.PDB.Service.Services;
	using NUnit.Framework;
	using PDB.Tests.Common;

	[TestFixture, Category("Integration")]
	public class ServerCleanupRepositoryTests
	{
		private ServerCleanupRepository serverCleanupRepository;
		private ServerCleanup testServerCleanup;
		private ServerCleanup testServerCleanupResult;

		[SetUp]
		public async Task SetUp()
		{
			this.serverCleanupRepository =
				new ServerCleanupRepository(TestUtilities.GetIntegrationConnectionFactory());

			this.testServerCleanup = new ServerCleanup
			{
				HourId = 0,
				ServerId = 0,
				Success = true
			};
			this.testServerCleanupResult = await this.serverCleanupRepository.CreateAsync(testServerCleanup);
		}

		[Test]
		public async Task ReadAsync()
		{
			// Arrange
			// Act
			var result = await this.serverCleanupRepository.ReadAsync(testServerCleanupResult.Id);

			// Assert
			Assert.That(this.testServerCleanupResult, Is.Not.Null);
			Assert.That(result.Id, Is.EqualTo(testServerCleanupResult.Id));
			Assert.That(result.HourId, Is.EqualTo(testServerCleanupResult.HourId));
			Assert.That(result.ServerId, Is.EqualTo(testServerCleanupResult.ServerId));
			Assert.That(result.Success, Is.EqualTo(testServerCleanupResult.Success));
		}

		[Test]
		public void CreateAsync()
		{
			// Arrange
			// Act
			// Assert
			Assert.That(testServerCleanupResult, Is.Not.Null);
			Assert.That(testServerCleanupResult.HourId, Is.EqualTo(testServerCleanup.HourId));
			Assert.That(testServerCleanupResult.ServerId, Is.EqualTo(testServerCleanup.ServerId));
			Assert.That(testServerCleanupResult.Success, Is.EqualTo(testServerCleanup.Success));
		}

		[Test]
		public async Task UpdateAsync()
		{
			// Arrange
			var testServerCleanupUpdateResult = new ServerCleanup
			{
				Id = testServerCleanupResult.Id,
				HourId = testServerCleanupResult.HourId,
				ServerId = testServerCleanupResult.ServerId,
				Success = !testServerCleanupResult.Success
			};
			
			// Act
			await this.serverCleanupRepository.UpdateAsync(testServerCleanupUpdateResult);

			// Assert
			var result = await this.serverCleanupRepository.ReadAsync(testServerCleanupUpdateResult.Id);
			Assert.That(result, Is.Not.Null);
			Assert.That(result.HourId, Is.EqualTo(testServerCleanupUpdateResult.HourId));
			Assert.That(result.ServerId, Is.EqualTo(testServerCleanupUpdateResult.ServerId));
			Assert.That(result.Success, Is.EqualTo(testServerCleanupUpdateResult.Success));
		}
	}
}
