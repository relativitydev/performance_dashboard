namespace kCura.PDB.Data.Repositories
{
	using System.Data;
	using System.Threading.Tasks;
	using Dapper;
	using kCura.PDB.Core.Interfaces.Repositories;

	/// <summary>
	/// To be used sparingly. This is a repository for queries that are not going against any specific data in sql server.
	/// </summary>
	public class GeneralSqlRepository : IGeneralSqlRepository
	{
		/// <summary>
		/// Gets the sql server timezone offset in minutes.
		/// Note: the connection is passed in as a parameter so this operation piggy backs on the current operation and doesn't require an extra connection.
		/// </summary>
		/// <param name="connection">An active connection</param>
		/// <returns>Task of timezone offset in minutes</returns>
		public async Task<int> GetSqlTimezoneOffset(IDbConnection connection) =>
			await connection.QueryFirstOrDefaultAsync<int>("select DATEDIFF(MINUTE, getutcdate(), getdate())");
	}
}
