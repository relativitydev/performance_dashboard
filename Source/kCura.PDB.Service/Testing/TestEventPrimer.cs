namespace kCura.PDB.Service.Testing
{
	using System.Linq;
	using System.Threading.Tasks;

	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Servers;
	using kCura.PDB.Core.Interfaces.Testing.Repositories;
	using kCura.PDB.Core.Interfaces.Testing.Services;
	using kCura.PDB.Core.Models;

	public class TestEventPrimer : ITestEventPrimer
	{
		private readonly IHourTestDataRepository hourTestDataRepository;
		private readonly ICategoryRepository categoryRepository;
		private readonly IEventRepository eventRepository;
		private readonly IDatabaseService databaseService;

		public TestEventPrimer(
			IHourTestDataRepository hourTestDataRepository,
			ICategoryRepository categoryRepository,
			IEventRepository eventRepository,
			IDatabaseService databaseService)
		{
			this.hourTestDataRepository = hourTestDataRepository;
			this.categoryRepository = categoryRepository;
			this.eventRepository = eventRepository;
			this.databaseService = databaseService;
		}

		public async Task CreateEventDataAsync()
		{
			// Read test hours
			var createdHours = await this.hourTestDataRepository.ReadHoursAsync();

			// Create the test category
			var categories = createdHours.Select(
				h => new Category { HourId = h.Id, CategoryType = CategoryType.RecoverabilityIntegrity });
			var createdCategories = await categories.Select(c => this.categoryRepository.CreateAsync(c)).WhenAllStreamed();

			// Create a pending event (CreateCategoryScoresForCategory) to prime the event system
			var events = createdCategories.Select(
				c => new Event
					     {
						     HourId = c.HourId,
						     SourceId = c.Id,
						     SourceType = EventSourceType.CreateCategoryScoresForCategory
					     });
			var createdEvents = await events.Select(e => this.eventRepository.CreateAsync(e)).WhenAllStreamed();

			// Create test databases
			await this.databaseService.UpdateTrackedDatabasesAsync();
		}
	}
}
