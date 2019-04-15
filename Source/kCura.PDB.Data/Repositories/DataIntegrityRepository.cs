namespace kCura.PDB.Data.Repositories
{
	using System.Threading.Tasks;
	using Dapper;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;

	public class DataIntegrityRepository : IDataIntegrityRepository
	{
		private readonly IConnectionFactory connectionFactory;

		public DataIntegrityRepository(IConnectionFactory connectionFactory)
		{
			this.connectionFactory = connectionFactory;
		}

		public async Task DropAllTriggersInCurrentDatabase()
		{
			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{
				await conn.ExecuteAsync(Properties.Resources.DropAllTriggers);
			}
		}

		public async Task DropAllTriggersInCurrentDatabase(Server server)
		{
			using (var conn = this.connectionFactory.GetEddsQosConnection(server.ServerName))
			{
				await conn.ExecuteAsync(Properties.Resources.DropAllTriggers);
			}
		}
	}
}
