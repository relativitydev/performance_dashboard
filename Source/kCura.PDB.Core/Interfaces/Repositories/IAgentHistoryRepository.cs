namespace kCura.PDB.Core.Interfaces.Repositories
{
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Models;

	public interface IAgentHistoryRepository
	{
		Task<AgentHistory> CreateAsync(AgentHistory history);

		Task<AgentHistory> ReadAsync(int id);

		Task<IList<AgentHistory>> ReadByHourAsync(Hour hour);

		Task<AgentHistory> ReadEarliestAsync();

		Task UpdateAsync(AgentHistory history);

		Task DeleteAsync(AgentHistory history);
	}
}
