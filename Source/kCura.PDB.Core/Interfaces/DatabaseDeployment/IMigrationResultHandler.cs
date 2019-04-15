namespace kCura.PDB.Core.Interfaces.DatabaseDeployment
{
	using kCura.PDB.Core.Models;

	public interface IMigrationResultHandler
	{
		MigrationResultSet HandleDeploymentResponse(MigrationResultSet results);
	}
}
