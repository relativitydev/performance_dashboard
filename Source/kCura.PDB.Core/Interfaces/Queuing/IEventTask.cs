namespace kCura.PDB.Core.Interfaces.Queuing
{
	using System.Threading.Tasks;
	using kCura.PDB.Core.Models;

	public interface IEventTask
	{

		/// <summary>
		/// Processes an event and creates next events on success
		/// </summary>
		/// <param name="evnt">the event</param>
		/// <returns>Next events to create</returns>
		Task<EventResult> ProcessEvent(Event evnt);


		/// <summary>
		/// Saves the results of an event processing.
		/// </summary>
		/// <param name="result">The result of the event processing</param>
		/// <param name="evnt">The event</param>
		/// <returns>Task</returns>
		Task MarkEventResultAsync(EventResult result, Event evnt);

		/// <summary>
		/// Create the next events
		/// </summary>
		/// <param name="result">The result from processing event</param>
		/// <param name="previousEvent">The event that was processed</param>
		/// <returns>Task</returns>
		Task CreateNextEvents(EventResult result, Event previousEvent);
	}
}
