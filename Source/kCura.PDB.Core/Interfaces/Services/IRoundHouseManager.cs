namespace kCura.PDB.Core.Interfaces.Services
{
	using kCura.PDB.Core.Models;

	public interface 
		IRoundhouseManager
	{
		MigrationResultSet Run(string connection, string workingDirectory, string createScript, string databaseName, string server, int timeout);
	}
}
