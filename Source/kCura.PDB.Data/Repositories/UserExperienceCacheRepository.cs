namespace kCura.PDB.Data.Repositories
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using Dapper;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models.MetricDataSources;
	using kCura.PDB.Data.Properties;

	public class UserExperienceCacheRepository : IUserExperienceCacheRepository
	{
		private readonly IConnectionFactory connectionFactory;

		public UserExperienceCacheRepository(IConnectionFactory connectionFactory)
		{
			this.connectionFactory = connectionFactory;
		}

		public async Task<IList<UserExperience>> ReadAsync(int serverId, DateTime start, DateTime end)
		{
			using (var connection = this.connectionFactory.GetEddsPerformanceConnection())
			{
				return (await connection.QueryAsync<UserExperience>(Resources.UserExperienceCache_ReadByServerIdDateRange,
					new { serverId, start, end })).ToList();
			}
		}

		public async Task<UserExperience> CreateAsync(UserExperience userExperience)
		{
			using (var connection = this.connectionFactory.GetEddsPerformanceConnection())
			{
				var readResult = await connection.QueryFirstOrDefaultAsync<UserExperience>(
					Resources.UserExperienceCache_ReadByHourAndServer,
					new { hourId = userExperience.HourId, serverId = userExperience.ServerId });
				if (readResult == null)
				{
					return await connection.QueryFirstOrDefaultAsync<UserExperience>(Resources.UserExperienceCache_Create, userExperience);
				}
				else
				{
					return readResult;
				}
			}
		}
	}
}
