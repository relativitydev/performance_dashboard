namespace kCura.PDB.Core.Interfaces.Repositories
{
	using System;
	using System.Data;
	using Models.BISSummary.Grids;
	using Models.BISSummary.Models;
	using Models.ScriptInstallation;

	public interface IEnvironmentCheckRepository : IDbRepository
	{
		DataTable GetRecomendations(GridConditions gridConditions, EnvironmentCheckRecommendationFilterConditions filterConditions);

		void SaveServerDetails(EnvironmentCheckServerDetails serverDetails);

		void ExecuteCollectDatabaseDetails(string targetQoSServer);

		DataTable GetServerDetails(GridConditions gridConditions, EnvironmentCheckServerFilterConditions filterConditions, EnvironmentCheckServerFilterOperands filterOperands);

		DataTable GetDatabaseDetails(GridConditions gridConditions, EnvironmentCheckDatabaseFilterConditions filterConditions, EnvironmentCheckDatabaseFilterOperands filterOperands);

		DataTable ExecuteTuningForkSystem(string targetQoSServer);

		void SaveTuningForkSystemData(string serverName, DataTable data);

		bool? ReadCheckIFISettings(DatabaseDirectoryInfo mdfldfDirs);

		void ExecuteTuningForkRelativity();

		void SaveRecommendation(Guid id, String scope, String value);
	}
}
