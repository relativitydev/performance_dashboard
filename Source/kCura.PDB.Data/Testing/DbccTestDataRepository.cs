namespace kCura.PDB.Data.Testing
{
	using System.Collections.Generic;
	using System.Threading.Tasks;

	using Dapper;

	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Interfaces.Testing.Repositories;
	using kCura.PDB.Core.Models.Testing;
	using kCura.PDB.Data.Properties;

	public class DbccTestDataRepository : IDbccTestDataRepository
	{
		private readonly IConnectionFactory connectionFactory;

		public DbccTestDataRepository(IConnectionFactory connectionFactory)
		{
			this.connectionFactory = connectionFactory;
		}

		public async Task CreateAsync(IList<MockDbccServerResults> mockData)
		{
			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{
				await conn.ExecuteAsync(Resources.MockDbcc_Create, mockData);
			}
		}

		public async Task ClearAsync()
		{
			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{
				await conn.ExecuteAsync(Resources.MockDbcc_Clear);
			}
		}
	}
}
