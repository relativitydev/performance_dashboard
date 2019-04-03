namespace kCura.PDB.Core.Interfaces.Repositories
{
	using System.Threading.Tasks;
	using kCura.PDB.Core.Models;

	public interface IBackfillRepository : IDbRepository
	{
		Task<int> ReadHoursAwaitingDiscovery();

		Task<int> ReadHoursAwaitingAnalysis();

		Task<int> ReadHoursAwaitingScoring();

		Task<int> ReadHoursCompletedScoring();
	}
}
