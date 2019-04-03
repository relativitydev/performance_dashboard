namespace kCura.PDB.Data.Tests.Repositories
{
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Data.Repositories;
	using kCura.PDB.Data.Services;
	using kCura.PDB.Service.Services;
	using NUnit.Framework;
	using PDB.Tests.Common;

	[TestFixture, Category("Integration")]
	public class PrimarySqlServerRepositoryTests
	{
		[SetUp]
		public void SetUp()
		{
			this.connectionFactory = TestUtilities.GetIntegrationConnectionFactory();
			this.primarySqlServerRepository = new PrimarySqlServerRepository(this.connectionFactory);
		}

		private IPrimarySqlServerRepository primarySqlServerRepository;
		private IConnectionFactory connectionFactory;

		[Test]
		public void GetPrimarySqlServer_Success()
		{
			// Act
			var result = primarySqlServerRepository.GetPrimarySqlServer();

			// Assert
			Assert.That(result, Is.Not.Null);
		}
	}
}
