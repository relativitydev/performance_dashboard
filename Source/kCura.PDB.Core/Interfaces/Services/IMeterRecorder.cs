namespace kCura.PDB.Core.Interfaces.Services
{
	using System;
	using kCura.PDB.Core.Models;

	public interface IMeterRecorder : IDisposableStopwatch
	{
		void Increment();
	}
}
