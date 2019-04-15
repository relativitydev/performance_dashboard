namespace kCura.PDB.Core.Interfaces.Repositories
{
	using kCura.PDB.Core.Models;

	public interface IAdministrationInstallationRepository
	{
		bool HasDbccPermissions(string targetServer, GenericCredentialInfo credentialInfo);

		void InstallPrimaryServerAdminScripts(GenericCredentialInfo credentialInfo);

		void UpdateAdminScriptsRun();
	}
}
