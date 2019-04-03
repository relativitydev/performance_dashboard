namespace kCura.PDB.Core.Interfaces.Agent
{
	public interface IAgentService
	{
		int AgentID { get; }

		string Name { get; }

		int LoggingLevel { get; set; }

		void RaiseMessage(string message, int level);

		void RaiseWarning(string message);

		void RaiseWarning(string message, string detailMessage);

		void RaiseError(string message, string detailMessage);
	}
}
