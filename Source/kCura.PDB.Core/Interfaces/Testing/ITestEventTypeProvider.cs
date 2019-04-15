namespace kCura.PDB.Core.Interfaces.Testing
{
	using System.Collections.Generic;

	using kCura.PDB.Core.Models;

	public interface ITestEventTypeProvider
    {
        /// <summary>
        /// Returns a list of event types to enqueue for the TestMetricManager
        /// </summary>
        /// <returns>List of event types to enqueue</returns>
        IList<EventSourceType> GetEventTypesToEnqueue();

        /// <summary>
        /// Returns an event type to poll for completion
        /// </summary>
        /// <returns>Event Type to poll for completion</returns>
        EventSourceType GetEventTypeToComplete();
    }
}
