namespace kCura.PDB.Data.Repositories
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using Dapper;

	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.RecoverabilityIntegrity;
	using kCura.PDB.Data.Properties;

	public class DatabaseGapsRepository : IDatabaseGapsRepository
	{
		public DatabaseGapsRepository(IConnectionFactory connectionFactory)
		{
			this.connectionFactory = connectionFactory;
		}

		private readonly IConnectionFactory connectionFactory;

		/// <inheritdoc />
		public async Task CreateDatabaseGapsAsync<TGap>(IList<TGap> gaps)
			where TGap : Gap
		{
			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{
				foreach (var gap in gaps)
				{
					var gapExists = await conn.QueryFirstOrDefaultAsync<Gap>(Resources.DatabaseGap_Exists, gap)
						.ConfigureAwait(false);
					if (gapExists == null)
					{
						await conn.ExecuteAsync(Resources.DatabaseGap_Create, gap).ConfigureAwait(false);
					}
				}
			}
		}

		/// <inheritdoc />
		public async Task<TGap> ReadLargestGapsForHourAsync<TGap>(Server server, Hour hour, GapActivityType activityType)
		{
			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{
				return await conn.QueryFirstOrDefaultAsync<TGap>(
						   Resources.DatabaseGap_ReadLargestGapsForHour,
						   new
						   {
							   server.ServerId,
							   hourTimeStampStart = hour.HourTimeStamp,
							   hourTimeStampEnd = hour.GetHourEnd(),
							   activityType
						   }).ConfigureAwait(false);
			}
		}

		/// <inheritdoc />
		public async Task<IList<TGap>> ReadGapsLargerThanForHourAsync<TGap>(Server server, Hour hour, GapActivityType activityType, int minDuration)
		{
			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{
				return (await conn.QueryAsync<TGap>(
							Resources.DatabaseGap_ReadGapsLargerThanForHour,
							new
							{
								server.ServerId,
								hourTimeStampStart = hour.HourTimeStamp,
								hourTimeStampEnd = hour.GetHourEnd(),
								activityType,
								minDuration
							}).ConfigureAwait(false)).ToList();
			}
		}

		/// <inheritdoc />
		public async Task<IList<TGap>> ReadLargestGapsForEachDatabaseAsync<TGap>(Server server, Hour hour, GapActivityType activityType)
		{
			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{
				return (await conn.QueryAsync<TGap>(
							Resources.DatabaseGap_ReadLargestGapsForHourAllDatabase,
							new
							{
								server.ServerId,
								hourTimeStampStart = hour.HourTimeStamp,
								hourTimeStampEnd = hour.GetHourEnd(),
								activityType
							}).ConfigureAwait(false)).ToList();
			}
		}
	}
}
