namespace kCura.PDB.Data.Repositories
{
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using Dapper;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Data.Properties;

	public class MetricManagerStatsRepository : IMetricManagerStatsRepository
	{
		private readonly IConnectionFactory connectionFactory;

		public MetricManagerStatsRepository(IConnectionFactory connectionFactory)
		{
			this.connectionFactory = connectionFactory;
		}

		public async Task CreateAsync(IList<MetricManagerExecutionStat> stat)
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				await conn.ExecuteAsync(Resources.MetricManagerExecutionStat_Create, stat);
			}
		}
	}
}
