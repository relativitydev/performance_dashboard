namespace kCura.PDB.Service.Apm.Recorders
{
	using global::Relativity.Telemetry.APM;
	using kCura.PDB.Core.Interfaces.Apm.Recorders;

	public class HealthCheckRecorder : IHealthCheckRecorder
	{
		// flags to prevent multiple writes or multiple disposals
		private readonly IHealthMeasure healthCheck;
		private bool disposed;
		private bool written;

		public HealthCheckRecorder(IHealthMeasure healthCheckMeasure)
		{
			this.healthCheck = healthCheckMeasure;
		}

		public void Dispose()
		{
			if (!this.disposed)
			{
				// write metric before disposal
				this.Write();

				// mark as disposed
				this.disposed = true;
			}
		}

		public void Write()
		{
			if (!this.written)
			{
				this.healthCheck?.Write();
				this.written = true;
			}
		}
	}
}
