namespace kCura.PDB.Core.Interfaces.Queuing
{
    using System.Threading;
    using System.Threading.Tasks;
    using kCura.PDB.Core.Models;

    public interface IMetricSystemWorkerService
    {
        Task Execute(CancellationToken cancellationToken, EventWorker eventWorker = null);
    }
}
