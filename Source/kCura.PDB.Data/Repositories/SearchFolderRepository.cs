namespace kCura.PDB.Data.Repositories
{
	using System;
	using Dapper;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Data.Properties;

	public class SearchFolderRepository : ISearchFolderRepository
	{
		private readonly IConnectionFactory connectionFactory;

		public SearchFolderRepository(IConnectionFactory connectionFactory)
		{
			this.connectionFactory = connectionFactory;
		}

		public int GetSearchFolderCountForSearch(int workspaceId, int searchArtifactId)
		{
			using (var connection = this.connectionFactory.GetWorkspaceConnection(workspaceId))
			{
				return connection.ExecuteScalar<int>(Resources.SearchFolder_ReadTotalForSearchId, new { searchArtifactId });
			}
		}
	}
}
