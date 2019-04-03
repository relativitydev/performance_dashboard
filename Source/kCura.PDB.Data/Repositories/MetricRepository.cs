namespace kCura.PDB.Data.Repositories
{
	using System.Threading.Tasks;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using Dapper;
	using kCura.PDB.Data.Properties;

	public class MetricRepository : IMetricRepository
	{
		public MetricRepository(IConnectionFactory connectionFactory)
		{
			this.connectionFactory = connectionFactory;
		}

		private readonly IConnectionFactory connectionFactory;

		public async Task<Metric> CreateAsync(Metric metric)
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				var readResult = await conn.QueryFirstOrDefaultAsync<Metric>(Resources.Metric_CreateRead, metric);
				if (readResult == null)
					await conn.ExecuteAsync(Resources.Metric_CreateInsert, metric);
				else
					return readResult;
				return await conn.QueryFirstOrDefaultAsync<Metric>(Resources.Metric_CreateRead, metric);
			}
		}

		public async Task<Metric> ReadAsync(int id)
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				return await conn.QueryFirstOrDefaultAsync<Metric>(Resources.Metric_ReadByID, new { id });
			}
		}

		public async Task UpdateAsync(Metric metric)
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				await conn.ExecuteAsync(Resources.Metric_Update, metric);
			}
		}

		public async Task DeleteAsync(Metric metric)
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				await conn.ExecuteAsync(Resources.Metric_Delete, metric);
			}
		}
	}
}
