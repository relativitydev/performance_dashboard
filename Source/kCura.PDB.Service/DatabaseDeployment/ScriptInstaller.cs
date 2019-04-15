namespace kCura.PDB.Service.DatabaseDeployment
{
	using System;
	using System.Linq;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Interfaces.DatabaseDeployment;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;

	public class ScriptInstaller
	{
		private readonly IDatabaseDeployer databaseDeployer;
		private readonly IAppSettingsConfigurationService configurationService;
		private readonly IAdministrationInstallationService administrationInstallationService;

		public ScriptInstaller(
			IDatabaseDeployer databaseDeployer,
			IAppSettingsConfigurationService configurationService,
			IAdministrationInstallationService administrationInstallationService)
		{
			this.administrationInstallationService = administrationInstallationService;
			this.databaseDeployer = databaseDeployer;
			this.configurationService = configurationService;
		}

		public void AdminScriptInstall(string username, string password)
		{
			var creds = new GenericCredentialInfo { UserName = username, Password = password };
			var results = this.administrationInstallationService.InstallScripts(creds);

			if (!results.Success)
			{
				throw new Exception($"Failed to install scripts, details: {string.Join("\r\n| ", results.Messages.Select(m => m.Text))}");
			}
		}

		public void TestingScriptInstall(byte[] testingScripts)
		{
			var server = this.configurationService.GetConnectionStringBuilder("relativity").DataSource;
			var results = this.databaseDeployer.DeployTesting(server, Names.Database.EddsPerformance, testingScripts);

			if (!results.Success)
			{
				throw new Exception($"Failed to install testing scripts, details: {string.Join("\r\n| ", results.Messages.Select(m => m.Message))}");
			}
		}

		public void QoSDeployment()
		{
			var server = this.configurationService.GetConnectionStringBuilder("relativity").DataSource;
			var qosResults = this.databaseDeployer.DeployQos(new Server { ServerName = server });
			if (!qosResults.Success)
			{
				throw new Exception($"Failed to install QoS scripts, details: {string.Join("\r\n| ", qosResults.Messages.Select(m => m.Message))}");
			}
		}
	}
}
