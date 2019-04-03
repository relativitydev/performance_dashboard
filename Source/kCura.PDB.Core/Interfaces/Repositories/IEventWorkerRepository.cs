namespace kCura.PDB.Core.Interfaces.Repositories
{
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Models;

	public interface IEventWorkerRepository
	{
		/// <summary>
		/// Creates a worker
		/// </summary>
		/// <param name="eventWorker">The worker to create. Id is required</param>
		/// <returns>Task with the worker</returns>
		Task<EventWorker> CreateAsync(EventWorker eventWorker);

		/// <summary>
		/// Reads all current workers
		/// </summary>
		/// <returns>Task with list of workers</returns>
		Task<IList<EventWorker>> ReadAllWorkersAsync();

		/// <summary>
		/// Reads the worker
		/// </summary>
		/// <param name="id">the id of the worker</param>
		/// <returns>Task with list of workers</returns>
		Task<EventWorker> ReadAsync(int id);

		/// <summary>
		/// Deletes the worker
		/// </summary>
		/// <param name="eventWorker">The worker to delete</param>
		/// <returns>Task</returns>
		Task DeleteAsync(EventWorker eventWorker);
	}
}
