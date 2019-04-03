namespace kCura.PDB.Service.Tests.DatabaseDeployment
{
	using System.Configuration;
	using System.Data.SqlClient;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Interfaces.DatabaseDeployment;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.ScriptInstallation;
	using kCura.PDB.Service.DatabaseDeployment;
	using kCura.PDB.Tests.Common;
	using Moq;
	using NUnit.Framework;

	[TestFixture]
	[Category("Unit")]
	public class ScriptInstallerTests
	{
		[SetUp]
		public void Setup()
		{
			this.databaseDeployer = new Mock<IDatabaseDeployer>();
			this.configurationService = new Mock<IConfigurationService>();
			this.administrationInstallationService = new Mock<IAdministrationInstallationService>();
			this.scriptInstaller = new ScriptInstaller(this.databaseDeployer.Object, this.configurationService.Object, this.administrationInstallationService.Object);
		}

		private Mock<IDatabaseDeployer> databaseDeployer;
		private Mock<IConfigurationService> configurationService;
		private Mock<IAdministrationInstallationService> administrationInstallationService;
		private ScriptInstaller scriptInstaller;

		[Test]
		public void AdminScriptInstall_Success()
		{
			// Arrange
			this.administrationInstallationService.Setup(s => s.InstallScripts(It.Is<GenericCredentialInfo>(ci => ci.UserName == "sa" && ci.Password == "pw")))
				.Returns(new ScriptInstallationResults { Success = true });

			// Act
			this.scriptInstaller.AdminScriptInstall("sa", "pw");

			// Assert
			Assert.Pass("No return result");
		}

		[Test]
		public void AdminScriptInstall_Fail()
		{
			// Arrange
			this.administrationInstallationService.Setup(s => s.InstallScripts(It.Is<GenericCredentialInfo>(ci => ci.UserName == "sa" && ci.Password == "pw")))
				.Returns(new ScriptInstallationResults { Success = false });

			// Act & Assert
			Assert.Throws<System.Exception>(() => this.scriptInstaller.AdminScriptInstall("sa", "pw"));
		}

		[Test]
		public void TestingScriptInstall_Success()
		{
			// Arrange
			var bytes = new byte[] { 1, 2, 3, 4 };
			this.databaseDeployer.Setup(dd => dd.DeployTesting(It.IsAny<string>(), Names.Database.EddsPerformance, bytes))
				.Returns(new MigrationResultSet(true, new[] { new LogMessage(LogSeverity.Info, "success") }));
			this.configurationService.Setup(s => s.GetConnectionStringBuilder("relativity", true)).Returns(new SqlConnectionStringBuilder(DatabaseConstants.TestConnectionString));

			// Act
			this.scriptInstaller.TestingScriptInstall(bytes);

			// Assert
			Assert.Pass("No return result");
		}

		[Test]
		public void TestingScriptInstall_Fail()
		{
			// Arrange
			var bytes = new byte[] { 1, 2, 3, 4 };
			this.databaseDeployer.Setup(dd => dd.DeployTesting(It.IsAny<string>(), Names.Database.EddsPerformance, bytes))
				.Returns(new MigrationResultSet(false, new[] { new LogMessage(LogSeverity.Error, "failure") }));
			this.configurationService.Setup(s => s.GetConnectionStringBuilder("relativity", true)).Returns(new SqlConnectionStringBuilder(DatabaseConstants.TestConnectionString));

			// Act & Assert
			Assert.Throws<System.Exception>(() => this.scriptInstaller.TestingScriptInstall(bytes));
		}
	}
}
