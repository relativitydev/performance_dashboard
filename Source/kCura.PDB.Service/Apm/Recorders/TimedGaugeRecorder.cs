namespace kCura.PDB.Service.Apm.Recorders
{
	using global::Relativity.Telemetry.APM;
	using kCura.PDB.Core.Interfaces.Apm.Recorders;

	public class TimedGaugeRecorder : ITimedGaugeRecorder
	{
		// flags to prevent multiple writes or multiple disposals
		private readonly ITimedGaugeMeasure gauge;
		private bool disposed;
		private bool written;

		public TimedGaugeRecorder(ITimedGaugeMeasure gaugeMeasure)
		{
			this.gauge = gaugeMeasure;
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
				this.gauge?.Write();
				this.written = true;
			}
		}
	}
}
