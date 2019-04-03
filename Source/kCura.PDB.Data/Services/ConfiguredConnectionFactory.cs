namespace kCura.PDB.Data.Services
{
	using System;
	using System.Data.SqlClient;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;

	public class ConfiguredConnectionFactory : CachedConnectionStringFactory
	{
		public ConfiguredConnectionFactory(IAppSettingsConfigurationService configService)
			: base(new ConfiguredWorkspaceServerProvider(configService))
		{
			this.configService = configService;
		}

		private readonly IAppSettingsConfigurationService configService;

		protected override SqlConnectionStringBuilder GetConnectionString(string server = null, GenericCredentialInfo credentialInfo = null)
		{
			var builder = this.configService.GetConnectionStringBuilder("relativity");
			if (builder == null)
			{
				throw new Exception("There is no connection string or connection information configured.");
			}

			if (string.IsNullOrEmpty(server) == false)
				builder.DataSource = server;
			builder.ApplicationName = $"{Names.Application.PerformanceDashboard} Configured";
			return builder
				.ModifyCreditentals(credentialInfo)
				.AddDefaultTimeout();
		}

		
	}
}
