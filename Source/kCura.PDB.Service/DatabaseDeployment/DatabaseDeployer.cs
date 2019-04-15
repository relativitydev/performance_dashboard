namespace kCura.PDB.Service.DatabaseDeployment
{
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.DatabaseDeployment;
	using kCura.PDB.Core.Models;

	public class DatabaseDeployer : IDatabaseDeployer
	{
		private readonly IDatabaseMigratorFactory databaseMigratorFactory;
		private readonly IMigrationResultHandler migrationResultHandler;

		public DatabaseDeployer(IDatabaseMigratorFactory databaseMigratorFactory, IMigrationResultHandler migrationResultHandler)
		{
			this.migrationResultHandler = migrationResultHandler;
			this.databaseMigratorFactory = databaseMigratorFactory;
		}

		public MigrationResultSet DeployQos(Server server) =>
			this.databaseMigratorFactory.GetQosDeploymentMigrator(server.ServerName)
				.Deploy()
				.Pipe(this.migrationResultHandler.HandleDeploymentResponse);

		public MigrationResultSet DeployResource(Server server, GenericCredentialInfo credentialInfo) =>
			this.databaseMigratorFactory.GetResourceMigrator(server.ServerName, credentialInfo)
					.Deploy()
					.Pipe(this.migrationResultHandler.HandleDeploymentResponse);

		public MigrationResultSet DeployPerformance(string serverName) =>
			this.databaseMigratorFactory.GetPerformanceMigrator(serverName)
					.Deploy()
					.Pipe(this.migrationResultHandler.HandleDeploymentResponse);

		public MigrationResultSet DeployTesting(string serverName, string database, byte[] testingScripts) =>
			this.databaseMigratorFactory.GetTestingDeploymentMigrator(serverName, database, testingScripts)
					.Deploy()
					.Pipe(this.migrationResultHandler.HandleDeploymentResponse);
	}
}
