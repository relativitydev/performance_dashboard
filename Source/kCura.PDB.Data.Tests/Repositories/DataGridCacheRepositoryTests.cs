namespace kCura.PDB.Data.Tests.Repositories
{
	using System;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Data.Repositories;
	using kCura.PDB.Tests.Common;
	using NUnit.Framework;

	[TestFixture]
	[Category("UnitPlatform")]
	[Category("Integration")] // For TeamCity to ignore this test
	public class DataGridCacheRepositoryTests
	{
		private DataGridCacheRepository dataGridCacheRepository;
		private HourRepository hourRepository;
		private Hour hour;

		[OneTimeSetUp]
		public async Task Setup()
		{
			this.hourRepository = new HourRepository(ConnectionFactorySetup.ConnectionFactory);
			this.hour = await this.hourRepository.CreateAsync(new Hour { HourTimeStamp = DateTime.Now });
			this.dataGridCacheRepository = new DataGridCacheRepository(ConnectionFactorySetup.ConnectionFactory);
		}

		[Test]
		public async Task ReadUseDataGrid()
		{
			var workspaceId = Config.WorkSpaceId;

			var result = await this.dataGridCacheRepository.ReadUseDataGrid(workspaceId, this.hour.Id);

			Console.WriteLine($"Result = {result}");
			Assert.Pass();
		}

		[Test]
		[Description("Warning: Modifies data on environment")]
		public async Task UpdateAndClear()
		{
			var workspaceId = Config.WorkSpaceId;

			await this.dataGridCacheRepository.UpdateDataGridCache(workspaceId, hour.Id);
			var updateResult = await this.dataGridCacheRepository.ReadUseDataGrid(workspaceId, hour.Id);
			await this.dataGridCacheRepository.Clear(workspaceId);
			var clearResult = await this.dataGridCacheRepository.ReadUseDataGrid(workspaceId, hour.Id);

			Assert.That(updateResult, Is.True);
			Assert.That(clearResult, Is.False);
		}
	}
}
