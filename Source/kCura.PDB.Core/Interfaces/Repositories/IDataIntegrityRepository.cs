namespace kCura.PDB.Core.Interfaces.Repositories
{
	using System.Threading.Tasks;
	using kCura.PDB.Core.Models;

	public interface IDataIntegrityRepository
	{
		Task DropAllTriggersInCurrentDatabase();

		Task DropAllTriggersInCurrentDatabase(Server server);
	}
}
