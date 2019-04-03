namespace kCura.PDB.Core.Interfaces.Queuing
{
	using System.Threading.Tasks;
	using kCura.PDB.Core.Models;

	public interface IEventOrphanService
	{
		/// <summary>
		/// Removes event locks and workers that are determined to be orphaned. Also sets related events to errored.
		/// </summary>
		/// <returns>Task</returns>
		Task ResolveOrphanedEventLocks();

		/// <summary>
		/// Read events associated with the locks and mark them as errored.
		/// </summary>
		/// <returns>Task</returns>
		Task MarkOrphanedEventsErrored(EventWorker orphanedWorker);

		/// <summary>
		/// Sets events that are In Progress to Errored if they haven't been updated for an hour.
		/// </summary>
		/// <returns>Task</returns>
		Task ResolveTimedOutEvents();
	}
}
