namespace kCura.PDB.Data.Repositories.Testing
{
	using System.Collections.Generic;
	using System.Threading.Tasks;

	using Dapper;

	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Data.Properties;

	public class IntegrationTestNewDatabaseRepository : DatabaseRepository
	{
		private readonly IConnectionFactory connectionFactory;

		public IntegrationTestNewDatabaseRepository(IConnectionFactory connectionFactory)
			: base(connectionFactory)
		{
			this.connectionFactory = connectionFactory;
		}

		public override async Task<IList<string>> GetByServerAsync(Server server)
		{
			// Read from mock data instead of actual sql server
			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{
				return await conn.QueryAsync<string>(Resources.DatabaseTest_ReadAllByServer, new { server.ServerName })
					       .ToListAsync();
			}
		}
	}
}
