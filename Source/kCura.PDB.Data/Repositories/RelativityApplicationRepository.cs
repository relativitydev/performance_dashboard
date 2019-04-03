namespace kCura.PDB.Data.Repositories
{
	using System;
	using Dapper;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Data.Properties;

	public class RelativityApplicationRepository : IRelativityApplicationRepository
	{
		private readonly IConnectionFactory connectionFactory;

		public RelativityApplicationRepository(IConnectionFactory connectionFactory)
		{
			this.connectionFactory = connectionFactory;
		}

		public bool ApplicationIsInstalledOnEnvironment(Guid applicationGuid)
		{
			using (var connection = this.connectionFactory.GetEddsConnection())
			{
				return connection.ExecuteScalar<bool>(Resources.RelativityApplication_ApplicationIsInstalledOnEnvironment, new {applicationGuid});
			}
		}

		public string GetApplicationVersion(int workspaceId, Guid applicationGuid)
		{
			using (var connection = this.connectionFactory.GetEddsConnection())
			{
				return connection.QueryFirstOrDefault<string>(Resources.RelativityApplication_ApplicationVersionOnWorkspace, new
				{
					caseArtifactId = workspaceId,
					applicationGuid
				});
			}
		}
	}
}
