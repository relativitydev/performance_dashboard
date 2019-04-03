namespace kCura.PDB.Data.Tests.Repositories
{
	using System.Threading.Tasks;
	using kCura.PDB.Data.Repositories;
	using NUnit.Framework;
	using PDB.Tests.Common;

	[TestFixture]
	[Category("UnitPlatform")]
	[Category("Integration")] // For TeamCity to ignore this test
	public class BackfillRepositoryTests
	{
		private BackfillRepository backfillRepository;

		[OneTimeSetUp]
		public void SetUp()
		{
			this.backfillRepository = new BackfillRepository(ConnectionFactorySetup.ConnectionFactory);
		}

		[Test]
		public async Task Backfill_ReadHoursAwaitingDiscovery()
		{
			// Act
			var result = await this.backfillRepository.ReadHoursAwaitingDiscovery();

			// Assert
			Assert.Pass($"result can be any value. {result}");
		}

		[Test]
		public async Task Backfill_ReadHoursAwaitingAnalysis()
		{
			// Act
			var result = await this.backfillRepository.ReadHoursAwaitingAnalysis();

			// Assert
			Assert.Pass($"result can be any value. {result}");
		}

		[Test]
		public async Task Backfill_ReadHoursAwaitingScoring()
		{
			// Act
			var result = await this.backfillRepository.ReadHoursAwaitingScoring();

			// Assert
			Assert.Pass($"result can be any value. {result}");
		}

		[Test]
		public async Task Backfill_ReadHoursCompletedScoring()
		{
			// Act
			var result = await this.backfillRepository.ReadHoursCompletedScoring();

			// Assert
			Assert.Pass($"result can be any value. {result}");
		}
	}
}
