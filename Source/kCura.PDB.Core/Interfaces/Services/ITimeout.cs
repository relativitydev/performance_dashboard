namespace kCura.PDB.Core.Interfaces.Services
{
	using System;

	public interface ITimeout
	{
		bool IsAfterTimedOut { get; }

		TimeSpan TimeRemaining { get; }

		void Reset();
	}
}
