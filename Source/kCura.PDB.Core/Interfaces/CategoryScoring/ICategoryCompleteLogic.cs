namespace kCura.PDB.Core.Interfaces.CategoryScoring
{
	using System.Threading.Tasks;
	using kCura.PDB.Core.Models;

	public interface ICategoryCompleteLogic
	{
		Task CompleteCategory(Category category);
	}
}
