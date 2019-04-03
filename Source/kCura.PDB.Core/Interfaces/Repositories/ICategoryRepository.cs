namespace kCura.PDB.Core.Interfaces.Repositories
{
	using System.Threading.Tasks;
	using Models;

	public interface ICategoryRepository
	{
		Task<Category> CreateAsync(Category category);

		Task<Category> ReadAsync(int categoryId);

		Task<Category> ReadByCategoryScoreAsync(int categoryScoreId);

		Task UpdateAsync(Category category);

		Task DeleteAsync(Category category);
	}
}
