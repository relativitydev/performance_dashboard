namespace kCura.PDB.Data.Repositories
{
	using System;
	using System.Data.SqlClient;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Data.Services;

	public abstract class BaseDbRepository : IDbRepository
	{
		protected readonly IConnectionFactory connectionFactory;
		protected BaseDbRepository(IConnectionFactory connectionFactory)
		{
			this.connectionFactory = connectionFactory;
		}
		
		public int? GetWorkspaceServerId(int caseArtifactId)
		{
			throw new System.NotImplementedException();
		}

		public void SetTimezoneOffset(int offset)
		{
			throw new System.NotImplementedException();
		}
	}
}
