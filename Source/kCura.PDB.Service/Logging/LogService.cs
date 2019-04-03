namespace kCura.PDB.Service.Logging
{
	using System;
	using System.Collections.Generic;
	using System.Data.SqlClient;
	using System.Linq;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using LogLevel = kCura.PDB.Core.Models.LogLevel;

	public class LogService : ILogService
	{
		private readonly IConfigurationRepository configurationRepository;
		private readonly IDictionary<string, int> logCategories;
		private readonly LogLevel logLevel;

		public LogService(IConfigurationRepository configurationRepository, ILogRepository logRepository)
		{
			this.configurationRepository = configurationRepository;
			this.logCategories = logRepository.ReadCategories()?.ToDictionary(c => c.Name.ToLower(), c => c.LogLevel);
			this.logLevel = GetLogLevel();
		}

		public LogLevel GetLogLevel()
		{
			try
			{
				var logLevelConfigValue =
					this.configurationRepository.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.LogLevel);
				Console.WriteLine($"logLevelConfigValue: {logLevelConfigValue ?? "null"}");
				switch (logLevelConfigValue?.ToLower())
				{
					case "error":
					case "errors":
						return LogLevel.Errors;
					case "info":
					case "information":
					case "verbose":
						return LogLevel.Verbose;
					case "none":
					case "neverlog":
						return LogLevel.NeverLog;
					default:
						return LogLevel.Warnings;
				}
			}
			catch (SqlException)
			{
				// ensures that we don't log if we're failing when getting a logger or the database doesn't exist
				return LogLevel.NeverLog;
			}
		}

		public bool ShouldLog(int level, IList<string> logCategories)
		{
			return
				ShouldLogFromLogCategories(level, logCategories)
				|| ShouldLogFromLogLevel(level);
		}

		internal bool ShouldLogFromLogLevel(int level)
		{
			return this.logLevel != LogLevel.NeverLog
			&& (level < 2
				|| (level < 6 && this.logLevel == LogLevel.Warnings)
				|| this.logLevel == LogLevel.Verbose);
		}

		internal bool ShouldLogFromLogCategories(int level, IList<string> logCategories)
		{
			return this.logCategories != null
				&& logCategories != null
				&& logCategories
					.Where(c => c != null)
					.Any(c => this.logCategories.ContainsKey(c.ToLower()) && this.logCategories[c.ToLower()] >= level);
		}
	}
}
