namespace kCura.PDB.Core.Interfaces.DataProviders
{
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.RecoverabilityIntegrity;

	public interface IDbccProvider
	{
		Task<IList<Dbcc>> GetDbccsAsync(Server server, IList<Database> databases);
	}
}
