namespace kCura.PDB.Core.Interfaces.Repositories
{
	using System;
	using System.Collections.Generic;
	using kCura.PDB.Core.Models;

	public interface IAnalyticsRepository : IDbRepository
	{
		void SaveAnalyticsRecommendation(Server analyticsServer, Guid id, String value);

		IEnumerable<String> ReadCaatPopTables(int workspaceId);

		IEnumerable<Int32> ReadCaatIndexes(int workspaceId);

		Dictionary<Int32, long> ReadCaatSearchableDocuments(int workspaceId, IEnumerable<String> caatPopTables, IEnumerable<Int32> caatIndexIds);

		Dictionary<Int32, long> ReadCaatTrainingDocuments(int workspaceId, IEnumerable<String> caatPopTables, IEnumerable<Int32> caatIndexIds);
	}
}
