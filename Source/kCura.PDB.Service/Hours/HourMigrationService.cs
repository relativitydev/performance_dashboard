namespace kCura.PDB.Service.Hours
{
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Interfaces.Hours;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Models;

	public class HourMigrationService : IHourMigrationService
	{
		private readonly IHourRepository hourRepository;
		private readonly IEventRepository eventRepository;
		private readonly ISampleHistoryRepository sampleHistoryRepository;

		public HourMigrationService(IHourRepository hourRepository, IEventRepository eventRepository, ISampleHistoryRepository sampleHistoryRepository)
		{
			this.hourRepository = hourRepository;
			this.eventRepository = eventRepository;
			this.sampleHistoryRepository = sampleHistoryRepository;
		}

		public Task CancelEvents() =>
			this.eventRepository.CancelEventsAsync(
				new[] { EventStatus.Pending, EventStatus.PendingHangfire, EventStatus.InProgress },
				EventConstants.PrerequisiteEvents);

		public Task<IList<int>> IdentifyIncompleteHours() => this.hourRepository.ReadIncompleteHoursAsync();

		public async Task<int> CancelHour(int hourId)
		{
			var hour = await this.hourRepository.ReadAsync(hourId);
			hour.Status = HourStatus.Cancelled;

			await Task.WhenAll(
				this.hourRepository.UpdateAsync(hour),
				this.sampleHistoryRepository.RemoveHourFromSampleAsync(hourId));

			return hourId;
		}
	}
}
