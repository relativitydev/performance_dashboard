namespace kCura.PDB.Service.Services
{
    using System.Threading.Tasks;

    using kCura.PDB.Core.Models;

	public interface IApplicationInstallationService
	{
		Task<ApplicationInstallResponse> InstallApplication();
	}
}
