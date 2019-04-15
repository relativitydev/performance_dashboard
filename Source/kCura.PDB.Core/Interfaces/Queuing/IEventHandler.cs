namespace kCura.PDB.Core.Interfaces.Queuing
{
	using System.Threading.Tasks;
	using kCura.PDB.Core.Models;

	public interface IEventHandler
	{
		/// <summary>
		/// Processes an event and creates next events on success
		/// </summary>
		/// <param name="eventId">the event id</param>
		/// <param name="eventType">the event type</param>
		/// <returns>Task</returns>
		Task HandleEvent(long eventId, EventSourceType eventType);
	}
}
