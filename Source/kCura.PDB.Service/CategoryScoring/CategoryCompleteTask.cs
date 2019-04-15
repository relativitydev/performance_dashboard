namespace kCura.PDB.Service.CategoryScoring
{
	using System.Linq;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Interfaces.CategoryScoring;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;

	public class CategoryCompleteTask : ICategoryCompleteTask
	{
		private readonly ICategoryScoreRepository categoryScoreRepository;
		private readonly ICategoryRepository categoryRepository;
		private readonly IServiceFactory<ICategoryCompleteLogic, CategoryType> categoryCompleteFactory;
		private readonly ILogger logger;

		public CategoryCompleteTask(ICategoryScoreRepository categoryScoreRepository, ICategoryRepository categoryRepository, IServiceFactory<ICategoryCompleteLogic, CategoryType> categoryCompleteFactory, ILogger logger)
		{
			this.categoryScoreRepository = categoryScoreRepository;
			this.categoryRepository = categoryRepository;
			this.categoryCompleteFactory = categoryCompleteFactory;
			this.logger = logger;
		}

		public async Task<bool> CompleteCategory(int categoryId)
		{
			// Query the related data
			var category = await this.categoryRepository.ReadAsync(categoryId);

			// Check if this is the last category (are all scored?)
			var categoryScoresForCategory = await this.categoryScoreRepository.ReadAsync(category);
			if (categoryScoresForCategory.Any(cs => !cs.Score.HasValue))
			{
				// If not, return false
				return false;
			}

			// If so, execute category specific logic
			var logic = this.categoryCompleteFactory.GetService(category.CategoryType);
			if (logic == null && MetricConstants.ActiveCategoryTypes.Contains(category.CategoryType))
			{
				this.logger.LogError($"No CategoryComplete logic class implemented for category type: {category.CategoryType} on category id: {categoryId}");
			}

			// If this throws an exception, let it fail the event
			logic?.CompleteCategory(category);
			
			// Complete successfully
			return true;
		}
	}
}
