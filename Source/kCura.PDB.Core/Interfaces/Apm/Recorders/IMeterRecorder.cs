namespace kCura.PDB.Core.Interfaces.Apm.Recorders
{
	using System;

	public interface IMeterRecorder : IDisposable
	{
		/// <summary>
		/// Marks events over the course a duration meter, ie a file completion when measure file/s download speed
		/// </summary>
		/// <param name="numberOfMarks">number of events to mark, defaults to 1</param>
		void Mark(long numberOfMarks = 1);

		/// <summary>
		/// Explicitly writes result of recorder to service bus
		/// </summary>
		void Write();
	}
}
