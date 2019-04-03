namespace kCura.PDB.Service.Apm
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using global::Relativity.Telemetry.APM;
	using global::Relativity.Telemetry.APM.Types;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Apm;
	using kCura.PDB.Core.Interfaces.Apm.Recorders;
	using Recorders;

	public class ApmMetricsService : IApmMetricsService
	{
		private static IAPM apm;

		protected IAPM ApmClient
		{
			get
			{
				return apm ?? (apm = Client.APMClient);
			}

			private set
			{
				if (apm == null)
				{
					apm = value;
				}
			}
		}

		public ApmMetricsService()
		{

		}

		public ApmMetricsService(IAPM apmClient)
		{
			this.ApmClient = apmClient;
		}

		/// <inheritdoc />
		public IMeterRecorder RecordMeter(
			string measureName,
			string measurement,
			TimeUnit unitOfMeasure = TimeUnit.Seconds,
			object customData = null)
		{
			var meter = this.ApmClient.MeterOperation(
				measureName,
				measurement,
				rateUnit: unitOfMeasure,
				customData: customData?.ToKeyValuePairs());
			return new MeterRecorder(meter);
		}

		/// <inheritdoc />
		public ICounterRecorder RecordCounter(
			string measureName,
			string unitOfMeasure = "item(s)",
			object customData = null)
		{
			var counter = this.ApmClient.CountOperation(
				measureName,
				unitOfMeasure: unitOfMeasure,
				customData: customData?.ToKeyValuePairs());
			return new CounterRecorder(counter);
		}

		/// <inheritdoc />
		public void RecordGauge<T>(
			string measureName,
			T value,
			string unitOfMeasure = "item(s)",
			string correlationId = null,
			object customData = null)
		{
			var gauge = this.ApmClient.GaugeOperation<T>(
				measureName,
				() => value,
				unitOfMeasure: unitOfMeasure,
				correlationID: correlationId,
				customData: customData?.ToKeyValuePairs());
			gauge.Write();
		}

		/// <inheritdoc />
		public ITimedGaugeRecorder RecordTimedGauge<T>(
			string measureName,
			Func<T> function,
			TimeSpan interval,
			string unitOfMeasure = "item(s)",
			object customData = null)
		{
			var timedGauge = this.ApmClient.TimedGaugeOperation<T>(
				measureName,
				function,
				interval,
				unitOfMeasure: unitOfMeasure,
				customData: customData?.ToKeyValuePairs());
			return new TimedGaugeRecorder(timedGauge);
		}

		/// <inheritdoc />
		public IHealthCheckRecorder RecordHealthCheck(
			string measureName,
			Func<HealthCheckOperationResult> function,
			object customData = null)
		{
			var health = this.ApmClient.HealthCheckOperation(
				measureName,
				function,
				customData: customData?.ToKeyValuePairs());
			return new HealthCheckRecorder(health);
		}

		/// <inheritdoc />
		public ITimerRecorder RecordTimer(
			string measureName,
			object customData = null)
		{
			var timer = this.ApmClient.TimedOperation(
				measureName,
				customData: customData?.ToKeyValuePairs());
			return new TimerRecorder(timer);
		}
	}
}
