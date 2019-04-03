namespace kCura.PDB.Core.Interfaces.Metrics
{
	using System.Threading.Tasks;
	using kCura.PDB.Core.Models;

	public interface IMetricTask
	{
		/// <summary>
		/// Handles routing to a metric logic and collecting metric data for a given metric
		/// </summary>
		/// <param name="metricDataId">The Id for the metric data</param>
		/// <returns>Task</returns>
		Task<EventResult> CollectMetricData(int metricDataId);

		/// <summary>
		/// Handles routing to a metric logic and scoring metric data for a given metric
		/// </summary>
		/// <param name="metricDataId">The Id for the metric data</param>
		/// <returns>Task</returns>
		Task ScoreMetric(int metricDataId);

		/// <summary>
		/// Handles routing to a metric logic and checking if the metric data is ready for data collection for a given metric
		/// </summary>
		/// <param name="metricDataId">The Id for the metric data</param>
		/// <returns>Task</returns>
		Task<bool> CheckMetricReadyForDataCollection(int metricDataId);

		/// <summary>
		/// Starts any prerequisite events for a metric data
		/// </summary>
		/// <param name="metricDataId">The Id for the metric data</param>
		/// <returns>next events to start</returns>
		Task<EventResult> StartPrerequisitesForMetricData(int metricDataId);

		/// <summary>
		/// Checks if the current time is during or after the hours time stamp. Uses the metrics sample type to determine if data should be sampled during of after the hour.
		/// </summary>
		/// <param name="metricDataId">The Id for the metric data</param>
		/// <returns>Loops or goes to start prerequisites</returns>
		Task<EventResult> CheckSamplingPeriodForMetricData(int metricDataId);
	}
}
