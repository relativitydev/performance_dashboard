namespace kCura.PDB.Service.RecoverabilityIntegrity
{
	using System.Threading.Tasks;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Metrics;
	using kCura.PDB.Core.Interfaces.RecoverabilityIntegrity;
	using kCura.PDB.Core.Interfaces.Services;

	public class ServerRecoverabilityProcessor : IServerRecoverabilityProcessor
	{
		private readonly IServerBackupProcessor serverBackupProcessor;
		private readonly IServerDbccProcessor serverDbccProcessor;
		private readonly IMetricDataService metricDataService;
		private readonly ILogger logger;

		public ServerRecoverabilityProcessor(
			IServerBackupProcessor serverBackupProcessor,
			IServerDbccProcessor serverDbccProcessor,
			IMetricDataService metricDataService,
			ILogger logger)
		{
			this.serverBackupProcessor = serverBackupProcessor;
			this.serverDbccProcessor = serverDbccProcessor;
			this.metricDataService = metricDataService;
			this.logger = logger.WithClassName().WithCategory(Names.LogCategory.RecoverabilityIntegrity);
		}

		public async Task ProcessRecoverabilityForServer(int metricDataId)
		{

			var metricData = await this.metricDataService.GetMetricData(metricDataId);
			await this.logger.LogVerboseAsync($"Processing backups for {metricData.Server.ServerName}");
			await this.serverBackupProcessor.ProcessBackupsForServer(metricData.Metric.Hour, metricData.Server);
			await this.logger.LogVerboseAsync($"Processing dbccs for {metricData.Server.ServerName}");
			await this.serverDbccProcessor.ProcessDbccsForServer(metricData.Metric.Hour, metricData.Server);
			await this.logger.LogVerboseAsync($"Completed processing backups and dbccs for {metricData.Server.ServerName}");
		}
	}
}
