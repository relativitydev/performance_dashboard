namespace kCura.PDB.Service.Agent
{
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Agent;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Service.Logging;

	public class AgentLogger : GenericLogger, ILogger
	{
		public AgentLogger(IAgentService agentService)
		{
			this.agentService = agentService;
		}

		private readonly IAgentService agentService;

		// kCura Log Levels:
		// 0-1 Error
		// 2-5 Warning
		// 6-10 Informational

		protected override void Log(int level, string message, params string[] categories)
		{
			var merged = $@"{string.Join(", ", categories)}: {message}".Truncate(30000);
			if (level <= 1)
			{
				this.agentService.RaiseError(merged, string.Empty);
			}
			else if (level <= 5)
			{
				this.agentService.RaiseWarning(merged, string.Empty);
			}
			else
			{
				this.agentService.RaiseMessage(merged, level);
			}
		}
	}
}
