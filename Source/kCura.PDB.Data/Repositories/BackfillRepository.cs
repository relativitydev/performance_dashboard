namespace kCura.PDB.Data.Repositories
{
	using Core.Interfaces.Repositories;
	using Core.Interfaces.Services;
	using Core.Models;
	using System.Threading.Tasks;
	using Dapper;
	using Core.Constants;
	using Properties;

	public class BackfillRepository : BaseDbRepository, IBackfillRepository
	{
		public BackfillRepository(IConnectionFactory connectionFactory) : base (connectionFactory)
		{
		}

		public async Task<int> ReadHoursAwaitingDiscovery()
		{
			using (var connection = this.connectionFactory.GetEddsPerformanceConnection())
			{
				return await connection.QueryFirstOrDefaultAsync<int>(Resources.Backfill_ReadHoursAwaitingDiscovery, new { backFillHours = Defaults.BackfillDays });
			}
		}

		public async Task<int> ReadHoursAwaitingAnalysis()
		{
			using (var connection = this.connectionFactory.GetEddsPerformanceConnection())
			{
				return (await connection.QueryFirstOrDefaultAsync<int?>(Resources.Backfill_ReadHoursAwaitingAnalysis, new { backFillHours = Defaults.BackfillDays })) ?? 0;
			}
		}

		public async Task<int> ReadHoursAwaitingScoring()
		{
			using (var connection = this.connectionFactory.GetEddsPerformanceConnection())
			{
				return (await connection.QueryFirstOrDefaultAsync<int?>(Resources.Backfill_ReadHoursAwaitingScoring, new { backFillHours = Defaults.BackfillDays })) ?? 0;
			}
		}

		public async Task<int> ReadHoursCompletedScoring()
		{
			using (var connection = this.connectionFactory.GetEddsPerformanceConnection())
			{
				return await connection.QueryFirstOrDefaultAsync<int>(Resources.Backfill_ReadHoursCompletedScoring, new { backFillHours = Defaults.BackfillDays });
			}
		}
	}
}
