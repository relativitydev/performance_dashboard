namespace kCura.PDB.Core.Interfaces.Repositories
{
	using kCura.PDB.Core.Models.Audits;

	/// <summary>
	/// Repository that interacts with the View table in Relativity
	/// </summary>
	public interface IViewRepository
	{
		/// <summary>
		/// Reads the SearchText field for a given search (Different from QueryText)
		/// </summary>
		/// <param name="workspaceId">The ArtifactId for the workspace that contains the search</param>
		/// <param name="searchArtifactId">The ArtifactId for the search</param>
		/// <returns>SearchText for a given search artifact Id</returns>
		Search ReadById(int workspaceId, int searchArtifactId);
	}
}
