namespace kCura.PDB.Core.Interfaces.Repositories
{
	using System.Threading.Tasks;
	using kCura.PDB.Core.Models;

	public interface IServerCleanupRepository
	{
		Task<ServerCleanup> ReadAsync(int serverCleanupId);

		Task<ServerCleanup> CreateAsync(ServerCleanup serverCleanup);

		Task UpdateAsync(ServerCleanup serverCleanup);
	}
}
