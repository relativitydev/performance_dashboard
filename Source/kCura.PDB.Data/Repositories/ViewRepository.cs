namespace kCura.PDB.Data.Repositories
{
	using Dapper;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models.Audits;
	using kCura.PDB.Data.Properties;

	public class ViewRepository : IViewRepository
	{
		private readonly IConnectionFactory connectionFactory;

		public ViewRepository(IConnectionFactory connectionFactory)
		{
			this.connectionFactory = connectionFactory;
		}

		public Search ReadById(int workspaceId, int searchArtifactId)
		{
			using (var connection = this.connectionFactory.GetWorkspaceConnection(workspaceId))
			{
				return connection.QueryFirstOrDefault<Search>(Resources.View_ReadBySearchId, new { searchArtifactId });
			}
		}
	}
}
