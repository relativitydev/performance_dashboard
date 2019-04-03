namespace kCura.PDB.Data.Repositories.Testing
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;

	using Dapper;

	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.DataProviders;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.RecoverabilityIntegrity;
	using kCura.PDB.Data.Properties;

	public class IntegrationTestDbccProvider : IDbccProvider
	{
		private readonly IConnectionFactory connectionFactory;

		public IntegrationTestDbccProvider(IConnectionFactory connectionFactory)
		{
			this.connectionFactory = connectionFactory;
		}

		public async Task<IList<Dbcc>> GetDbccsAsync(Server server, IList<Database> databases)
		{
			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{
				return await databases.Select(async db =>
									 {
										 var lastDbccDateTime = await conn.QueryFirstOrDefaultAsync<DateTime>(
												 Resources.DbccTest_ReadLast,
												 new { server = server.ServerName, database = db.Name });
										 return new Dbcc { DatabaseId = db.Id, End = lastDbccDateTime };
									 })
					.WhenAllStreamed(1);
			}
		}
	}
}
