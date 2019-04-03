namespace kCura.PDB.Core.Interfaces.CategoryScoring
{
	using System.Threading.Tasks;

	public interface ICategoryCompleteTask
	{
		Task<bool> CompleteCategory(int sourceId);
	}
}
