namespace kCura.PDB.Data.Repositories.Testing
{
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

	public class IntegrationTestBackupProvider : IBackupProvider
	{
		private readonly IConnectionFactory connectionFactory;

		public IntegrationTestBackupProvider(IConnectionFactory connectionFactory)
		{
			this.connectionFactory = connectionFactory;
		}

		public async Task<IList<Backup>> GetBackupsAsync(Hour hour, Server server, IList<Database> databases)
		{
			var databaseNames = databases.Select(d => d.Name).ToList();
			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{
				return await conn.QueryAsync<Backup>(
					        Resources.BackupTest_ReadForHour,
					        new
						        {
							        server.ServerName,
							        hourStartDate = hour.HourTimeStamp,
							        hourEndDate = hour.GetNextHour(),
							        databaseNames
						        }).ToListAsync();
			}
		}

		public async Task<IList<Backup>> GetLastBackupsBeforeHourAsync(Hour hour, Server server, IList<Database> databases, IList<BackupType> backupTypes)
		{
			var databaseNames = databases.Select(d => d.Name).ToList();
			var types = backupTypes.Select(t => (char)t);
			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{
				return await conn.QueryAsync<Backup>(
					       Resources.BackupTest_ReadFirstBeforeHour,
					       new
						       {
							       server.ServerName,
							       hourEndDate = hour.HourTimeStamp,
								   databaseNames,
								   backupTypes = types
						       }).ToListAsync();
			}
		}
	}
}
