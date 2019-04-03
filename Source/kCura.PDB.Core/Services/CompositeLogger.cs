namespace kCura.PDB.Core.Services
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Services;

	public class CompositeLogger : ILogger
	{
		public CompositeLogger(IList<ILogger> loggers)
		{
			this.loggers = loggers.ToList();
		}

		private readonly IList<ILogger> loggers;

		public void LogCritical(string message, List<string> categories)
		{
			this.loggers.ForEach(l => l.LogCritical(message, categories));
		}

		public void LogCritical(string message, string category = null)
		{
			this.loggers.ForEach(l => l.LogCritical(message, category));
		}

		public void LogError(string message, string category = null)
		{
			this.loggers.ForEach(l => l.LogError(message, category));
		}

		public void LogError(string message, Exception ex, string category = null)
		{
			this.loggers.ForEach(l => l.LogError(message, ex, category));
		}

		public void LogError(string message, Exception ex, List<string> categories)
		{
			this.loggers.ForEach(l => l.LogError(message, ex, categories));
		}

		public void LogInformation(string message, string category = null)
		{
			this.loggers.ForEach(l => l.LogInformation(message, category));
		}

		public void LogVerbose(string message, string category = null)
		{
			this.loggers.ForEach(l => l.LogVerbose(message, category));
		}

		public void LogWarning(string message, string category = null)
		{
			this.loggers.ForEach(l => l.LogWarning(message, category));
		}

		public void LogWarning(string message, List<string> categories)
		{
			this.loggers.ForEach(l => l.LogWarning(message, categories));
		}

		public void LogVerbose(string message, List<string> categories)
		{
			this.loggers.ForEach(l => l.LogVerbose(message, categories));
		}

		public void LogInformation(string message, List<string> categories)
		{
			this.loggers.ForEach(l => l.LogInformation(message, categories));
		}

		public void LogError(string message, List<string> categories)
		{
			this.loggers.ForEach(l => l.LogError(message, categories));
		}

		public Task LogVerboseAsync(string message, List<string> categories)
		{
			return this.loggers.Select(l => l.LogVerboseAsync(message, categories))
				.WhenAllStreamed();
		}

		public Task LogVerboseAsync(string message, string category = null)
		{
			return this.loggers.Select(l => l.LogVerboseAsync(message, category))
				.WhenAllStreamed();
		}

		public Task LogWarningAsync(string message, string category = null)
		{
			return this.loggers.Select(l => l.LogWarningAsync(message, category)).WhenAllStreamed();
		}

		public Task LogWarningAsync(string message, List<string> categories)
		{
			return this.loggers.Select(l => l.LogWarningAsync(message, categories)).WhenAllStreamed();
		}

		public Task LogWarningAsync(string message, Exception ex, List<string> categories)
		{
			return this.loggers.Select(l => l.LogWarningAsync(message, ex, categories)).WhenAllStreamed();
		}

		public Task LogWarningAsync(string message, Exception ex, string category = null)
		{
			return this.loggers.Select(l => l.LogWarningAsync(message, ex, category)).WhenAllStreamed();
		}
	}
}
