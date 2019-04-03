namespace kCura.PDB.Data.Repositories
{
	using System.Threading.Tasks;
	using kCura.PDB.Core.Interfaces.Repositories;
	using Core.Models;
	using Dapper;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Data.Properties;

	public class PrimarySqlServerRepository : BaseDbRepository, IPrimarySqlServerRepository
	{
		public PrimarySqlServerRepository(IConnectionFactory connectionFactory) : base(connectionFactory)
		{
		}

		public async Task<ResourceServer> GetPrimarySqlServerAsync()
		{
			using (var conn = connectionFactory.GetEddsConnection())
			{
				return await conn.QueryFirstOrDefaultAsync<ResourceServer>(Resources.PrimarySqlServer_Read);
			}
		}

		public ResourceServer GetPrimarySqlServer()
		{
			using (var conn = connectionFactory.GetEddsConnection())
			{
				return conn.QueryFirstOrDefault<ResourceServer>(Resources.PrimarySqlServer_Read);
			}
		}
    }
}
