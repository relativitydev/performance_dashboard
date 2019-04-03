namespace kCura.PDB.Service.Queuing
{
	using System;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Agent;
	using kCura.PDB.Core.Interfaces.Queuing;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Models;

	public class EventWorkerService : IEventWorkerService
	{
		private readonly IAgentService agentService;
		private readonly IEventWorkerRepository eventWorkerRepository;

		public EventWorkerService(IAgentService agentService, IEventWorkerRepository eventWorkerRepository)
		{
			this.agentService = agentService;
			this.eventWorkerRepository = eventWorkerRepository;
		}

		public Task<EventWorker> GetCurrentWorker() =>
			this.eventWorkerRepository.ReadAsync(this.agentService.AgentID);

		public Task<EventWorker> CreateWorker(EventWorker worker = null) =>
			this.eventWorkerRepository.CreateAsync(worker ?? new EventWorker(this.agentService));

		public Task RemoveCurrentWorker() =>
			this.GetCurrentWorker()
			.PipeAsync(this.eventWorkerRepository.DeleteAsync);
	}
}
