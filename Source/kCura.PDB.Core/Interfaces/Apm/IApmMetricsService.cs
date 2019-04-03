namespace kCura.PDB.Core.Interfaces.Apm
{
	using System;
	using System.Collections.Generic;
	using global::Relativity.Telemetry.APM;
	using global::Relativity.Telemetry.APM.Types;
	using kCura.PDB.Core.Interfaces.Apm.Recorders;

	public interface IApmMetricsService
	{
		/// <summary>
		/// Creates a disposable meter recorder to measure the duration of an event
		/// </summary>
		/// <param name="measureName">Unique name for the measure</param>
		/// <param name="measurement">Description of what is being measured ie download speed in Mb/s</param>
		/// <param name="unitOfMeasure">Unit of time measurement, defaults to seconds</param>
		/// <param name="customData">optional custom data</param>
		/// <returns>disposable apm recorder</returns>
		IMeterRecorder RecordMeter(
			string measureName,
			string measurement,
			TimeUnit unitOfMeasure = TimeUnit.Seconds,
			object customData = null);

		/// <summary>
		/// Creates APM recorder to measure a count of operations
		/// </summary>
		/// <param name="measureName">Unique name for the measure</param>
		/// <param name="unitOfMeasure">unit of measure</param>
		/// <param name="customData">optional custom data</param>
		/// <returns>disposable apm recorder</returns>
		ICounterRecorder RecordCounter(
			string measureName,
			string unitOfMeasure = "item(s)",
			object customData = null);

		/// <summary>
		/// Creates APM recorder to measure a value with a provided function.
		/// Gauge supports Int, Long, or Double
		/// </summary>
		/// <typeparam name="T">return type of the measurement unit; supports Int, Long, or Double</typeparam>
		/// <param name="measureName">Unique name for the measure</param>
		/// <param name="value">the value to measure</param>
		/// <param name="unitOfMeasure">unit of measure</param>
		/// <param name="correlationId">correlates a group of metrics</param>
		/// <param name="customData">optional custom data</param>
		void RecordGauge<T>(
			string measureName,
			T value,
			string unitOfMeasure = "item(s)",
			string correlationId = null,
			object customData = null);

		/// <summary>
		/// Creates APM recorder to measure a value with a provided function repeatedly at a given interval.
		/// Gauge supports Int, Long or Double
		/// </summary>
		/// <typeparam name="T">return type of the measurement unit, supports Int, Long, or Double</typeparam>
		/// <param name="measureName">Unique name for the measure</param>
		/// <param name="function">function to collect the measure</param>
		/// <param name="interval">collection interval</param>
		/// <param name="unitOfMeasure">unit of measure</param>
		/// <param name="customData">optional custom data</param>
		/// <returns>disposable apm recorder</returns>
		ITimedGaugeRecorder RecordTimedGauge<T>(
			string measureName,
			Func<T> function,
			TimeSpan interval,
			string unitOfMeasure = "item(s)",
			object customData = null);

		/// <summary>
		/// Creates APM recorder to send results of a provided health check function
		/// </summary>
		/// <param name="measureName">Unique name for the measure</param>
		/// <param name="function">function to collect the measure</param>
		/// <param name="customData">optional custom data</param>
		/// <returns>disposable apm recorder</returns>
		IHealthCheckRecorder RecordHealthCheck(
			string measureName,
			Func<HealthCheckOperationResult> function,
			object customData = null);

		/// <summary>
		/// Creates APM recorder to measure a timespan
		/// </summary>
		/// <param name="measureName">Unique name for the measure</param>
		/// <param name="customData">optional custom data</param>
		/// <returns>disposable apm recorder</returns>
		ITimerRecorder RecordTimer(
			string measureName,
			object customData = null);
	}
}
