namespace kCura.PDB.Core.Interfaces.Queuing
{
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Models;

	public interface IEventSourceService
	{
		Task EnqueueTasksForPendingEvents();

		Task CreateHourProcessingEvents();

		Task CreateNextEvents(IList<int> sourceIds, IList<EventSourceType> eventSourceTypes, int? delay, long? previousEventId, int? hourId);
	}
}
