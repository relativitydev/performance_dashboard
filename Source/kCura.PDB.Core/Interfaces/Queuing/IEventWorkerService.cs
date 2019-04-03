namespace kCura.PDB.Core.Interfaces.Queuing
{
	using System.Threading.Tasks;
	using kCura.PDB.Core.Models;

	public interface IEventWorkerService
	{
		Task<EventWorker> GetCurrentWorker();

		Task<EventWorker> CreateWorker(EventWorker worker = null);

		Task RemoveCurrentWorker();
	}
}
