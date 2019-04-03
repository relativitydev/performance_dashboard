namespace kCura.PDB.Service.RecoverabilityIntegrity
{
	using System.Threading.Tasks;
	using kCura.PDB.Core.Interfaces.RecoverabilityIntegrity;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Models;

	public class GapsCollectionVerifier : IGapsCollectionVerifier
	{
		private readonly IMetricDataRepository metricDataRepository;
		private readonly IEventRepository eventRepository;

		public GapsCollectionVerifier(IMetricDataRepository metricDataRepository, IEventRepository eventRepository)
		{
			this.metricDataRepository = metricDataRepository;
			this.eventRepository = eventRepository;
		}


		/// <summary>
		/// For a given metric data it queries a sibling metric data with the same hour and server of type `MetricType.BackupGaps`.
		/// Then for that sibling backup gaps metric data it checks if it is complete or not.
		/// Note this checks if the dbcc gap processing is complete as well since both are done by the ProcessRecoverabilityForServer event
		/// </summary>
		/// <param name="metricData">The metric data</param>
		/// <returns>Task of true or false for if the gaps have been collected</returns>
		public async Task<bool> VerifyGapsCollected(MetricData metricData)
		{
			var backupGapsMetricData = await this.metricDataRepository.ReadByHourAndMetricTypeAsync(metricData.Metric.Hour, metricData.Server, MetricType.BackupGaps);

			return await this.VerifyGapsCollected(backupGapsMetricData.Id);
		}

		/// <summary>
		/// Given a metric id of the `MetricType.BackupGaps` it checks if the last `ProcessRecoverabilityForServer` event with that source id is complete
		/// Note this checks if the dbcc gap processing is complete as well since both are done by the ProcessRecoverabilityForServer event
		/// </summary>
		/// <param name="metricDataId">The source id for the event to check</param>
		/// <returns>Task of true or false for if the event for processing the backup and dbcc gaps are complete</returns>
		public async Task<bool> VerifyGapsCollected(int metricDataId)
		{
			var processGapsEvent = await this.eventRepository.ReadLastBySourceIdAndTypeAsync(
				EventSourceType.ProcessRecoverabilityForServer,
				metricDataId);

			return processGapsEvent.Status == EventStatus.Completed;
		}
	}
}
