namespace kCura.PDB.Core.Interfaces.Services
{
	using System.Collections.Generic;
	using kCura.PDB.Core.Models;

	public interface IRefreshServerService
	{
		IList<ResourceServer> GetServerList();
		void UpdateServerList(IList<ResourceServer> currentServers);
	}
}
