namespace kCura.PDB.Core.Interfaces.Repositories
{
	using System.Collections.Generic;
	using System.Xml.Linq;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;

	public interface IResourceServerRepository : IDbRepository
	{
		IEnumerable<ResourceServer> ReadResourceServers();

		IEnumerable<string> ReadFileServers();

		IList<ResourceServer> GetAllServers(ILogger logger);

		void MergeServerInformation(XElement xml);
	}
}
