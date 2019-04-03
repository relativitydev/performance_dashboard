namespace kCura.PDB.Data.Repositories
{
	using System;
	using System.Threading.Tasks;
	using Dapper;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models.Audits;
	using kCura.PDB.Data.Properties;

	public class UserExperienceRatingsRepository : IUserExperienceRatingsRepository
	{
		public UserExperienceRatingsRepository(IConnectionFactory connectionFactory)
		{
			this.connectionFactory = connectionFactory;
		}

		private readonly IConnectionFactory connectionFactory;

		public async Task CreateAsync(int serverArtifactId, decimal arrivalRateUXScore, decimal concurrencyUXScore, int hourId)
		{
			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{
				var readResult = await conn.QueryFirstOrDefaultAsync<UserExperienceRating>(Resources.UserExperienceRating_ReadByHourAndServer,
						new { serverArtifactId, hourId });
				if (readResult == null)
				{
					await conn.ExecuteAsync(Resources.UserExperienceRating_Create, new {serverArtifactId, arrivalRateUXScore, concurrencyUXScore, hourId});
				}
			}
		}
	}
}
