namespace kCura.PDB.Data.Testing
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;

	using Dapper;

	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Interfaces.Testing.Repositories;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Data.Properties;
	using kCura.PDB.Data.Repositories;

	public class ReportCleanupRepository : IReportCleanupRepository
	{
		private readonly IConnectionFactory connectionFactory;

		public ReportCleanupRepository(IConnectionFactory connectionFactory)
		{
			this.connectionFactory = connectionFactory;
		}

		public async Task ClearReportData(IList<DateTime> summaryDayHour)
		{
			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{
				await conn.ExecuteAsync(Resources.BackupDbccReportData_Clear, new { summaryDayHour });
			}
		}

		public async Task ClearExistingBackupHistory()
		{
			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{
				await conn.ExecuteAsync(Resources.BackupHistory_Clear);
			}
		}

		public async Task ClearExistingDbccHistory()
		{
			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{
				await conn.ExecuteAsync(Resources.DbccHistory_Clear);
			}
		}
	}
}
