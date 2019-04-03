namespace kCura.PDB.Service.ProcessControls
{
	using System;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Enumerations;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;

	public class LookingGlassTaskGroup
	{
		public LookingGlassTaskGroup(ISqlServerRepository sqlServerRepository, ILogger logger)
		{
			this.logger = logger.WithClassName();
			this.sqlServerRepository = sqlServerRepository;
		}

		private readonly ILogger logger;
		private readonly ISqlServerRepository sqlServerRepository;

		private DateTime utcDate = DateTime.UtcNow;

		public void Execute(int agentId)
		{
			logger.LogVerbose("RunLookingGlassTasks Called");

			try
			{
				//query the servers UTC time since it may be off from the server and add an additional 5 seconds just in case
				utcDate = sqlServerRepository.ReadServerUtcTime().AddSeconds(-15);

				if (sqlServerRepository.AdminScriptsInstalled())
				{
					//Collect backup/DBCC data
					this.MonitorBackupDbcc(agentId);
				}
				else
				{
					logger.LogWarning("Installation of Performance Dashboard is incomplete. Please install the latest scripts from PDB's custom pages.");
				}

				logger.LogVerbose("RunLookingGlassTasks Called - Success");

			}
			catch (Exception ex)
			{
				var message = ex.GetExceptionDetails();
				logger.LogError($"RunLookingGlassTasks Called - Failure. Details: {message}");
			}

		}

		internal void MonitorBackupDbcc(int agentId, string sproc = Names.Database.BackupAndDBCCMonLauncherSproc)
		{
			var processControl = this.sqlServerRepository.ProcessControlRepository.ReadById(ProcessControlId.MonitorBackupDBCC);
			if (processControl == null)
			{
				logger.WithCategory("MonitorBackupDBCC").LogVerbose("Skipping due to process control interval.");
				return;
			}

			var execSucceeded = processControl.LastExecSucceeded;
			try
			{
				logger.LogVerbose("MonitorBackupDBCC Called");
				var interval = processControl.Frequency.GetValueOrDefault(1440);

				if (interval > 0 && processControl.LastProcessExecDateTime.AddMinutes(interval) <= utcDate)
				{
					// Refresh eddsdbo.DBCCTarget with current linked servers
					sqlServerRepository.RefreshDbccTargets();

					// If we're configured to purge the backup/DBCC tables, do so before running monitoring
					var purge = sqlServerRepository.ConfigurationRepository.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.PurgeBackupDBCCTables) ?? String.Empty;
					if (purge.ToLower().Equals("true"))
					{
						logger.LogVerbose("MonitorBackupDBCC Called - Purging backup/DBCC tables...");
						sqlServerRepository.PurgeBackupDBCCTables();
						sqlServerRepository.ConfigurationRepository.SetConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.PurgeBackupDBCCTables, "False");
						logger.LogVerbose("MonitorBackupDBCC Called - Purged backup/DBCC tables.");
					}

					// Installation is complete - run the monitoring procedure
					var results = sqlServerRepository.ExecuteBackupDBCCMonitor(agentId, sproc);

					// Failed servers should show up as an error.
					if (results.FailedServers > 0)
						logger.LogError($"MonitorBackupDBCC Failed - Errors monitoring {results.FailedServers} server(s): {results.ServerErrors}");

					// Failed databases should show up as a warning.
					if (results.FailedDatabases > 0)
						logger.LogWarning($"MonitorBackupDBCC Failed - Errors monitoring {results.FailedDatabases} database(s): {results.DatabaseErrors}");

					// If monitoring failed for one or more servers or databases, recovery will most likely require intervention.
					// We don't want this task running every five seconds and spamming the event viewer with errors.
					// (e.g. putting a database in single-user mode)
					processControl.LastProcessExecDateTime = new DateTime(utcDate.Year, utcDate.Month, utcDate.Day, utcDate.Hour, processControl.LastProcessExecDateTime.Minute, 0);
					execSucceeded = true;
				}
				else
				{
					logger.LogVerbose("MonitorBackupDBCC Called - Skipping due to interval");
				}
			}
			catch (Exception ex)
			{
				var message = ex.GetExceptionDetails();
				logger.LogError($"MonitorBackupDBCC Called - Failure. Details: {message}");
				processControl.LastErrorMessage = ex.ToString();
				execSucceeded = false;
			}
			finally
			{
				processControl.LastExecSucceeded = execSucceeded;
				this.sqlServerRepository.ProcessControlRepository.Update(processControl);
			}
		}
	}
}
