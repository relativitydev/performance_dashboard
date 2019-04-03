namespace kCura.PDB.Data.Tests.Repositories
{
	using System;
	using System.Threading.Tasks;

	using kCura.PDB.Data.Repositories;
	using kCura.PDB.Tests.Common;

	using NUnit.Framework;

	[TestFixture]
	[Category("Integration")]
	public class PdbVersionRepositoryTests
	{
		private PdbVersionRepository pdbVersionRepository;

		[SetUp]
		public void Setup()
		{
			this.pdbVersionRepository = new PdbVersionRepository(TestUtilities.GetIntegrationConnectionFactory());
		}

		[Test]
		public async Task InitializeIfNeeded()
		{
			await this.pdbVersionRepository.InitializeIfNotExists();
		}

		[Test]
		public async Task GetLatestVersionAsync()
		{
			var version = await this.pdbVersionRepository.GetLatestVersionAsync();
		}

		[Test]
		public async Task SetLatestVersionAsync()
		{
			var testVersion = new Version("0.0.0.0");
			await this.pdbVersionRepository.SetLatestVersionAsync(testVersion);
		}
	}
}
