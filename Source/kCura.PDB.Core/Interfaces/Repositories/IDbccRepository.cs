namespace kCura.PDB.Core.Interfaces.Repositories
{
	using System.Threading.Tasks;
	using kCura.PDB.Core.Interfaces.DataProviders;

	public interface IDbccRepository : IDbccProvider
	{
		Task RunDbcc();
	}
}
