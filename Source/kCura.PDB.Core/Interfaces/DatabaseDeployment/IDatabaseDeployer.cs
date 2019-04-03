namespace kCura.PDB.Core.Interfaces.DatabaseDeployment
{
	using kCura.PDB.Core.Models;

	public interface IDatabaseDeployer
	{
		MigrationResultSet DeployQos(Server server);

		MigrationResultSet DeployResource(Server server, GenericCredentialInfo credentialInfo);

		MigrationResultSet DeployPerformance(string serverName);

		MigrationResultSet DeployTesting(string serverName, string database, byte[] testingScripts);
	}
}
