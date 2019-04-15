namespace kCura.PDB.Core.Interfaces.Services
{
	using System.Threading.Tasks;
	using kCura.PDB.Core.Models;

	public interface IPostInstallService
	{
		ApplicationInstallResponse RunOnce();

		Task<ApplicationInstallResponse> RunEveryTimeAsync();
	}
}
