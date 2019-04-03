namespace kCura.PDB.Service.Tests.DatabaseDeployment
{
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Data.Services;
	using kCura.PDB.Service.DatabaseDeployment;
	using kCura.PDB.Service.Services;
	using kCura.PDB.Tests.Common;
	using NUnit.Framework;

	[TestFixture, Category("Integration")]
	public class DeploymentServiceFactoryTests
	{
		[SetUp]
		public void Setup()
		{
			this.configurationService = new AppSettingsConfigurationService();
			this.connectionFactory = new ConfiguredConnectionFactory(this.configurationService);
		}

		private IConnectionFactory connectionFactory;
		private IAppSettingsConfigurationService configurationService;

		[Test]
		public void GetResourceDeploymentService()
		{
			//Act
			var srv = new DatabaseMigratorFactory(connectionFactory);
			var result = srv.GetResourceMigrator(kCura.PDB.Tests.Common.Config.Server, TestUtilities.GetSACredentialInfo());

			//Assert
			Assert.That(result, Is.Not.Null);
		}

		[Test]
		public void GetPerformanceDeploymentService()
		{
			//Act
			var srv = new DatabaseMigratorFactory(connectionFactory);
			var result = srv.GetPerformanceMigrator("primary-sql-server");

			//Assert
			Assert.That(result, Is.Not.Null);
		}

		[Test]
		public void GetQosDeploymentService()
		{
			//Act
			var srv = new DatabaseMigratorFactory(connectionFactory);
			var result = srv.GetQosDeploymentMigrator(kCura.PDB.Tests.Common.Config.Server);

			//Assert
			Assert.That(result, Is.Not.Null);
		}
	}
}
