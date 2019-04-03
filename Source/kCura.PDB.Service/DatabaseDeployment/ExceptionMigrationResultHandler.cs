namespace kCura.PDB.Service.DatabaseDeployment
{
	using System;
	using System.Linq;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.DatabaseDeployment;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;

	public class ExceptionMigrationResultHandler : IMigrationResultHandler
	{
		private readonly ILogger logger;

		public ExceptionMigrationResultHandler(ILogger logger)
		{
			this.logger = logger
				.WithCategory(Names.LogCategory.DatabaseDeployment);
		}

		public MigrationResultSet HandleDeploymentResponse(MigrationResultSet results)
		{
			results.Messages.ForEach(this.LogMessage);

			if (results.Success) return results;

			var errorMessage =
				results.Messages
					.Where(m => m.Severity == LogSeverity.Error)
					.Select(m => m.Message)
					.Join("{0}\r\n{1}")
					.Truncate(5000);

			throw new Exception($"Error redeploying services - {errorMessage}");
		}

		internal void LogMessage(LogMessage message)
		{
			switch (message.Severity)
			{
				case LogSeverity.Fatal:
					this.logger.LogCritical(message.ToString());
					break;
				case LogSeverity.Error:
					this.logger.LogError(message.ToString());
					break;
				case LogSeverity.Warning:
					this.logger.LogWarning(message.ToString());
					break;
				default:
					this.logger.LogVerbose(message.ToString());
					break;
			}
		}
	}
}
