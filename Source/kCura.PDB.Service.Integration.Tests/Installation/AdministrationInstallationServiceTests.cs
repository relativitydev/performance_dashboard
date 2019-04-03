namespace kCura.PDB.Service.Integration.Tests.Installation
{
	using kCura.PDB.Core.Models;
	using kCura.PDB.Data.Repositories;
	using kCura.PDB.Service.DatabaseDeployment;
	using kCura.PDB.Service.Installation;
	using kCura.PDB.Service.Services;
	using kCura.PDB.Tests.Common;
	using NUnit.Framework;

	[TestFixture, Category("Integration")]
	public class AdministrationInstallationServiceTests
	{
		private AdministrationInstallationService administrationInstallationService;

		[SetUp]
		public void SetUp()
		{
			var connectionFactory = TestUtilities.GetIntegrationConnectionFactory();
			var logger = TestUtilities.GetMockLogger();
			var databaseDeployer = new DatabaseDeployer(new DatabaseMigratorFactory(connectionFactory),
				new ExceptionMigrationResultHandler(logger.Object));
			var serverRepository = new ServerRepository(connectionFactory);
			var refreshServerService = new RefreshServerService(logger.Object, new ResourceServerRepository(connectionFactory));
			var administrationInstallationRepository = new AdministrationInstallationRepository(connectionFactory);

			this.administrationInstallationService = new AdministrationInstallationService(databaseDeployer, serverRepository, refreshServerService, administrationInstallationRepository);
		}

		[Test, Explicit]
		public void CredentialsAreValid()
		{
			var credentialInfo = TestUtilities.GetSACredentialInfo(); // TestUtilities.GetIntegratedCredentialInfo();
			this.administrationInstallationService.CredentialsAreValid(credentialInfo);
		}

		[Test, Explicit]
		public void InstallScripts()
		{
			var credentialInfo = TestUtilities.GetSACredentialInfo();
			this.administrationInstallationService.InstallScripts(credentialInfo);
		}
	}
}
