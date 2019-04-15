namespace kCura.PDB.Data.Repositories
{
	using System.Collections.Generic;
	using System.Linq;
	using Dapper;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models.Audits;
	using kCura.PDB.Data.Properties;

	public class ViewCriteriaRepository : IViewCriteriaRepository
	{
		private readonly IConnectionFactory connectionFactory;

		public ViewCriteriaRepository(IConnectionFactory connectionFactory)
		{
			this.connectionFactory = connectionFactory;
		}

		public IList<ViewCriteria> ReadViewCriteriasForSearch(int workspaceId, int searchArtifactId)
		{
			using (var connection = this.connectionFactory.GetWorkspaceConnection(workspaceId))
			{
				return connection.Query<ViewCriteria>(Resources.ViewCriteria_ReadBySearchId, new { searchArtifactId }).ToList();
			}
		}
	}
}
