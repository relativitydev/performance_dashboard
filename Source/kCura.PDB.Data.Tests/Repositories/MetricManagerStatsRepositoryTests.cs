namespace kCura.PDB.Data.Tests.Repositories
{
	using System;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Data.Repositories;
	using NUnit.Framework;

	[TestFixture]
	[Category("UnitPlatform")]
	[Category("Integration")] // For TeamCity to ignore this test
	public class MetricManagerStatsRepositoryTests
	{
		[SetUp]
		public void Setup()
		{
			this.metricManagerStatsRepository = new MetricManagerStatsRepository(ConnectionFactorySetup.ConnectionFactory);
		}
		
		private MetricManagerStatsRepository metricManagerStatsRepository;

		[Test]
		public async Task MetricManagerStatsRepository_CreateAsync()
		{
			var stats = new[]
			{
				new MetricManagerExecutionStat
				{
					Start = DateTime.UtcNow,
					End = DateTime.UtcNow,
					Name = "Test stat 1",
					Count = 1234,
					ExecutionId = Guid.NewGuid(),
					MaxTime = 123.4567,
					TotalTime = 123456.789012
				},
				new MetricManagerExecutionStat
				{
					Start = DateTime.UtcNow,
					End = DateTime.UtcNow,
					Name = "Test stat 2",
					Count = 4321,
					ExecutionId = Guid.NewGuid(),
					MaxTime = 987.4567,
					TotalTime = 0.789012
				}
			};
			await this.metricManagerStatsRepository.CreateAsync(stats);
		}
	}
}
