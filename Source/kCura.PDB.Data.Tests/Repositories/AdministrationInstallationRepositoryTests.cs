namespace kCura.PDB.Data.Tests.Repositories
{
	using kCura.PDB.Data.Repositories;
	using kCura.PDB.Tests.Common;
	using NUnit.Framework;

	[TestFixture, Category("Integration")]
	public class AdministrationInstallationRepositoryTests
	{
		private AdministrationInstallationRepository administrationInstallationRepository;

		[SetUp]
		public void SetUp()
		{
			var connectionFactory = TestUtilities.GetIntegrationConnectionFactory();
			this.administrationInstallationRepository = new AdministrationInstallationRepository(connectionFactory);
		}

		[Test]
		public void UpdateAdminScriptsRun()
		{
			this.administrationInstallationRepository.UpdateAdminScriptsRun();
		}
	}
}
