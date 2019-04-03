namespace kCura.PDB.Core.Interfaces.Repositories
{
	using System.Threading.Tasks;

	public interface IRatingsRepository
	{
		Task<bool> Exists(int hourId);
	}
}
