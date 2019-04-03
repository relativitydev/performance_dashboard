namespace kCura.PDB.Core.Interfaces.Repositories
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.RecoverabilityIntegrity;

	public interface IDatabaseRepository
	{
		/// <summary>
		/// Reads the database
		/// </summary>
		/// <param name="databaseId">Reads the database by it's Id</param>
		/// <returns>Task of database</returns>
		Task<Database> ReadAsync(int databaseId);

		/// <summary>
		/// Reads the databases for a given server from the eddsdbo.Database table
		/// </summary>
		/// <param name="serverId">Id for the server</param>
		/// <returns>Task of list of databases</returns>
		Task<IList<Database>> ReadByServerIdAsync(int serverId);

		/// <summary>
		/// Gets the databases for a given server. This actually checks the server and queries the databases on that server and doesn't look in the eddsdbo.Database table
		/// </summary>
		/// <param name="server">The server</param>
		/// <returns>Task of list of database names</returns>
		Task<IList<string>> GetByServerAsync(Server server);

		/// <summary>
		/// Creates a databases
		/// </summary>
		/// <param name="database">Database model</param>
		/// <returns>Task of databases</returns>
		Task<Database> CreateAsync(Database database);

		/// <summary>
		/// Updates a databases
		/// </summary>
		/// <param name="database">Database model</param>
		/// <returns>Task</returns>
		Task UpdateAsync(Database database);

		/// <summary>
		/// Reads the time of the most out of date database by the last activity
		/// </summary>
		/// <param name="server">Query filters by server</param>
		/// <param name="activityType">Filters by dbcc/backups</param>
		/// <returns>Date of the most out of date activity</returns>
		Task<DateTime?> ReadMostOutOfDateActivityByServerAsync(Server server, GapActivityType activityType);

		/// <summary>
		/// Reads the databases that are out of date by the last activity (ie. if the database has an out of date dbcc)
		/// </summary>
		/// <param name="server">Query filters by server</param>
		/// <param name="windowExceededDate">Filters for last recorded activities less than the windows exceeded by</param>
		/// <param name="activityType">Filters by dbcc/backups</param>
		/// <returns>Task of list of databases that are out of date</returns>
		Task<IList<Database>> ReadOutOfDateDatabasesAsync(Server server, DateTime windowExceededDate, GapActivityType activityType);

		/// <summary>
		/// Reads the count of databases by server
		/// </summary>
		/// <param name="server">Query filters databases by server</param>
		/// <returns>Task of 0 or greater for the database count</returns>
		Task<int> ReadCountByServerAsync(Server server);
	}
}
