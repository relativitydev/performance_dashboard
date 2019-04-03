namespace kCura.PDB.Service.Agent
{
    using System.Threading;
    using System.Threading.Tasks;

    using kCura.PDB.Core.Extensions;
    using kCura.PDB.Core.Interfaces.Queuing;
    using kCura.PDB.Core.Interfaces.Services;
    using kCura.PDB.Core.Models;
    using kCura.PDB.Data;
    using Ninject;

	public class MetricSystemWorkerAgentLogic : IMetricSystemWorkerService
    {
        private readonly IQueuingConfiguration queuingConfiguration;
        private readonly IEventWorkerService eventWorkerService;
        private readonly IEventOrphanService eventOrphanService;
        private readonly IKernel kernel;
        private readonly ILogger logger;

        public MetricSystemWorkerAgentLogic(
            IKernel kernel,
            IQueuingConfiguration queuingConfiguration,
            IEventWorkerService eventWorkerService,
            IEventOrphanService eventOrphanService,
            ILogger logger)
        {
            this.kernel = kernel;
            this.queuingConfiguration = queuingConfiguration;
            this.eventWorkerService = eventWorkerService;
            this.eventOrphanService = eventOrphanService;
            this.logger = logger.WithTypeName(this);  //.WithClassName(); -- doesn't work
        }

		public async Task Execute(CancellationToken cancellationToken, EventWorker eventWorker = null)
        {
            cancellationToken.ThrowIfCancellationRequested();

            DataSetup.Setup();

            this.queuingConfiguration.ConfigureSystem();
            var worker = await this.eventWorkerService.CreateWorker(eventWorker);

            // Mark any events and event locks as orphaned since we haven't started processing anything yet.
            await this.eventOrphanService.MarkOrphanedEventsErrored(worker);

            await this.logger.LogVerboseAsync(
                $"Starting Job Server on Event Worker Id: {worker?.Id} - Name: {worker?.Name}");
            using (var server = this.kernel.Get<IJobServer>())
            {
                await server.WaitTillProcessesAreDone(cancellationToken);

                await this.logger.LogVerboseAsync(
                    $"Processes done, begin jobServer dispose on Event Worker Id: {worker?.Id} - Name: {worker?.Name}");
            }
            
            await this.logger.LogVerboseAsync(
                $"Job Server disposed on Event Worker Id: {worker?.Id} - Name: {worker?.Name}");

            await this.eventWorkerService.RemoveCurrentWorker();
            
            await this.logger.LogVerboseAsync(
                $"Removed Event Worker Id: {worker?.Id} - Name: {worker?.Name}");
        }
    }
}
