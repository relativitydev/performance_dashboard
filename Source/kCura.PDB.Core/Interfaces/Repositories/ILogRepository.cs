namespace kCura.PDB.Core.Interfaces.Repositories
{
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using Models;

	public interface ILogRepository : IDbRepository
	{
		int Create(LogEntry logEntry);

		Task<int> CreateAsync(LogEntry logEntry);

		Task<LogEntry> ReadLastAsync();

		Task<IList<LogEntryFull>> ReadLastAsync(int count, int logLevel);

		IList<LogCategory> ReadCategories();
	}
}
