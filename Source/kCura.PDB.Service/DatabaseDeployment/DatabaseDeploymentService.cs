namespace kCura.PDB.Service.DatabaseDeployment
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.DatabaseDeployment;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Models;

	public class DatabaseDeploymentService
	{
		private readonly IEventRepository eventRepository;
		private readonly IServerRepository serverRepository;

		public DatabaseDeploymentService(IEventRepository eventRepository, IServerRepository serverRepository)
		{
			this.eventRepository = eventRepository;
			this.serverRepository = serverRepository;
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an async method.")]
		public async Task<IList<Event>> GetLastDeploymentEvents()
		{
			var servers = await this.serverRepository.ReadAllActiveAsync();
			var databaseServers = servers.Where(s => s.ServerType == ServerType.Database).ToList();
			return (await databaseServers
				.Select(server => this.eventRepository.ReadLastBySourceIdAndTypeAsync(EventSourceType.DeployServerDatabases, server.ServerId))
				.WhenAllStreamed())
				.Where(e => e != null)
				.ToList();
		}

		public async Task<bool> FindAnyFailedDeployments()
		{
			var lastDeploymentEvents = await this.GetLastDeploymentEvents();
			return lastDeploymentEvents.Any(e => e.Status == EventStatus.Error);
		}
	}
}
