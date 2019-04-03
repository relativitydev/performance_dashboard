namespace kCura.PDB.Core.Interfaces.Apm.Recorders
{
	using System;

	/// <summary>
	/// Counts number of operations
	/// </summary>
	public interface ICounterRecorder : IDisposable
	{
		/// <summary>
		/// Increment count of recorder
		/// </summary>
		void Increment();

		/// <summary>
		/// Decrements count of recorder
		/// </summary>
		void Decrement();

		/// <summary>
		/// Resets count of recorder
		/// </summary>
		void Reset();

		/// <summary>
		/// Explicitly writes result of recorder to service bus
		/// </summary>
		void Write();
	}
}
