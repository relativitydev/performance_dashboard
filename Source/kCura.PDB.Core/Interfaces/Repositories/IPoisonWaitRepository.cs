namespace kCura.PDB.Core.Interfaces.Repositories
{
	using System.Threading.Tasks;
	using kCura.PDB.Core.Models;

	public interface IPoisonWaitRepository
	{
		Task<bool> ReadIfPoisonWaitsForHourAsync(Hour hour);

		Task<bool> ReadPoisonWaitsForHourAsync(Hour hour, int serverId);
	}
}
