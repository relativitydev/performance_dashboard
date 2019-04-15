namespace kCura.PDB.Core.Models
{
	using kCura.PDB.Core.Interfaces.Agent;

	public class EventWorker
	{
		public EventWorker()
		{

		}

		public EventWorker(IAgentService agent)
		{
			this.Id = agent.AgentID;
			this.Name = agent.Name;
			this.Type = EventWorkerType.Agent;
		}

		public int Id { get; set; }

		public string Name { get; set; }

		public EventWorkerType Type { get; set; }
	}

	public enum EventWorkerType
	{
		Agent = 1,
		Other = 99,
	}
}
