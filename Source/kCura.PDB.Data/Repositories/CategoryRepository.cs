namespace kCura.PDB.Data.Repositories
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using Core.Interfaces.Repositories;
	using Core.Interfaces.Services;
	using Core.Models;
	using Dapper;
	using Properties;

	public class CategoryRepository : ICategoryRepository
	{
		public CategoryRepository(IConnectionFactory connectionFactory)
		{
			this.connectionFactory = connectionFactory;
		}

		private readonly IConnectionFactory connectionFactory;

		public async Task<Category> CreateAsync(Category category)
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				var readResult = await conn.QueryFirstOrDefaultAsync<Category>(Resources.Category_CreateRead, category);
				if (readResult == null)
					await conn.ExecuteAsync(Resources.Category_CreateInsert, category);
				else
					return readResult;
				return await conn.QueryFirstOrDefaultAsync<Category>(Resources.Category_CreateRead, category);
			}
		}

		public async Task<Category> ReadAsync(int categoryId)
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				return await conn.QueryFirstOrDefaultAsync<Category>(Resources.Category_ReadByID, new { id = categoryId });
			}
		}

		public async Task<Category> ReadByCategoryScoreAsync(int categoryScoreId)
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				return await conn.QueryFirstOrDefaultAsync<Category>(Resources.Category_ReadByCategoryScore, new { categoryScoreId });
			}
		}

		public async Task UpdateAsync(Category category)
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				await conn.ExecuteAsync(Resources.Category_Update, category);
			}
		}

		public async Task DeleteAsync(Category category)
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				await conn.ExecuteAsync(Resources.Category_Delete, new { category.Id });
			}
		}
	}
}
