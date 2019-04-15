namespace kCura.PDB.Service.Integration.Tests.Installation
{
    using System.Threading.Tasks;

    using kCura.PDB.Data.Repositories;
	using kCura.PDB.Service.DatabaseDeployment;
	using kCura.PDB.Service.Installation;
	using kCura.PDB.Service.Logging;
	using kCura.PDB.Service.Services;
	using kCura.PDB.Tests.Common;
	using NUnit.Framework;

	[TestFixture]
	[Category("Integration")]
	public class ApplicationInstallationServiceTests
	{
		private ApplicationInstallationService applicationInstallationService;
		private TextLogger textLogger;

		[SetUp]
		public void SetUp()
		{
			var connectionFactory = TestUtilities.GetIntegrationConnectionFactory();
			var sql = new SqlServerRepository(connectionFactory);
			var hashConverter = new LegacyHashConversionService();
			var tabService = new TabService(new TabRepository(connectionFactory), new RelativityOneService(sql.ConfigurationRepository));
			this.textLogger = new TextLogger();
		    var dbDeployer = new DatabaseDeployer(
		        new DatabaseMigratorFactory(connectionFactory),
		        new ErrorOnlyMigrationResultHandler(this.textLogger));
			this.applicationInstallationService =
				new ApplicationInstallationService(connectionFactory, sql, hashConverter, tabService, dbDeployer, this.textLogger);
			
		}

		[Test]
		public async Task InstallApplication_Test()
		{
			//Arrange
			// Act
			var results = await this.applicationInstallationService.InstallApplication();

			// Assert
			Assert.That(results.Success, Is.True);
		}
	}
}
