namespace kCura.PDB.Data.Tests.Repositories
{
	using System;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Data.Repositories;
	using NUnit.Framework;

	[TestFixture]
	[Category("UnitPlatform")]
	[Category("Integration")] // For TeamCity to ignore this test
	public class ConfigurationRepositoryTests
	{
		[OneTimeSetUp]
		public void OneTimeSetup()
		{
			this.configurationRepository = new ConfigurationRepository(ConnectionFactorySetup.ConnectionFactory);
		}

		private ConfigurationRepository configurationRepository;

		[Test]
		public async Task ConfigurationRepository_ReadValueAsync()
		{
			// Arrange
			// Act
			var result = await this.configurationRepository.ReadValueAsync(ConfigurationKeys.LogLevel);

			//Assert
			Assert.That(result, Is.Not.Empty);
		}

		[Test]
		public async Task ConfigurationRepository_ReadValueAsync_type()
		{
			// Arrange
			this.configurationRepository.SetConfigurationValue(ConfigurationKeys.Section, "TEST_bool", "true");

			// Act
			var result = await this.configurationRepository.ReadValueAsync<bool>("TEST_bool");

			//Assert
			Assert.That(result.HasValue, Is.True);
		}

		[Test]
		public void ConfigurationRepository_ReadConfigurationValue()
		{
			// Arrange
			// Act
			var result = this.configurationRepository.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.AssemblyFileVersion);

			//Assert
			Assert.That(result, Is.Not.Empty);
		}

		[Test]
		public void ConfigurationRepository_ReadConfigurationValue_NoValue()
		{
			// Arrange
			// Act
			var result = this.configurationRepository.ReadConfigurationValue(ConfigurationKeys.Section, System.Guid.NewGuid().ToString());

			//Assert
			Assert.That(result, Is.Null);
		}

		[Test]
		public void ConfigurationRepository_ReadConfigurationValue_WithType()
		{
			// Arrange
			// Act
			var result = this.configurationRepository.ReadConfigurationValue<string>(ConfigurationKeys.Section, ConfigurationKeys.AssemblyFileVersion);

			//Assert
			Assert.That(result, Is.Not.Empty);
			Console.WriteLine(result);
		}

		[Test]
		public void ConfigurationRepository_ReadConfigurationValue_WithVersion()
		{
			// Arrange
			this.configurationRepository.SetConfigurationValue(ConfigurationKeys.Section, "TEST_AssemblyFileVersion", "1.2.3.4");

			// Act
			var result = this.configurationRepository.ReadConfigurationValue(ConfigurationKeys.Section, "TEST_AssemblyFileVersion");
			var version = new Version(result);

			//Assert
			Assert.That(version, Is.EqualTo(new Version(1, 2, 3, 4)));
			Console.WriteLine(result);
		}

		[Test]
		public async Task ConfigurationRepository_ReadUnknownConfigurationValue_WithEnum()
		{
			// Arrange
			// Act
			var result = await this.configurationRepository.ReadValueAsync<AuditServiceType>("Some random config that shouldn't exist");

			//Assert
			Assert.That(result.HasValue, Is.False);
			Assert.That(result, Is.Null);
		}

		[Test]
		public void ConfigurationRepository_ReadUnknownConfigurationValue_WithType()
		{
			// Arrange
			// Act
			var result = this.configurationRepository.ReadConfigurationValue<int>(ConfigurationKeys.Section, "SomeTrashConfig");

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result, Is.EqualTo(new int()));
		}

		[Test]
		public void ConfigurationRepository_SetConfigurationValue()
		{
			// Arrange
			// Act
			this.configurationRepository.SetConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.LogLevel, "Errors");

			//Assert
			Assert.Pass();
		}

		[Test]
		[TestCase(ConfigurationKeys.Section, ConfigurationKeys.LogLevel)]
		public void ReadPdbConfigurationInfo(string section, string name)
		{
			// Arrange
			// Act
			var result =
				this.configurationRepository.ReadPdbConfigurationInfo(section, name);

			// Assert
			Assert.That(result, Is.Not.Null);
		}

		[Test]
		[TestCase(ConfigurationKeys.Section, ConfigurationKeys.LogLevel)]
		public async Task ReadPdbConfigurationInfoAsync(string section, string name)
		{
			// Arrange
			// Act
			var result = await this.configurationRepository.ReadPdbConfigurationInfoAsync(section, name);

			// Assert
			Assert.That(result, Is.Not.Null);
		}
	}
}
