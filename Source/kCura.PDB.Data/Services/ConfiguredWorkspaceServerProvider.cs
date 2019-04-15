namespace kCura.PDB.Data.Services
{
	using System;
	using kCura.PDB.Core.Interfaces.Data;
	using kCura.PDB.Core.Interfaces.Services;

	public class ConfiguredWorkspaceServerProvider : IWorkspaceServerProvider
	{
		private readonly IAppSettingsConfigurationService configService;

		public ConfiguredWorkspaceServerProvider(IAppSettingsConfigurationService configService)
		{
			this.configService = configService;
		}

		public string GetWorkspaceServer(int workspaceId)
		{
			var builder = this.configService.GetConnectionStringBuilder("relativityWorkspace", false);
			if (builder == null)
			{
				// Fallback to normal relativity connection string
				builder = this.configService.GetConnectionStringBuilder("relativity");
				if (builder == null)
				{
					throw new Exception("There is no connection string or connection information configured.");
				}
			}

			return builder.DataSource;
		}
	}
}
