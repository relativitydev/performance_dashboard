namespace kCura.PDB.Core.Interfaces.Repositories
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;

	public interface IAgentRepository : IDbRepository
	{
		bool ReadAgentEnabled(int agentId);

		Task<IList<int>> ReadAgentsAsync(IList<Guid> agentGuids);

		Task<IList<int>> ReadAllEnabledAgentsAsync(/* IList<Guid> agentGuids = null */);

		Task<IList<int>> ReadAllAgentsAsync(/* IList<Guid> agentGuids = null */);

		Task<bool> ReadAgentWithTypeExists(Guid guid);
	}
}
