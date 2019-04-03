namespace kCura.PDB.Service.CategoryScoring
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;

	using global::Relativity.Toggles;

	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.CategoryScoring;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Toggles;
	using kCura.PDB.Service.Services;

	public class CategoryService : ICategoryService
	{
		public CategoryService(IServerRepository serverRepository, ICategoryRepository categoryRepository, ICategoryScoreRepository categoryScoreRepository, IToggleProvider toggleProvider)
		{
			this.serverRepository = serverRepository;
			this.categoryRepository = categoryRepository;
			this.categoryScoreRepository = categoryScoreRepository;
			this.toggleProvider = toggleProvider;
		}

		private readonly IServerRepository serverRepository;
		private readonly ICategoryRepository categoryRepository;
		private readonly ICategoryScoreRepository categoryScoreRepository;
		private readonly IToggleProvider toggleProvider;

		public async Task<IList<int>> CreateCategoriesForHour(int hourId)
		{
			var activeCategoryTypes = MetricConstants.ActiveCategoryTypes;

			// Filter R/I category out with toggle
			if (!await this.toggleProvider.IsEnabledAsync<RecoverabilityIntegrityMetricSystemToggle>())
			{
				activeCategoryTypes = activeCategoryTypes.Where(ct => ct != CategoryType.RecoverabilityIntegrity).ToArray();
			}

			var categories = await activeCategoryTypes
				.Select(ct => new Category { HourId = hourId, CategoryType = ct })
				.Select(c => this.categoryRepository.CreateAsync(c))
				.WhenAllStreamed();
			return categories.Select(c => c.Id).ToList();
		}

		public async Task<IList<int>> CreateCategoryScoresForCategory(int categoryId)
		{
			var category = await this.categoryRepository.ReadAsync(categoryId);

			// Get list of compatible serverTypes for this metric type
			var compatibleServerTypes = ServerTypeMapper.GetServerTypes(category.CategoryType);
			var categoryScoresToCreate = new List<CategoryScore>();
			if (ServerTypeMapper.GetServerTypes(category.CategoryType).Any())
			{
				var servers = await this.serverRepository.ReadAllActiveAsync();
				categoryScoresToCreate.AddRange(servers
					.Where(s => compatibleServerTypes.Any(st => s.ServerType == st))
					.GroupBy(s => s.ServerIpAddress)
					.Select(sg => new CategoryScore { CategoryId = category.Id, ServerId = sg.OrderBy(s => s.ServerId).First().ServerId }));
			}
			else
			{
				categoryScoresToCreate.Add(new CategoryScore { CategoryId = category.Id });
			}

			var categoryScores = await categoryScoresToCreate
				.Select(cs => this.categoryScoreRepository.CreateAsync(cs))
				.WhenAllStreamed();
			return categoryScores.Select(cs => cs.Id).ToList();
		}
	}
}
