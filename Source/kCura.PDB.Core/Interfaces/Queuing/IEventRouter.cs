namespace kCura.PDB.Core.Interfaces.Queuing
{
	using System.Threading.Tasks;
	using kCura.PDB.Core.Models;

	public interface IEventRouter
	{
		Task<EventResult> RouteEvent(Event evnt);
	}
}
