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
	public class FcmRepositoryTests
	{
		[SetUp]
		public async Task SetUp()
		{
			var connectionFactory = TestUtilities.GetIntegrationConnectionFactory();
			this.fcmRepository = new FcmRepository(connectionFactory);
			this.hourRepository = new HourRepository(connectionFactory);
			hour = await this.hourRepository.ReadCurrentAsync() ?? await this.hourRepository.ReadLastAsync();
		}

		private FcmRepository fcmRepository;
		private HourRepository hourRepository;
		private Hour hour;

		[Test]
		public async Task ValidatePreBuildAndRateSample()
		{
			// Arrange
			// Act
			await this.fcmRepository.ValidatePreBuildAndRateSample(hour.Id, true);

			// Assert
			// TODO?
		}

		[Test]
		public async Task ApplySecondaryHashes()
		{
			// Arrange
			// Act
			await this.fcmRepository.ApplySecondaryHashes();

			// Assert
			// TODO?
		}
	}
}
