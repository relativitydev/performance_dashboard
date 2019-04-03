namespace kCura.PDB.Core.Interfaces.DatabaseDeployment
{
	using kCura.PDB.Core.Models;

	public interface IDatabaseMigrator
	{
		MigrationResultSet ReDeployScripts();

		MigrationResultSet Deploy();
	}
}
