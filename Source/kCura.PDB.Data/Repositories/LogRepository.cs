namespace kCura.PDB.Data.Repositories
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Linq;
	using System.Threading.Tasks;
	using Core.Interfaces.Repositories;
	using Core.Interfaces.Services;
	using Core.Models;
	using Dapper;
	using kCura.PDB.Data.Properties;

	public class LogRepository : BaseDbRepository, ILogRepository
	{
		public LogRepository(IConnectionFactory connectionFactory) : base(connectionFactory)
		{
		}

		public int Create(LogEntry logEntry)
		{
			logEntry.LogTimestampUtc = logEntry.LogTimestampUtc == DateTime.MinValue ? DateTime.UtcNow : logEntry.LogTimestampUtc;

			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{
				return conn.QueryFirstOrDefault<int>(Resources.GlassRunLog_Insert, logEntry, commandType: CommandType.Text);
			}
		}

		public async Task<int> CreateAsync(LogEntry logEntry)
		{
			logEntry.LogTimestampUtc = logEntry.LogTimestampUtc == DateTime.MinValue ? DateTime.UtcNow : logEntry.LogTimestampUtc;

			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{
				return await conn.QueryFirstOrDefaultAsync<int>(Resources.GlassRunLog_Insert, logEntry, commandType: CommandType.Text);
			}
		}

		public async Task<LogEntry> ReadLastAsync()
		{
			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{
				return await conn.QueryFirstOrDefaultAsync<LogEntry>(Resources.GlassRunLog_ReadLast);
			}
		}

		public async Task<IList<LogEntryFull>> ReadLastAsync(int count, int logLevel)
		{
			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{
				return (logLevel >= 10
					? await conn.QueryAsync<LogEntryFull>(Resources.GlassRunLog_ReadLastFullLogWithEventInfo, new { count })
					: await conn.QueryAsync<LogEntryFull>(Resources.GlassRunLog_ReadLastByLogLevelWithEventInfo, new { count, logLevel }))
					.ToList();
			}
		}

		public IList<LogCategory> ReadCategories()
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				return conn.Query<LogCategory>(Resources.LogCategories_ReadAll).ToList();
			}
		}
	}
}
