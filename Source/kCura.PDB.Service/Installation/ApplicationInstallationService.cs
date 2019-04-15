namespace kCura.PDB.Service.Installation
{
	using System;
	using System.Data.SqlClient;
	using System.Linq;
	using System.Threading.Tasks;

	using kCura.PDB.Core.Interfaces.DatabaseDeployment;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Service.Logging;
	using kCura.PDB.Service.Services;

	public class ApplicationInstallationService : IApplicationInstallationService
	{
		private readonly IConnectionFactory connectionFactory;
		private readonly ISqlServerRepository sqlRepository;
		private readonly IHashConversionService hashConverter;
		private readonly ITabService tabService;
		private readonly IDatabaseDeployer databaseDeployer;
        private readonly TextLogger logger; 

		public ApplicationInstallationService(IConnectionFactory connectionFactory, ISqlServerRepository sqlServerRepository, IHashConversionService hashConversionService, ITabService tabService, IDatabaseDeployer databaseDeployer, TextLogger textLogger)
		{
			this.connectionFactory = connectionFactory;
			this.sqlRepository = sqlServerRepository;
			this.hashConverter = hashConversionService;
			this.tabService = tabService;
			this.databaseDeployer = databaseDeployer;
		    this.logger = textLogger;
		}

		public async Task<ApplicationInstallResponse> InstallApplication()
		{
			var response = new ApplicationInstallResponse
			{
				Success = false, // Assume failure until life proves rosy and beautiful
				Message = string.Empty
			};

			try
			{
				// Initialize
				using (var conn = (SqlConnection)this.connectionFactory.GetEddsConnection())
				{
					// Upgrade pre-RH databases for RH compatibility
					await this.logger.LogVerboseAsync("Performing legacy EDDSPerformance upgrade if necessary...");
					this.sqlRepository.UpgradeIfMissingRoundhouse();

					var perfExists = this.sqlRepository.PerformanceExists();
					if (perfExists)
					{
						var servers = this.sqlRepository.GetRegisteredSQLServers();
						this.hashConverter.ConvertHashes(this.sqlRepository, servers);
						this.sqlRepository.DeploymentRepository.InsertRoundhouseTimeoutSettingIfNotExists();
					}

                    // log the current server we're deploying performance to
				    await this.logger.LogVerboseAsync($"EDDSPerformance deploying to {conn.DataSource}");

					// Migrate EDDSPerformance
					var results = this.databaseDeployer.DeployPerformance(conn.DataSource);
					if (!results.Success)
					{
						// Deployment of EDDSPerformance failed - stop here
						var errors = results.Messages
							.Where(m => m.Severity != LogSeverity.Debug
							            && (!results.Success || m.Severity != LogSeverity.Info)) // Give us more detailed logs when this thing fails
							.Select(m => m.Message.ToString());

						foreach (var error in errors)
						{
							this.logger.LogError(error);
						}

						response.Message = this.logger.Text;
						return response;
					}

                    // Create tabs
				    await this.logger.LogVerboseAsync($"Deploying tabs");
					this.tabService.CreateApplicationTabs();
				    await this.logger.LogVerboseAsync("Application installation complete");

					// Logging everything that's happening with the install (text logger), last error message sent

					// Result Handler output to database?
				}
			}
			catch (Exception ex)
			{
			    await this.logger.LogVerboseAsync($"Installation failed due to exception: {ex.Message}");
			    await this.logger.LogVerboseAsync($"Exception: {ex.ToString()}");
				response.Message = this.logger.Text;
				return response;
			}

			response.Message = this.logger.Text;
			response.Success = true;
			return response;
		}
	}
}
