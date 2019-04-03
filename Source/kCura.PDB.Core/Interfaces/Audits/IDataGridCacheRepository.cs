namespace kCura.PDB.Core.Interfaces.Audits
{
	using System.Threading.Tasks;

	public interface IDataGridCacheRepository
	{
		Task<bool> ReadUseDataGrid(int workspaceId, int hourId);

		Task UpdateDataGridCache(int workspaceId, int earliestDataGridHourId);

		Task Clear(int workspaceId);
	}
}
