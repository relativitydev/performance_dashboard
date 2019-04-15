namespace kCura.PDB.Core.Interfaces.Services
{
	using System;

	public interface IDisposableStopwatch : IDisposable
	{
		TimeSpan Elapsed { get; }
	}
}
