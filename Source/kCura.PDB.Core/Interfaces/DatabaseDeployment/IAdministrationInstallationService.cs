namespace kCura.PDB.Core.Interfaces.DatabaseDeployment
{
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.ScriptInstallation;

	public interface IAdministrationInstallationService
	{
		bool CredentialsAreValid(GenericCredentialInfo credentialInfo);

		ScriptInstallationResults InstallScripts(GenericCredentialInfo credentialInfo);
	}
}
