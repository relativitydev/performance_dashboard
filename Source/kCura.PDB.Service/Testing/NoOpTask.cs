namespace kCura.PDB.Service.Testing
{
	using System;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Testing;

	public class NoOpTask : INoOpTask
	{
		internal const int NumberOfFailsExepected = 2;
		private readonly IEventRepository eventRepository;

		public NoOpTask(IEventRepository eventRepository)
		{
			this.eventRepository = eventRepository;
		}

		public async Task FailsThenSucceeds(long eventId)
		{
			var evnt = await this.eventRepository.ReadAsync(eventId);

			if (evnt.Retries >= NumberOfFailsExepected)
			{
				return;
			}

			throw new Exception("Not enough retries");
		}
	}
}
