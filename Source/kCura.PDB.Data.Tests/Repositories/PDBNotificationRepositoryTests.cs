namespace kCura.PDB.Data.Tests.Repositories
{
	using System.Data.SqlClient;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Data.Repositories;
	using kCura.PDB.Tests.Common;
	using NUnit.Framework;

	[TestFixture, Category("Integration")]
	public class PDBNotificationRepositoryTests
	{
		private IConnectionFactory connectionFactory;
		private ISqlServerRepository sqlServerRepository;

		[SetUp]
		public void SetUp()
		{
			this.connectionFactory = TestUtilities.GetIntegrationConnectionFactory();
			this.sqlServerRepository = new SqlServerRepository(this.connectionFactory);
		}

		[Test]
		public void GetAgentsAlert()
		{
			//Arrange
			//Act
			var dt = this.sqlServerRepository.PDBNotificationRepository.GetAgentsAlert();

			//Assert
			Assert.That(dt.Rows, Is.Not.Empty);
		}

		[Test]
		public void GetFailingProcessControls()
		{
			//Arrange
			//Act
			var dt = this.sqlServerRepository.PDBNotificationRepository.GetFailingProcessControls();

			//Assert
			Assert.That(dt.Rows, Is.Not.Empty);
		}
	}
}
