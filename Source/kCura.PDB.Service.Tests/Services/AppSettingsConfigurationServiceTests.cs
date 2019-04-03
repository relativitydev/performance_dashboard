namespace kCura.PDB.Service.Tests.Services
{
	using System;
	using kCura.PDB.Service.Services;
	using NUnit.Framework;

	[TestFixture]
	[Category("Unit")]
	public class AppSettingsConfigurationServiceTests
	{
		[SetUp]
		public void Setup()
		{
			this.configurationService = new AppSettingsConfigurationService();
		}

		private AppSettingsConfigurationService configurationService;

		[Test]
		public void ContainsAppSettingsKey()
		{
			//Act
			var result = this.configurationService.ContainsAppSettingsKey("WorkspaceId");

			//Assert
			Assert.That(result, Is.True);
		}

		[Test]
		public void ContainsAppSettingsKeys()
		{
			//Act
			var result = this.configurationService.ContainsAppSettingsKeys("WorkspaceId");

			//Assert
			Assert.That(result, Is.True);
		}

		[Test]
		public void GetAppSetting()
		{
			//Act
			var result = this.configurationService.GetAppSetting("WorkspaceId");

			//Assert
			Assert.That(result, Is.Not.Empty);
		}

		[Test]
		public void GetConnectionStrings()
		{
			//Act
			var result = this.configurationService.GetConnectionStrings("relativity");

			//Assert
			Assert.That(result, Is.Not.Null);
		}

		[Test]
		public void GetConnectionStringBuilder()
		{
			//Act
			var connectionBuilder = this.configurationService.GetConnectionStringBuilder("relativity");

			//Assert
			Assert.That(connectionBuilder, Is.Not.Null);
		}

		[Test]
		public void GetConnectionStringBuilder_Errors()
		{
			//Act & Assert
			Assert.Throws<Exception>(() => this.configurationService.GetConnectionStringBuilder("ConnectionStringDoesntExist"));
		}

		[Test]
		public void GetConnectionStringBuilder_ErrorsWithoutException()
		{
			//Act
			var connectionBuilder = this.configurationService.GetConnectionStringBuilder("ConnectionStringDoesntExist", false);

			//Assert
			Assert.That(connectionBuilder, Is.Null);
		}
	}
}
