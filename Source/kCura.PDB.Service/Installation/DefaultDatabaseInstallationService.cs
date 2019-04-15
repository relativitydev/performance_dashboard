namespace kCura.PDB.Service.Installation
{
    using System.Threading.Tasks;

    using kCura.PDB.Core.Models;
    using kCura.PDB.Service.Interfaces;
    using kCura.PDB.Service.Services;

    public class DefaultDatabaseInstallationService : IParallelizationInstallationService
    {
        private readonly IApplicationInstallationService applicationInstallationService;

        public DefaultDatabaseInstallationService(IApplicationInstallationService applicationInstallationService)
        {
            this.applicationInstallationService = applicationInstallationService;
        }

        public Task<ApplicationInstallResponse> InstallApplication(int activeCaseId)
        {
            return this.applicationInstallationService.InstallApplication();
        }
    }
}
