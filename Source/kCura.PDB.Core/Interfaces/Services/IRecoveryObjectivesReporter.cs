namespace kCura.PDB.Core.Interfaces.Services
{
	using System.Collections.Generic;
	using System.Threading.Tasks;

	using kCura.PDB.Core.Models.BISSummary.Models;

	public interface IRecoveryObjectivesReporter
	{
		Task UpdateRpoReport(IList<DatabaseRpoScoreData> databaseRpoScoreData);

		Task UpdateRtoReport(IList<DatabaseRtoScoreData> databaseRtoScoreData);
	}
}
