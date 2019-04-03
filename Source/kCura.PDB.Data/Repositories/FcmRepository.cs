namespace kCura.PDB.Data.Repositories
{
	using System.Threading.Tasks;
	using Dapper;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Data.Properties;

	public class FcmRepository : IFcmRepository
	{
		public FcmRepository(IConnectionFactory connectionFactory)
		{
			this.connectionFactory = connectionFactory;
		}

		private readonly IConnectionFactory connectionFactory;

		public async Task ValidatePreBuildAndRateSample(int hourId, bool enableLogging)
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				await conn.ExecuteAsync(Resources.FCM_ValidatePreBARS, new { hourId, logging = enableLogging });
			}
		}

		public async Task ApplySecondaryHashes()
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				await conn.ExecuteAsync(Resources.FCM_ApplySecondaryHashes);
			}
		}
	}
}
