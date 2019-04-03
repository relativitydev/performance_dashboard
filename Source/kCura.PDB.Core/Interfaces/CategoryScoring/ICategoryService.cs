namespace kCura.PDB.Core.Interfaces.CategoryScoring
{
	using System.Collections.Generic;
	using System.Threading.Tasks;

	public interface ICategoryService
	{
		Task<IList<int>> CreateCategoriesForHour(int hourId);

		Task<IList<int>> CreateCategoryScoresForCategory(int categoryId);
	}
}
