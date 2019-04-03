namespace kCura.PDB.Core.Interfaces.DatabaseDeployment
{
	using System.Collections.Generic;
	using System.Threading.Tasks;

	public interface IQosDatabaseDeployer
	{
		Task<IList<int>> StartQosDatabaseDeployment();

		Task ServerDatabaseDeployment(int serverId);
	}
}
