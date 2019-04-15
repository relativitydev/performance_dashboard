namespace kCura.PDB.Core.Interfaces.Testing.Services
{
	using System.Threading;
	using System.Threading.Tasks;

	public interface IServiceExecution
    {
        Task ExecuteAsync(CancellationToken cancellationToken);
    }
}
