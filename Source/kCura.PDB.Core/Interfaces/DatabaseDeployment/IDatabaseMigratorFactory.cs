namespace kCura.PDB.Core.Interfaces.DatabaseDeployment
{
	using kCura.PDB.Core.Models;

	public interface IDatabaseMigratorFactory
	{
		IDatabaseMigrator GetResourceMigrator(string serverName, GenericCredentialInfo credentialInfo);

		IDatabaseMigrator GetPerformanceMigrator(string serverName);

		IDatabaseMigrator GetQosDeploymentMigrator(string serverName);

		IDatabaseMigrator GetTestingDeploymentMigrator(string serverName, string database, byte[] testingScripts);
	}
}
