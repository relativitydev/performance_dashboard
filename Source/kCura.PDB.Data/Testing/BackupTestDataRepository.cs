namespace kCura.PDB.Data.Testing
{
	using System.Collections.Generic;
	using System.Threading.Tasks;

	using Dapper;

	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Interfaces.Testing.Repositories;
	using kCura.PDB.Core.Models.Testing;
	using kCura.PDB.Data.Properties;

	public class BackupTestDataRepository : IBackupTestDataRepository
	{
		private readonly IConnectionFactory connectionFactory;

		public BackupTestDataRepository(IConnectionFactory connectionFactory)
		{
			this.connectionFactory = connectionFactory;
		}

		public async Task CreateAsync(IList<MockBackupSet> backups)
		{
			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{
				await conn.ExecuteAsync(Resources.MockBackup_Create, backups);
			}
		}

		public async Task ClearAsync()
		{
			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{
				await conn.ExecuteAsync(Resources.MockBackup_Clear);
			}
		}
	}
}
