namespace kCura.PDB.Service.Logging
{
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;

	public class LogContext : ILogContext
	{
		public LogContext(Event evnt = null)
		{
			this.Event = evnt;
		}

		public Event Event { get; set; }
	}
}
