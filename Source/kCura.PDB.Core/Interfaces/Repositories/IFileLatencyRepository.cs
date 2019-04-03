namespace kCura.PDB.Core.Interfaces.Repositories
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.BISSummary.Grids;
	using kCura.PDB.Core.Models.BISSummary.Models;

	public interface IFileLatencyRepository : IDbRepository
	{
		void ExecuteSaveFileLevelLatencyDetails(string targetQoSServer, string eddsPerformanceServer);

		DataTable GetFileLevelLatencyDetails(GridConditions gridConditions, Dictionary<FileLatency.Columns, String> filterConditions, Dictionary<FileLatency.Columns, FilterOperand> filterOperands);
	}
}
