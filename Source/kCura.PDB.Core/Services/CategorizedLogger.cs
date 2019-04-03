namespace kCura.PDB.Core.Services
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Interfaces.Services;

	public class CategorizedLogger : ILogger
	{
		public CategorizedLogger(ILogger logger, string logCategory)
		{
			this.logger = logger;
			this.logCategory = logCategory;
		}

		private readonly ILogger logger;
		private readonly string logCategory;

		public void LogCritical(string message, List<string> categories)
		{
			var tmpCategories = new List<string>() { this.logCategory };
			if (categories != null)
				tmpCategories.AddRange(categories);
			this.logger.LogCritical(message, tmpCategories);
		}

		public void LogCritical(string message, string category = null)
		{
			var tmpCategories = new List<string>() { this.logCategory };
			if (category != null)
				tmpCategories.Add(category);
			this.logger.LogCritical(message, tmpCategories);
		}

		public void LogError(string message, string category = null)
		{
			var tmpCategories = new List<string>() { this.logCategory };
			if (category != null)
				tmpCategories.Add(category);
			this.logger.LogError(message, tmpCategories);
		}

		public void LogError(string message, Exception ex, string category = null)
		{
			var tmpCategories = new List<string>() { this.logCategory };
			if (category != null)
				tmpCategories.Add(category);
			this.logger.LogError(message, ex, tmpCategories);
		}

		public void LogError(string message, Exception ex, List<string> categories)
		{
			var tmpCategories = new List<string>() { this.logCategory };
			if (categories != null)
				tmpCategories.AddRange(categories);
			this.logger.LogError(message, ex, tmpCategories);
		}

		public void LogInformation(string message, string category = null)
		{
			var tmpCategories = new List<string>() { this.logCategory };
			if (category != null)
				tmpCategories.Add(category);
			this.logger.LogInformation(message, tmpCategories);
		}

		public void LogVerbose(string message, string category = null)
		{
			var tmpCategories = new List<string>() { this.logCategory };
			if (category != null)
				tmpCategories.Add(category);
			this.logger.LogVerbose(message, tmpCategories);
		}

		public void LogWarning(string message, string category = null)
		{
			var tmpCategories = new List<string>() { this.logCategory };
			if (category != null)
				tmpCategories.Add(category);
			this.logger.LogWarning(message, tmpCategories);
		}

		public void LogWarning(string message, List<string> categories)
		{
			var tmpCategories = new List<string>() { this.logCategory };
			if (categories != null)
				tmpCategories.AddRange(categories);
			this.logger.LogWarning(message, tmpCategories);
		}

		public void LogVerbose(string message, List<string> categories)
		{
			var tmpCategories = new List<string>() { this.logCategory };
			if (categories != null)
				tmpCategories.AddRange(categories);
			this.logger.LogVerbose(message, tmpCategories);
		}

		public void LogInformation(string message, List<string> categories)
		{
			var tmpCategories = new List<string>() { this.logCategory };
			if (categories != null)
				tmpCategories.AddRange(categories);
			this.logger.LogInformation(message, tmpCategories);
		}

		public void LogError(string message, List<string> categories)
		{
			var tmpCategories = new List<string>() { this.logCategory };
			if (categories != null)
				tmpCategories.AddRange(categories);
			this.logger.LogError(message, tmpCategories);
		}

		public Task LogVerboseAsync(string message, List<string> categories)
		{
			var tmpCategories = new List<string>() { this.logCategory };
			if (categories != null)
				tmpCategories.AddRange(categories);
			return this.logger.LogVerboseAsync(message, tmpCategories);
		}

		public Task LogVerboseAsync(string message, string category = null)
		{
			var tmpCategories = new List<string>() { this.logCategory };
			if (category != null)
				tmpCategories.Add(category);
			return this.logger.LogVerboseAsync(message, tmpCategories);
		}

		public Task LogWarningAsync(string message, string category = null)
		{
			var tmpCategories = new List<string>() { this.logCategory };
			if (category != null)
				tmpCategories.Add(category);
			return this.logger.LogWarningAsync(message, tmpCategories);
		}

		public Task LogWarningAsync(string message, List<string> categories)
		{
			var tmpCategories = new List<string>() { this.logCategory };
			if (categories != null)
				tmpCategories.AddRange(categories);
			return this.logger.LogWarningAsync(message, tmpCategories);
		}

		public Task LogWarningAsync(string message, Exception ex, List<string> categories)
		{
			var tmpCategories = new List<string>() { this.logCategory };
			if (categories != null)
				tmpCategories.AddRange(categories);
			return this.logger.LogWarningAsync(message, ex, tmpCategories);
		}

		public Task LogWarningAsync(string message, Exception ex, string category = null)
		{
			var tmpCategories = new List<string>() { this.logCategory };
			if (category != null)
				tmpCategories.Add(category);
			return this.logger.LogWarningAsync(message, ex, tmpCategories);
		}
	}
}
