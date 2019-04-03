namespace kCura.PDB.Core.Interfaces.Repositories
{
	public interface ISearchFolderRepository
	{
		int GetSearchFolderCountForSearch(int workspaceId, int searchArtifactId);
	}
}
