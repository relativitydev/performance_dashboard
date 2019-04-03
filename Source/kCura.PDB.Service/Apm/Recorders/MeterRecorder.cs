namespace kCura.PDB.Service.Apm.Recorders
{
	using global::Relativity.Telemetry.APM;
	using kCura.PDB.Core.Interfaces.Apm.Recorders;

	public class MeterRecorder : IMeterRecorder
	{
		// flags to prevent multiple writes or multiple disposals
		private readonly IMeterMeasure meter;
		private bool disposed;
		private bool written;

		public MeterRecorder(IMeterMeasure meterMeasure)
		{
			this.meter = meterMeasure;
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
				this.meter?.Write();
				this.written = true;
			}
		}

		/// <inheritdoc />
		public void Mark(long numberOfMarks = 1)
		{
			this.meter?.Mark(numberOfMarks);
		}
	}
}
