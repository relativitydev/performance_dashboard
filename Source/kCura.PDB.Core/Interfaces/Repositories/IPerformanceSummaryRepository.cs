namespace kCura.PDB.Core.Interfaces.Repositories
{
	using System;

	public interface IPerformanceSummaryRepository : IDbRepository
	{
		void LoadApplicationHealthSummary(DateTime processExecDate);

		void LoadErrorHealthDwData(DateTime processExecDate);

		void LoadUserHealthDwData(DateTime processExecDate);
	}
}
