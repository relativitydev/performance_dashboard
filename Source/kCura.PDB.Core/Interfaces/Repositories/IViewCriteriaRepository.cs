namespace kCura.PDB.Core.Interfaces.Repositories
{
	using System.Collections.Generic;
	using kCura.PDB.Core.Models.Audits;

	/// <summary>
	/// Repository that interacts with the ViewCriteria table in Relativity
	/// </summary>
	public interface IViewCriteriaRepository
	{
		/// <summary>
		/// Reads the ViewCriteria for a given search
		/// </summary>
		/// <param name="workspaceId">The ArtifactId for the workspace that contains the search</param>
		/// <param name="searchArtifactId">The ArtifactId for the search</param>
		/// <returns>A list of ViewCriteria for a given search in a workspace</returns>
		IList<ViewCriteria> ReadViewCriteriasForSearch(int workspaceId, int searchArtifactId);
	}
}