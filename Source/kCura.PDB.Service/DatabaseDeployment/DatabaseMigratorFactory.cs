namespace kCura.PDB.Service.DatabaseDeployment
{
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Interfaces.DatabaseDeployment;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.Deployment;
	using kCura.PDB.Service.Resources;

	public class DatabaseMigratorFactory : IDatabaseMigratorFactory
	{
		private readonly IConnectionFactory connectionFactory;

		public DatabaseMigratorFactory(IConnectionFactory connectionFactory)
		{
			this.connectionFactory = connectionFactory;
		}

		public IDatabaseMigrator GetResourceMigrator(string serverName, GenericCredentialInfo credentialInfo)
		{
			var settings = GetDeploymentSettings(serverName, Names.Database.PdbResource);
			settings.MigrationResource = DatabaseDeploymentConfiguration.MigrateResource;
			settings.CreateScriptName = DeploymentDirectoryStructureConstants.CreateResouceCustomScript;
			settings.CredentialInfo = credentialInfo;
			return new DatabaseMigrator(settings, connectionFactory);
		}

		public IDatabaseMigrator GetPerformanceMigrator(string serverName)
		{
			var settings = GetDeploymentSettings(serverName, Names.Database.EddsPerformance);
			settings.MigrationResource = DatabaseDeploymentConfiguration.MigratePerformance;
			settings.CreateScriptName = DeploymentDirectoryStructureConstants.CreatePerformanceCustomScript;
			return new DatabaseMigrator(settings, connectionFactory);
		}

		public IDatabaseMigrator GetQosDeploymentMigrator(string serverName)
		{
			var settings = GetDeploymentSettings(serverName, Names.Database.EddsQoS);
			settings.MigrationResource = DatabaseDeploymentConfiguration.MigrateQoS;
			settings.CreateScriptName = DeploymentDirectoryStructureConstants.CreateQoSCustomScript;
			return new DatabaseMigrator(settings, connectionFactory);
		}

		public IDatabaseMigrator GetTestingDeploymentMigrator(string serverName, string database, byte[] testingScripts)
		{
			var settings = GetDeploymentSettings(serverName, database);
			settings.MigrationResource = testingScripts;
			return new DatabaseMigrator(settings, connectionFactory);
		}
		
		internal static DeploymentSettings GetDeploymentSettings(string server, string databaseName, GenericCredentialInfo credentialInfo)
		{
			var settings = GetDeploymentSettings(server, databaseName);
			settings.CredentialInfo = credentialInfo;
			return settings;
		}

		internal static DeploymentSettings GetDeploymentSettings(string server, string databaseName)
		{
			return new DeploymentSettings
			{
				Server = server,
				DatabaseName = databaseName
			};
		}
	}
}
