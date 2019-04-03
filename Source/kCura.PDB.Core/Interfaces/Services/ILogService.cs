namespace kCura.PDB.Core.Interfaces.Services
{
	using System.Collections.Generic;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Models;

	public interface ILogService
	{
		LogLevel GetLogLevel();

		bool ShouldLog(int level, IList<string> logCategories);
	}
}