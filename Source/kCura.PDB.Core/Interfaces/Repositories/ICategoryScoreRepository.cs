namespace kCura.PDB.Core.Interfaces.Repositories
{
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Models;

	public interface ICategoryScoreRepository
	{
		Task<CategoryScore> CreateAsync(CategoryScore score);

		Task<CategoryScore> ReadAsync(int id);

		Task<IList<CategoryScore>> ReadAsync(Hour hour);

		Task<IList<CategoryScore>> ReadAsync(Category category);

		Task UpdateAsync(CategoryScore score);

		Task DeleteAsync(CategoryScore score);
	}
}
