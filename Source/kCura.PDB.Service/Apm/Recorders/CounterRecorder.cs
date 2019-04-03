namespace kCura.PDB.Service.Apm.Recorders
{
	using global::Relativity.Telemetry.APM;
	using kCura.PDB.Core.Interfaces.Apm.Recorders;

	public class CounterRecorder : ICounterRecorder
	{
		// flags to prevent multiple writes or multiple disposals
		private readonly ICounterMeasure counter;
		private bool disposed;
		private bool written;

		public CounterRecorder(ICounterMeasure meterMeasure)
		{
			this.counter = meterMeasure;
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
				this.counter?.Write();
				this.written = true;
			}
		}

		/// <inheritdoc />
		public void Increment()
		{

		}

		/// <inheritdoc />
		public void Decrement()
		{

		}

		/// <inheritdoc />
		public void Reset()
		{

		}
	}
}
