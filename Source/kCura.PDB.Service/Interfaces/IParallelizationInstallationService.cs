namespace kCura.PDB.Service.Interfaces
{
    using System.Threading.Tasks;

    using kCura.PDB.Core.Models;

    public interface IParallelizationInstallationService
    {
        Task<ApplicationInstallResponse> InstallApplication(int activeCaseId);
    }
}
