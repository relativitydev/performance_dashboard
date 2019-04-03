namespace kCura.PDB.Service.DatabaseDeployment
{
	using System.Linq;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.DatabaseDeployment;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;

	public class ErrorOnlyMigrationResultHandler : IMigrationResultHandler
	{
		private readonly ILogger logger;

		public ErrorOnlyMigrationResultHandler(ILogger logger)
		{
			this.logger = logger.WithCategory(Names.LogCategory.DatabaseDeployment);
		}

		public MigrationResultSet HandleDeploymentResponse(MigrationResultSet results)
		{
			var errors = from message in results.Messages
						 where message.Severity != LogSeverity.Debug &&
								(!results.Success || message.Severity != LogSeverity.Info) //Give us more detailed logs when this thing fails
						 select message.ToString();

			foreach (var error in errors)
			{
				this.logger.LogError(error);
			}

			return results;
		}
	}
}
