using System;
using System.Configuration;
using kCura.PDB.Core.Interfaces.Services;
using kCura.PDB.Data.Services;
using kCura.PDB.Service.Services;
using Moq;
using NUnit.Framework;

namespace kCura.PDB.Data.Tests.Services
{
	using System.Data.SqlClient;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Tests.Common;

	[TestFixture]
	public class ConfiguredConnectionFactoryTests
	{
		[SetUp]
		public void Setup()
		{
			this.configService = new Mock<IAppSettingsConfigurationService>();
			this.configuredConnectionFactory = new ConfiguredConnectionFactory(configService.Object);
		}

		private Mock<IAppSettingsConfigurationService> configService;
		private ConfiguredConnectionFactory configuredConnectionFactory;

		[Test, Category("Unit")]
		public void GetEddsConnection_ConfigString()
		{
			//Arrange
			configService.Setup(c => c.GetConnectionStringBuilder("relativity", true))
				.Returns(new SqlConnectionStringBuilder(DatabaseConstants.TestConnectionString));

			//Act
			var connection = this.configuredConnectionFactory.GetEddsConnection();

			//Assert
			Assert.That(connection.ConnectionString, Is.Not.Empty);
			Assert.That(connection.Database.ToLower(), Is.EqualTo("edds"));
		}

		[Test, Category("Unit")]
		public void GetEddsPerformanceConnection_ConfigString()
		{
			//Arrange
			configService.Setup(c => c.GetConnectionStringBuilder("relativity", true))
				.Returns(new SqlConnectionStringBuilder(DatabaseConstants.TestConnectionString));

			//Act
			var connection = this.configuredConnectionFactory.GetEddsPerformanceConnection();

			//Assert
			Assert.That(connection.ConnectionString, Is.Not.Empty);
			Assert.That(connection.Database.ToLower(), Is.EqualTo("eddsperformance"));
		}

		[Test, Category("Unit")]
		public void GetEddsQosConnection_ConfigString()
		{
			//Arrange
			configService.Setup(c => c.GetConnectionStringBuilder("relativity", true))
				.Returns(new SqlConnectionStringBuilder(DatabaseConstants.TestConnectionString));

			//Act
			var connection = this.configuredConnectionFactory.GetEddsQosConnection();

			//Assert
			Assert.That(connection.ConnectionString, Is.Not.Empty);
			Assert.That(connection.Database.ToLower(), Is.EqualTo("eddsqos"));
		}

		[Test, Category("Integration")]
		public void GetEddsConnection_Int()
		{
			//Act
			int records;
			using (var connection = this.configuredConnectionFactory.GetEddsConnection())
			{
				connection.Open();

				var cmd = connection.CreateCommand();
				cmd.CommandText = "SELECT count(*) FROM [sys].[objects]";
				cmd.CommandType = System.Data.CommandType.Text;
				records = (int)cmd.ExecuteScalar();
			}

			//Assert
			Assert.That(records, Is.GreaterThan(0));
		}

		[Test]
		[Category("Integration")]
		public async Task GetWorkspaceConnection_ConfigString()
		{
			//Arrange

			//Act
			var connection = await this.configuredConnectionFactory.GetWorkspaceConnectionAsync(Config.WorkSpaceId);

			//Assert
			Assert.That(connection.ConnectionString, Is.Not.Empty);
			Assert.That(connection.Database.ToLower(), Is.EqualTo($"edds{Config.WorkSpaceId}"));
		}

		[Test]
		[Category("Integration")]
		public async Task GetWorkspaceConnection_ConfigStringFallback()
		{
			//Arrange

			//Act
			var connection = await this.configuredConnectionFactory.GetWorkspaceConnectionAsync(Config.WorkSpaceId);

			//Assert
			Assert.That(connection.ConnectionString, Is.Not.Empty);
			Assert.That(connection.Database.ToLower(), Is.EqualTo($"edds{Config.WorkSpaceId}"));
		}
	}
}
