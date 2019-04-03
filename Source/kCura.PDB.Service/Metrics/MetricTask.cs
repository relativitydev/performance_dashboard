namespace kCura.PDB.Service.Metrics
{
	using System;
	using System.Linq;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Metrics;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;

	public class MetricTask : IMetricTask
	{
		public MetricTask(
			IMetricDataRepository metricDataRepository,
			IServiceFactory<IMetricLogic, MetricType> metricServiceFactory,
			IServiceFactory<IMetricReadyForDataCollectionLogic, MetricType> isReadyServiceFactory,
			IMetricDataService metricDataService,
			ILogger logger)
		{
			this.metricServiceFactory = metricServiceFactory;
			this.isReadyServiceFactory = isReadyServiceFactory;
			this.metricDataRepository = metricDataRepository;
			this.metricDataService = metricDataService;
			this.logger = logger.WithClassName().WithCategory(Names.LogCategory.MetricData);
		}

		private readonly IServiceFactory<IMetricLogic, MetricType> metricServiceFactory;
		private readonly IServiceFactory<IMetricReadyForDataCollectionLogic, MetricType> isReadyServiceFactory;
		private readonly IMetricDataRepository metricDataRepository;
		private readonly IMetricDataService metricDataService;
		private readonly ILogger logger;

		/// <inheritdoc/>
		public async Task<EventResult> CollectMetricData(int metricDataId)
		{
			try
			{
				// Query the related data
				var metricData = await this.metricDataService.GetMetricData(metricDataId);

				await this.logger.LogVerboseAsync($"Collecting metric data for metric type: {metricData.Metric.MetricType} on metric data id: {metricDataId}");

				// If the metric's hour is after the current hour then we'll wait to collect data later
				if ((metricData.Metric.Hour.HourTimeStamp - DateTime.UtcNow).TotalMilliseconds > 0)
				{
					await this.logger.LogVerboseAsync($"Waiting to collect metric data for metric type: {metricData.Metric.MetricType} on metric data id: {metricDataId}");
					return new EventResult(metricDataId, EventSourceType.CollectMetricData, (int)(metricData.Metric.Hour.HourTimeStamp - DateTime.UtcNow).TotalSeconds);
				}

				// Get the metric logic class
				var logic = this.metricServiceFactory.GetService(metricData.Metric.MetricType);
				if (MetricConstants.ActiveMetricTypes.Contains(metricData.Metric.MetricType) && logic == null)
				{
					this.logger.LogError($"Cannot collect data for metric data, no logic class implemented for metric type: {metricData.Metric.MetricType} on metric data id: {metricDataId}");
				}

				// Collect data
				var result = (logic != null)
					? await logic.CollectMetricData(metricData)
					: null;

				// Save result
				this.metricDataService.SetData(metricData, result);
				await this.logger.LogVerboseAsync($"Collected metric data result {metricData.Data} for metric type: {metricData.Metric.MetricType} on metric data id: {metricDataId}");
				await this.metricDataRepository.UpdateAsync(metricData);

				// If the metric sample type is continuous then we will re-enqueue the metric collection if it's metric's hour. If it's greater than the metric's hour then we move on to scoring.
				var nextEvent = (metricData.Metric.SampleType == SampleType.Continuously && metricData.Metric.Hour.GetHourEnd() > DateTime.UtcNow)
					? EventSourceType.CollectMetricData
					: EventSourceType.ScoreMetricData;
				return new EventResult(metricDataId, nextEvent);
			}
			catch (Exception ex)
			{
				this.logger.LogError($"Failed to collect data for metric data Id: {metricDataId}", ex);
				throw;
			}
		}

		/// <inheritdoc/>
		public async Task ScoreMetric(int metricDataId)
		{
			try
			{
				// Query the related data
				var metricData = await this.metricDataService.GetMetricData(metricDataId);

				await this.logger.LogVerboseAsync($"Scoring metric data for metric type: {metricData.Metric.MetricType} on metric data id: {metricDataId}");

				// Get the metric logic class
				var logic = this.metricServiceFactory.GetService(metricData.Metric.MetricType);
				if (MetricConstants.ActiveMetricTypes.Contains(metricData.Metric.MetricType) && logic == null)
				{
					this.logger.LogError($"Cannot score metric data, no logic class implemented for metric type: {metricData.Metric.MetricType} on metric data id: {metricDataId}");
				}

				// Score metric data
				var result = (logic != null)
					? await logic.ScoreMetric(metricData)
					: Defaults.Scores.OneHundred;

				await this.logger.LogVerboseAsync($"Scored metric data {result} for metric type: {metricData.Metric.MetricType} on metric data id: {metricDataId}");

				// Save result
				metricData.Score = result;
				await this.metricDataRepository.UpdateAsync(metricData);
			}
			catch (Exception ex)
			{
				this.logger.LogError($"Failed to score metric for metric data Id: {metricDataId}", ex);
				throw;
			}
		}

		public async Task<EventResult> CheckSamplingPeriodForMetricData(int metricDataId)
		{
			var metricData = await this.metricDataService.GetMetricData(metricDataId);

			// if the current time is during or after the sample period then start the metric data prerequisites
			return IsDuringOrAfterSamplePeriod(metricData.Metric)
				? new EventResult(metricDataId, EventSourceType.StartPrerequisitesForMetricData)
				: new EventResult(metricDataId, EventSourceType.CheckSamplingPeriodForMetricData, delay: GetTimeRemainingTillSamplePeriod(metricData.Metric));
		}

		/// <inheritdoc/>
		public async Task<bool> CheckMetricReadyForDataCollection(int metricDataId)
		{
			var metricData = await this.metricDataService.GetMetricData(metricDataId);

			// If we've fallen out of the backfill range then just we want to stop
			// -6 hours just to avoid any off by 1 error
			if (metricData.Metric.Hour.HourTimeStamp < DateTime.UtcNow.NormilizeToHour().AddDays(Defaults.BackfillDays).AddHours(-6))
			{
				//throw new Exception($"Error checking metric is ready since metric is out of backfill range. Metric Data: {metricDataId}, Hour: {metricData.Metric.Hour.HourTimeStamp}, Current Hour: {DateTime.UtcNow.NormilizeToHour()}");
				await this.logger.LogVerboseAsync($"Error checking metric is ready since metric is out of backfill range. Metric Data: {metricDataId}, Hour: {metricData.Metric.Hour.HourTimeStamp}, Current Hour: {DateTime.UtcNow.NormilizeToHour()}");
			}

			// Get the metric logic class
			var logic = this.isReadyServiceFactory.GetService(metricData.Metric.MetricType);

			// check metric data pre-reqs are ready
			// some metrics do NOT have prerequisites checks so if logic is null then we move on to data collection.
			var result = logic == null || await logic.IsReady(metricData);

			// Save result
			await this.logger.LogVerboseAsync(result
				? $"Metric {metricData.Metric.MetricType} is ready for data collection"
				: $"Metric {metricData.Metric.MetricType} is waiting for metric data prerequisites");
			return result;
		}

		public async Task<EventResult> StartPrerequisitesForMetricData(int metricDataId)
		{
			var metricData = await this.metricDataService.GetMetricData(metricDataId);

			// Get any prerequisites and append CheckMetricDataIsReadyForDataCollection
			var prerequisiteEventTypes =
				MetricConstants.MetricPrerequisites[metricData.Metric.MetricType]
				.Concat(new[] { EventSourceType.CheckMetricDataIsReadyForDataCollection })
				.ToList();

			return new EventResult(metricDataId, prerequisiteEventTypes);
		}

		internal static DateTime GetSamplePeriodDateTime(Metric metric) =>
			metric.SampleType == SampleType.Continuously ? metric.Hour.HourTimeStamp : metric.Hour.HourTimeStamp.AddHours(1);

		internal static bool IsDuringOrAfterSamplePeriod(Metric metric) =>
			GetSamplePeriodDateTime(metric) <= DateTime.UtcNow;

		internal static int GetTimeRemainingTillSamplePeriod(Metric metric) =>
			(int)Math.Max((GetSamplePeriodDateTime(metric) - DateTime.UtcNow).TotalSeconds, 0);
	}
}
