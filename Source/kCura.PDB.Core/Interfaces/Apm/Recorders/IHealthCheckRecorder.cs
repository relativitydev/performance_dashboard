namespace kCura.PDB.Core.Interfaces.Apm.Recorders
{
	using System;

	public interface IHealthCheckRecorder : IDisposable
	{
		/// <summary>
		/// Explicitly writes result of recorder to service bus
		/// </summary>
		void Write();
	}
}
