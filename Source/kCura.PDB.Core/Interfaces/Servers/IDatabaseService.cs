namespace kCura.PDB.Core.Interfaces.Servers
{
	using System.Collections.Generic;
	using System.Threading.Tasks;

	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.RecoverabilityIntegrity;

	public interface IDatabaseService
	{
		Task UpdateTrackedDatabasesAsync();

		Task<IList<Gap>> ReadUnresolvedGapsAsync(Hour hour, Server server, GapActivityType activityType);
	}
}
