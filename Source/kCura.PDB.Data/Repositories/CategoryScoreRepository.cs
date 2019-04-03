namespace kCura.PDB.Data.Repositories
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using Dapper;
	using Core.Interfaces.Repositories;
	using Core.Interfaces.Services;
	using Core.Models;
	using Properties;

	public class CategoryScoreRepository : ICategoryScoreRepository
	{
		public CategoryScoreRepository(IConnectionFactory connectionFactory)
		{
			this.connectionFactory = connectionFactory;
		}

		private readonly IConnectionFactory connectionFactory;

		public async Task<CategoryScore> CreateAsync(CategoryScore score)
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				var readResult = await conn.QueryFirstOrDefaultAsync<CategoryScore>(Resources.CategoryScore_ReadByCategoryAndServer, score);
				if (readResult == null)
					await conn.ExecuteAsync(Resources.CategoryScore_Create, score);
				else
					return readResult;
				return await conn.QueryFirstOrDefaultAsync<CategoryScore>(Resources.CategoryScore_ReadByCategoryAndServer, score);
			}
		}

		public async Task<CategoryScore> ReadAsync(int id)
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				return await conn.QueryFirstOrDefaultAsync<CategoryScore>(Resources.CategoryScore_ReadByID, new { id });
			}
		}

		public async Task<IList<CategoryScore>> ReadAsync(Hour hour)
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				return (await conn.QueryAsync<CategoryScore>(Resources.CategoryScore_ReadByHour, new { hourId = hour.Id })).ToList();
			}
		}

		public async Task<IList<CategoryScore>> ReadAsync(Category category)
		{
			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{
				return (await conn.QueryAsync<CategoryScore>(Resources.CategoryScore_ReadByCategory, new { categoryId = category.Id })).ToList();
			}
		}

		public async Task UpdateAsync(CategoryScore score)
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				await conn.ExecuteAsync(Resources.CategoryScore_Update, score);
			}
		}

		public async Task DeleteAsync(CategoryScore score)
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				await conn.ExecuteAsync(Resources.CategoryScore_Delete, score);
			}
		}
	}
}