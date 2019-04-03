namespace kCura.PDD.Web.Controllers
{
	using System;
	using System.Net.Http;
	using System.Web.Http;
	using global::Relativity.CustomPages;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models.BISSummary.Models;
	using kCura.PDB.Data.Repositories;
	using kCura.PDB.Data.Services;
	using kCura.PDB.Service.Services;
	using kCura.PDD.Web.Factories;
	using kCura.PDD.Web.Filters;
	using Newtonsoft.Json;

	[AuthenticateUser]
	public class ConfigurationController : ApiController
	{
		private IPdbConfigurationService configurationService;

		public ConfigurationController()
		{
			var helper = ConnectionHelper.Helper();
			var connectionFactory = new HelperConnectionFactory(helper);
			var sqlRepo = new SqlServerRepository(connectionFactory);
			
			this.configurationService = new PdbConfigurationService(sqlRepo, sqlRepo.ConfigurationRepository, sqlRepo.ProcessControlRepository);
		}

		public ConfigurationController(IPdbConfigurationService pdbConfigurationService)
		{
			this.configurationService = pdbConfigurationService;
		}

		[HttpGet]
		public PerformanceDashboardConfigurationSettings GetSettings() => configurationService.GetConfiguration();

		[HttpGet]
		public bool ElevatedScriptsInstalled() => configurationService.ElevatedScriptsInstalled();

		[HttpPost]
		public ValidationResult SaveSettings(HttpRequestMessage config)
		{
			try
			{
				var _authService = new AuthenticationServiceFactory().GetService();
				var parsedConfiguration = JsonConvert.DeserializeObject<PerformanceDashboardConfigurationSettings>(config.Content.ReadAsStringAsync().Result);

				parsedConfiguration.LastModifiedBy = _authService.GetUserId() != -1 ? _authService.GetUserId().ToString() : "Unknown";
				var result = configurationService.ValidateConfiguration(parsedConfiguration);
				if (result.Valid)
				{
					configurationService.SetConfiguration(parsedConfiguration);
				}
				return result;

			}
			catch (Exception ex)
			{
				return new ValidationResult { Valid = false, Details = ex.ToString() };
			}
		}
	}
}
