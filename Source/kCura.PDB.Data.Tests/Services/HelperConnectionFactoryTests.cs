namespace kCura.PDB.Data.Tests.Services
{
	using System;
	using System.Data.SqlClient;
	using System.Threading.Tasks;
	using global::Relativity.API;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Data.Services;
	using Moq;
	using NUnit.Framework;
	using kCura.PDB.Service.Services;

	[TestFixture, Category("Integration")]
	public class HelperConnectionFactoryTests
	{
		[SetUp]
		public void Setup()
		{
			configurationService = new AppSettingsConfigurationService();
			connFactory = new ConfiguredConnectionFactory(configurationService);
			var connString = connFactory.GetEddsConnection().ConnectionString;
			sqlConnection = new SqlConnection(connString);
			// TODO -- Connection string refactor
			kCura.Data.RowDataGateway.Config.SetConnectionString(connString);
			this.helper = new Mock<IHelper>();
			this.helperConnectionFactory = new HelperConnectionFactory(helper.Object);
		}

		private IConnectionFactory connFactory;
		private IAppSettingsConfigurationService configurationService;
		private SqlConnection sqlConnection;
		private Mock<IHelper> helper;
		private HelperConnectionFactory helperConnectionFactory;

		[Test]
		public void HelperConnectionFactoryTests_GetEddsConnection()
		{
			//Arrange
			helper.SetupGet(h => h.GetDBContext(-1).ServerName).Returns(sqlConnection.DataSource);

			//Act
			var result = this.helperConnectionFactory.GetEddsConnection();

			//Assert
			Assert.That(result, Is.Not.Null);
		}

		[Test]
		public void HelperConnectionFactoryTests_GetEddsPerformanceConnection()
		{
			//Arrange
			helper.SetupGet(h => h.GetDBContext(-1).ServerName).Returns(sqlConnection.DataSource);

			//Act
			var result = this.helperConnectionFactory.GetEddsPerformanceConnection();

			//Assert
			Assert.That(result, Is.Not.Null);
		}

		[Test]
		public void HelperConnectionFactoryTests_GetEddsQosConnection()
		{
			//Arrange
			helper.SetupGet(h => h.GetDBContext(-1).ServerName).Returns(sqlConnection.DataSource);

			//Act
			var result = this.helperConnectionFactory.GetEddsQosConnection();

			//Assert
			Assert.That(result, Is.Not.Null);
		}

		[Test]
		public void HelperConnectionFactoryTests_GetWorkspaceConnection()
		{
			//Arrange
			var workspaceId = Convert.ToInt32(configurationService.GetAppSetting("WorkspaceId"));

			helper.SetupGet(h => h.GetDBContext(-1).ServerName).Returns(sqlConnection.DataSource);

			//Act
			var result = this.helperConnectionFactory.GetWorkspaceConnection(workspaceId);

			//Assert
			Assert.That(result, Is.Not.Null);
		}

		[Test]
		public async Task HelperConnectionFactoryTests_GetWorkspaceConnectionAsync()
		{
			//Arrange
			var workspaceId = Convert.ToInt32(configurationService.GetAppSetting("WorkspaceId"));

			helper.SetupGet(h => h.GetDBContext(-1).ServerName).Returns(sqlConnection.DataSource);

			//Act
			var result = await this.helperConnectionFactory.GetWorkspaceConnectionAsync(workspaceId);

			//Assert
			Assert.That(result, Is.Not.Null);
		}
	}
}
