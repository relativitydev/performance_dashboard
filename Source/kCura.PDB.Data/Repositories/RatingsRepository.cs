namespace kCura.PDB.Data.Repositories
{
	using System.Threading.Tasks;
	using Dapper;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Data.Properties;

	public class RatingsRepository : IRatingsRepository
	{
		public RatingsRepository(IConnectionFactory connectionFactory)
		{
			this.connectionFactory = connectionFactory;
		}

		private readonly IConnectionFactory connectionFactory;

		public async Task<bool> Exists(int hourId)
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				return await conn.QueryFirstAsync<bool>(Resources.QoSRatings_Exist, new { hourId });
			}
		}
	}
}
