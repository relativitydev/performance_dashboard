namespace kCura.PDB.Data.Repositories
{
	using System;
	using System.Threading.Tasks;
	using Core.Interfaces.Repositories;
	using Core.Interfaces.Services;
	using Dapper;
	using Properties;

	public class UptimeRatingsRepository : IUptimeRatingsRepository
	{
		public UptimeRatingsRepository(IConnectionFactory connectionFactory)
		{
			this.connectionFactory = connectionFactory;
		}

		private readonly IConnectionFactory connectionFactory;

		public async Task Create(decimal hoursDown, DateTime summaryDayHour, bool isWebDowntime, bool affectedByMaintenanceWindow)
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
                // Read first to see if there already is a value for the hour
				var uptimeRatingsExists = await conn.QueryFirstOrDefaultAsync<int>(Resources.UptimeRatings_ReadBySummaryDayHour, new { summaryDayHour });
				if (uptimeRatingsExists == 0)
				{
                    // insert the values into the table if there is not already values for that hour
					await conn.ExecuteAsync(Resources.UptimeRatings_Create, new { hoursDown, summaryDayHour, isWebDowntime, affectedByMaintenanceWindow } );
				}
			}
		}

		public async Task UpdateWeeklyScores()
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				await conn.ExecuteAsync(Resources.UptimeRatings_UpdateWeeklyScores);
			}
		}

		public async Task UpdateQuartlyScores(DateTime summaryDayHour)
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				await conn.ExecuteAsync(Resources.UptimeRatings_UpdateQuartlyScores, new { summaryDayHour });
			}
		}
	}
}
