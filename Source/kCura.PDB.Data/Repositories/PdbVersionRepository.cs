namespace kCura.PDB.Data.Repositories
{
	using System;
	using System.Threading.Tasks;

	using Dapper;

	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Data.Properties;

	/// <summary>
	/// Uses Table in EDDS to handle EDDSPerformance Versioning
	/// </summary>
	public class PdbVersionRepository : IPdbVersionRepository
	{
		private readonly IConnectionFactory connectionFactory;

		public PdbVersionRepository(IConnectionFactory connectionFactory)
		{
			this.connectionFactory = connectionFactory;
		}

		public async Task<Version> GetLatestVersionAsync()
		{
			using (var conn = this.connectionFactory.GetEddsConnection())
			{
				try
				{
					var dynamicVersion = await conn.QueryFirstOrDefaultAsync<dynamic>(Resources.EddsPerformanceVersion_Read);
					return new Version(dynamicVersion.Major, dynamicVersion.Minor, dynamicVersion.Build, dynamicVersion.Revision);
				}
				catch (Exception)
				{
					return null; // Assume table doesn't exist yet, so there's no version
				}
			}
		}

		public async Task SetLatestVersionAsync(Version version)
		{
			using (var conn = this.connectionFactory.GetEddsConnection())
			{
				await conn.ExecuteAsync(Resources.EddsPerformanceVersion_Insert, new { major = version.Major, minor = version.Minor, build = version.Build, revision = version.Revision });
			}
		}

		public async Task InitializeIfNotExists()
		{
			using (var conn = this.connectionFactory.GetEddsConnection())
			{
				await conn.ExecuteAsync(Resources.EddsPerformanceVersion_Create);
			}
		}
	}
}
