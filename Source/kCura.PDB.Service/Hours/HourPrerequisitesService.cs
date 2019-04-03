namespace kCura.PDB.Service.Hours
{
	using System.Linq;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Interfaces.Hours;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Models;

	public class HourPrerequisitesService : IHourPrerequisitesService
	{
		private readonly IServerRepository serverRepository;
		private readonly IEventRepository eventRepository;
		private readonly IHourRepository hourRepository;

		public HourPrerequisitesService(
			IServerRepository serverRepository,
			IEventRepository eventRepository,
			IHourRepository hourRepository)
		{
			this.serverRepository = serverRepository;
			this.eventRepository = eventRepository;
			this.hourRepository = hourRepository;
		}

		public async Task<EventResult> CheckForPrerequisites()
		{
			var eventSytemState = await this.eventRepository.ReadEventSystemStateAsync();
			if (eventSytemState == EventSystemState.Paused || eventSytemState == EventSystemState.Prerequisites) return EventResult.Stop;

			var serversPendingQosDeployment = await this.serverRepository.ReadServerPendingQosDeploymentAsync();
			if (!serversPendingQosDeployment.Any())
			{
				return new EventResult(new[] { EventSourceType.CreateNextHour, EventSourceType.FindNextCategoriesToScore });
			}

			await this.eventRepository.UpdateEventTypesAsync();
			await this.eventRepository.UpdateReadEventSystemStateAsync(EventSystemState.Prerequisites);

			return new EventResult(EventSourceType.StartPrerequisites);
		}

		public async Task<bool> CheckAllPrerequisitesComplete()
		{
			// If system is already in normal state then all prerequisites must be complete
			var eventSytemState = await this.eventRepository.ReadEventSystemStateAsync();
			if (eventSytemState == EventSystemState.Normal) return true;

			var serversPendingQosDeployment = await this.serverRepository.ReadServerPendingQosDeploymentAsync();
			var incompleteHours = await this.hourRepository.ReadAnyIncompleteHoursAsync();
			return !serversPendingQosDeployment.Any() && !incompleteHours;
		}

		public async Task CompletePrerequisites() =>
			await this.eventRepository.UpdateReadEventSystemStateAsync(EventSystemState.Normal);
	}
}
