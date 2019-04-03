namespace kCura.PDB.Core.Interfaces.Repositories
{
	using System.Data;
	using System.Threading.Tasks;

	/// <summary>
	/// To be used sparingly. This is a repository for queries that are not going against any specific data in sql server.
	/// </summary>
	public interface IGeneralSqlRepository
	{
		Task<int> GetSqlTimezoneOffset(IDbConnection connection);
	}
}
