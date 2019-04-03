namespace kCura.PDB.Core.Interfaces.Services
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;

	public interface ILogger
	{

		void LogCritical(string message, List<string> categories);

		void LogCritical(string message, string category = null);

		void LogError(string message, List<string> categories);

		void LogError(string message, string category = null);

		void LogError(string message, Exception ex, string category = null);

		void LogError(string message, Exception ex, List<string> categories);

		void LogInformation(string message, List<string> categories);

		void LogInformation(string message, string category = null);

		void LogVerbose(string message, List<string> categories);

		void LogVerbose(string message, string category = null);

		Task LogVerboseAsync(string message, List<string> categories);

		Task LogVerboseAsync(string message, string category = null);

		void LogWarning(string message, List<string> categories);

		void LogWarning(string message, string category = null);
		Task LogWarningAsync(string message, List<string> categories);

		Task LogWarningAsync(string message, string category = null);

		Task LogWarningAsync(string message, Exception ex, List<string> categories);

		Task LogWarningAsync(string message, Exception ex, string category = null);
	}
}