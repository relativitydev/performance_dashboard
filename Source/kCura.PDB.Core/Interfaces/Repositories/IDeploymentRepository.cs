namespace kCura.PDB.Core.Interfaces.Repositories
{
	using System;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.Deployment;
	using kCura.PDB.Core.Models.ScriptInstallation;

	public interface IDeploymentRepository : IDbRepository
	{
		DatabaseDirectoryInfo ReadMdfLdfDirectories(string server = "");

		string ReadCollationSettings();

		void RunSqlScripts(DeploymentSettings settings, string spocsScriptPath, MigrationResultSet result);

		void InsertRoundhouseTimeoutSettingIfNotExists();

		void RemoveOldApplicationReferences();

		void RemoveOldApplicationReferencesFromWorkspace(Guid agentGuid, int workspaceId);

		void RemoveOldResourceFiles();

		void RunCreateDatabaseScripts(DeploymentSettings settings, string createScriptPath, MigrationResultSet result);
	}
}