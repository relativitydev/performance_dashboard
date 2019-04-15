namespace kCura.PDB.Core.Interfaces.Apm.Recorders
{
	using System;

	public interface ITimerRecorder : IDisposable
	{
		/// <summary>
		/// Explicitly writes result of recorder to service bus
		/// </summary>
		void Write();

		/// <summary>
		/// Cancels timer operation & prevents it from being written
		/// </summary>
		void Cancel();
	}
}
