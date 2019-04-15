namespace kCura.PDB.Core.Interfaces.Repositories
{
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Models;

	public interface IEventLockRepository
	{
		/// <summary>
		/// Attempts to claim lock for Event's type and source ID
		/// </summary>
		/// <param name="evnt">Event to lock</param>
		/// <param name="workerId">the worker for the event lock if successfully claimed</param>
		/// <returns>A lock for the given event's type and source ID</returns>
		Task<EventLock> Claim(Event evnt, int workerId);

		/// <summary>
		/// Release Event Lock
		/// </summary>
		/// <param name="eventLock">Event Lock to remove</param>
		/// <returns>Task</returns>
		Task Release(EventLock eventLock);

		/// <summary>
		/// Returns a list of locks based on the worker
		/// </summary>
		/// <param name="eventWorker">the worker that the locks are related to</param>
		/// <returns>Task of event locks</returns>
		Task<IList<EventLock>> ReadByWorker(EventWorker eventWorker);

		/// <summary>
		/// Release all Event Locks for a worker
		/// </summary>
		/// <param name="eventLock">Worker to remove all locks for</param>
		/// <returns>Task</returns>
		Task Release(EventWorker eventWorker);
	}
}
