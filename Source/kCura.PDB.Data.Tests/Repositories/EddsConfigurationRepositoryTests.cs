namespace kCura.PDB.Data.Tests.Repositories
{
	using System.Threading.Tasks;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Data.Repositories;
	using kCura.PDB.Tests.Common;
	using NUnit.Framework;

	[TestFixture]
	[Category("Integration")]
	public class EddsConfigurationRepositoryTests
	{
		[OneTimeSetUp]
		public void OneTimeSetup()
		{
			connectionFatory = TestUtilities.GetIntegrationConnectionFactory();
			this.configurationRepository = new ConfigurationRepository(connectionFatory);
		}

		private IConnectionFactory connectionFatory;
		private ConfigurationRepository configurationRepository;

		[Test]
		[TestCase(ConfigurationKeys.Edds.SectionRelativityCore, ConfigurationKeys.Edds.AuditIdQueries)]
		[TestCase(ConfigurationKeys.Section, ConfigurationKeys.CreateAgentsOnInstall)]
		public void ReadEddsConfigurationInfo(string section, string name)
		{
			// Arrange
			// Act
			var result = this.configurationRepository.ReadEddsConfigurationInfo(section, name);

			// Assert
			Assert.That(result, Is.Not.Null);
		}

		[Test]
		[TestCase(ConfigurationKeys.Edds.SectionRelativityCore, ConfigurationKeys.Edds.AuditIdQueries)]
		[TestCase(ConfigurationKeys.Section, ConfigurationKeys.CreateAgentsOnInstall)]
		public async Task ReadEddsConfigurationInfoAsync(string section, string name)
		{
			// Arrange
			// Act
			var result = await this.configurationRepository.ReadEddsConfigurationInfoAsync(section, name);

			// Assert
			Assert.That(result, Is.Not.Null);
		}

		[Test]
		public void GetRelativityConfigurationInfo()
		{
			// Arrange
			// Act
			var result = this.configurationRepository.GetRelativityConfigurationInfo();

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result, Is.Not.Empty);
		}
	}
}
