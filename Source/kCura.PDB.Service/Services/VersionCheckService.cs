namespace kCura.PDB.Service.Services
{
	using System;
	using System.Reflection;
	using System.Threading.Tasks;

	using kCura.PDB.Core.Helpers;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;

	public class VersionCheckService : IVersionCheckService
	{
		private readonly ILogger logger;
		private readonly IPdbVersionRepository pdbVersionRepository; // Get from here instead of configuration table (configuration table may not exist yet!)

		public VersionCheckService(IPdbVersionRepository pdbVersionRepository, ILogger logger)
		{
			this.pdbVersionRepository = pdbVersionRepository;
			this.logger = logger;
		}

		public async Task UpdateLatestVersion(Version executingVersion)
		{
			ThrowOn.IsNull(executingVersion, "executingVersion");

			// Init the table if it doesn't exist
			await this.pdbVersionRepository.InitializeIfNotExists();

			// Get the latest recorded version from the environment
			var installedVersion = await this.pdbVersionRepository.GetLatestVersionAsync();

			if (executingVersion < installedVersion)
			{
				throw new Exception($"Cannot update from version {installedVersion} to version {executingVersion}");
			}

			// Update it via repository
			await this.pdbVersionRepository.SetLatestVersionAsync(executingVersion);

			await this.logger.LogVerboseAsync($"Updated from version {installedVersion} to version {executingVersion}");
		}

		public async Task<bool> CurrentVersionIsInstalled(Version executingVersion)
		{
			// Get the latest recorded version from the environment
			var installedVersion = await this.pdbVersionRepository.GetLatestVersionAsync();

			// Compare
			return executingVersion <= installedVersion;
		}
	}
}
