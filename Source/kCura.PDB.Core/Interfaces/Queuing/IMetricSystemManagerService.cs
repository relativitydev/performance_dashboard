namespace kCura.PDB.Core.Interfaces.Queuing
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IMetricSystemManagerService
    {
        Task Execute(CancellationToken cancellationToken);
    }
}
