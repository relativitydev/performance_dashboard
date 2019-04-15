namespace kCura.PDB.Core.Interfaces.Repositories
{
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Models;

	public interface IConfigurationAuditRepository : IDbRepository
	{
		IList<ConfigurationAudit> ReadAll();

		void Create(IList<ConfigurationAudit> audits);
	}
}
