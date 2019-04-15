namespace kCura.PDB.Service.DatabaseDeployment
{
	using System;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using roundhouse;
	using roundhouse.infrastructure.app;

	public class RoundhouseManager : IRoundhouseManager
	{
		private static object obj = new object();

		public MigrationResultSet Run(string connection, string workingDirectory, string createScript, string databaseName, string server, int timeout)
		{
			var logger = new RelativityRoundHouseLogger();

			//create migration agent
			var migrationAgent = new Migrate();

			//get default round house settings
			var configuration = migrationAgent.GetConfiguration();
			ApplicationConfiguraton.set_defaults_if_properties_are_not_set(configuration);

			//get the action delegate settings
			var roundHouseService = new RoundHouseSettingsService
			{
				ServerName = server
			};
			var roundhouseConfiguration = roundHouseService.GetRoundHouseSettings(connection, workingDirectory, createScript);

			//register the delegate
			migrationAgent.Set(roundhouseConfiguration);
			if (!string.IsNullOrEmpty(server))
				migrationAgent.Set(x => x.DatabaseName = databaseName);
			migrationAgent.SetCustomLogging(logger);
			migrationAgent.Set(x => x.CommandTimeout = timeout);
			migrationAgent.Set(x => x.CommandTimeoutAdmin = timeout);

			// Update the database
			try
			{
				// Some googling shows that roundhouse may not be threadsafe.
				// This starts a lock context to prevent multiple threads running roundhouse deployments at the same time.
				lock (obj)
				{
					migrationAgent.Run();
				}
			}
			catch (Exception e)
			{
				//This is theoretically already logged by RoundhousE, but if something in RH breaks rather than in SQL, I want to know
				logger.Messages.Add(new LogMessage(LogSeverity.Error, e.ToString()));
				return new MigrationResultSet(false, logger.Messages);
			}

			return new MigrationResultSet(true, logger.Messages);
		}


	}
}
