namespace kCura.PDB.Service.Installation
{
    using System;
    using System.Reflection;
    using System.Threading.Tasks;

    using kCura.PDB.Core.Constants;
    using kCura.PDB.Core.Interfaces.Services;
    using kCura.PDB.Core.Models;
    using kCura.PDB.Service.Interfaces;
    using kCura.PDB.Service.Services;

    public class SingletonDatabaseInstallationService : IParallelizationInstallationService
    {
        private readonly IApplicationInstallationService applicationInstallationService;
        private readonly IVersionCheckService versionCheckService;
        private readonly ITimeService timeService;
        private readonly ILogger logger;

        public SingletonDatabaseInstallationService(
            IApplicationInstallationService applicationInstallationService,
            IVersionCheckService versionCheckService,
            ITimeService timeService,
            ILogger logger)
        {
            this.applicationInstallationService = applicationInstallationService;
            this.versionCheckService = versionCheckService;
            this.timeService = timeService;
            this.logger = logger;
        }

        public async Task<ApplicationInstallResponse> InstallApplication(int activeWorkspaceId)
        {
            var response = new ApplicationInstallResponse
            {
                Success = false, // Assume failure until life proves rosy and beautiful
                Message = string.Empty
            };

            // This service implementation cares about the version
            var executingVersion = Assembly.GetExecutingAssembly().GetName().Version;

            // workspaceId branching
            if (activeWorkspaceId > 0)
            {
                response.Message =
                    $"Failed to see latest version installed.  Current version: {executingVersion}";

                // Workspace install
                // Setup a timeout for the install
                var installTimeout = this.timeService.GetUtcNow().AddSeconds(Defaults.Database.RoundHouseTimeout);

                // We're gonna read the latest version.  Once the latest version matches the one we are, succeed.  Else fail.
                do
                {
                    var currentVersionInstalled =
                        await this.versionCheckService.CurrentVersionIsInstalled(executingVersion);

                    if (currentVersionInstalled)
                    {
                        response.Success = true;
                        break;
                    }

                    this.timeService.Sleep(TimeSpan.FromSeconds(5));
                }
                while (this.timeService.GetUtcNow() < installTimeout);
            }
            else
            {
                // Admin case -- install as normal
                response = await this.applicationInstallationService.InstallApplication();
                if (!response.Success)
                {
                    return response;
                }

                // Update pdb version
                try
                {
                    await this.versionCheckService.UpdateLatestVersion(executingVersion);
                }
                catch (Exception ex)
                {
                    this.logger.LogError("Failed to update version.", ex);
                }
            }

            return response;
        }
    }
}
