namespace kCura.PDB.Core.Interfaces.Agent
{
	using System.Collections.Generic;
	using System.Threading.Tasks;

	public interface IAgentManagerService
	{
		/// <summary>
		/// Starts given agents
		/// </summary>
		Task StartPerformanceDashboardAgentsAsync(IList<int> agentArtifactIds);

		/// <summary>
		/// Starts all Performance Dashboard agents
		/// </summary>
		Task StartPerformanceDashboardAgentsAsync();

		/// <summary>
		/// Disables currently running Performance Dashboard Agents
		/// </summary>
		/// <returns>List of ArtifactIDs for the Agents successfully stopped</returns>
		Task<IList<int>> StopPerformanceDashboardAgentsAsync();

		/// <summary>
		/// Creates missing Agents that are needed to run Performance Dashboard
		/// </summary>
		/// <param name="machineName">The Agent server name to create on</param>
		/// <returns>ArtifactIDs of the created agents</returns>
		Task<IList<int>> CreatePerformanceDashboardAgents(string machineName);
	}
}
