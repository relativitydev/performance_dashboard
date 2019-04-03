namespace kCura.PDB.Service.DatabaseDeployment
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Threading;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Interfaces.DatabaseDeployment;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.Deployment;
	using kCura.PDB.Core.Services;
	using kCura.PDB.Data.Repositories;
	using kCura.PDB.Data.Services;
	using kCura.PDB.Service.Services;

	public class DatabaseMigrator : IDatabaseMigrator
	{
		private readonly bool keepWorkingDirectory = true;
		private readonly IRoundHouseFileService fileService;
		private readonly ISqlScriptTokenService sqlScriptTokenService;
		private readonly IRoundhouseManager roundHouseManager;
		private readonly IDeploymentRepository deploymentRepo;
		private readonly IAvailabilityGroupRepository availabilityGroupRepository;
		private readonly DeploymentSettings deploymentSettings;
		private readonly IConnectionFactory connectionFactory;
		private readonly IConfigurationRepository configurationRepository; // used to get Timeout
		private readonly ITimeService timeService;
		private string cachedWorkingDirectory;

		public DatabaseMigrator(DeploymentSettings deploymentSettings, IConnectionFactory connectionFactory)
		{
			this.deploymentSettings = deploymentSettings;
			this.connectionFactory = connectionFactory;

			// Instantiate everything else
			this.keepWorkingDirectory = false;
			roundHouseManager = new RoundhouseManager();
			fileService = new RoundHouseFileService();
			availabilityGroupRepository = new AvailabilityGroupRepository(connectionFactory);
			deploymentRepo = new DeploymentRepository(connectionFactory);
			sqlScriptTokenService = new SqlScriptTokenService(new SqlScriptTokenValueProvider(deploymentRepo, this.deploymentSettings.Server));
			this.configurationRepository = new ConfigurationRepository(connectionFactory);
			this.timeService = new TimeService();
		}

		public DatabaseMigrator(
			DeploymentSettings deploymentSettings,
			IConnectionFactory connectionFactory,
			ISqlScriptTokenService sqlScriptTokenService,
			IRoundhouseManager roundhouseManager,
			IDeploymentRepository deploymentRepo,
			IAvailabilityGroupRepository availabilityGroupRepository,
			IRoundHouseFileService roundHouseFileService,
			ITimeService timeService)
		{
			this.deploymentSettings = deploymentSettings;
			this.connectionFactory = connectionFactory;
			this.fileService = roundHouseFileService;
			this.sqlScriptTokenService = sqlScriptTokenService;
			this.roundHouseManager = roundhouseManager;
			this.deploymentRepo = deploymentRepo;
			this.availabilityGroupRepository = availabilityGroupRepository;
			this.timeService = timeService;
		}

		/// <summary>
		/// Unzip the migration files, replace storage paths, and run deployment
		/// </summary>
		/// <returns>result set</returns>
		public MigrationResultSet Deploy()
		{
			var workingDirectory = FetchZippedResource();
			var workingDirectoryScripts = Directory.GetFiles(workingDirectory, "*.sql", SearchOption.AllDirectories);
			sqlScriptTokenService.Replace(workingDirectoryScripts);
			var result = new MigrationResultSet() { Success = true, Messages = new List<LogMessage>() };
			RunCreateDatabaseScript(workingDirectory, result);
			if (result.Success)
			{
				var rhResult = Execute(workingDirectory);
				foreach (var rhResultsMessage in rhResult.Messages)
				{
					result.Messages.Add(rhResultsMessage);
				}

				result.Success = result.Success && rhResult.Success;
				return result;
			}
			else
			{
				return result;
			}
		}

		public MigrationResultSet ReDeployScripts()
		{
			var result = new MigrationResultSet
			{
				Success = true,
				Messages = new List<LogMessage>()
			};

			if (string.IsNullOrEmpty(cachedWorkingDirectory))
				cachedWorkingDirectory = FetchZippedResource();

			var workingDirectoryScripts = Directory.GetFiles(cachedWorkingDirectory, "*.sql", SearchOption.AllDirectories);
			sqlScriptTokenService.Replace(workingDirectoryScripts);

			try
			{
				var spocsScriptPath = Path.Combine(cachedWorkingDirectory, DeploymentDirectoryStructureConstants.SprocsFolderName);
				if (Directory.Exists(spocsScriptPath))
				{
					var msg = new LogMessage(LogSeverity.Info, $"Redeploying stored procedures {spocsScriptPath}");
					result.Messages.Add(msg);
					deploymentRepo.RunSqlScripts(deploymentSettings, spocsScriptPath, result);
				}

				var functionsScriptPath = Path.Combine(cachedWorkingDirectory, DeploymentDirectoryStructureConstants.FunctionsFolderName);
				if (Directory.Exists(functionsScriptPath))
				{
					var msg = new LogMessage(LogSeverity.Info, $"Redeploying stored procedures {functionsScriptPath}");
					result.Messages.Add(msg);
					deploymentRepo.RunSqlScripts(deploymentSettings, functionsScriptPath, result);
				}
			}
			catch (Exception ex)
			{
				result.Success = false;
				var msg = new LogMessage(LogSeverity.Error, $"Error redeploying scripts /n {ex.ToString()}");
				result.Messages.Add(msg);
			}

			return result;
		}

		/// <summary>
		/// Initiates deployment and indicates results
		/// </summary>
		/// <param name="workingDirectory">the working directory to that has the scripts</param>
		/// <param name="isRetry">indicates if the current install attempt is a retry attempt</param>
		/// <returns>result set</returns>
		internal MigrationResultSet Execute(string workingDirectory, bool isRetry = false)
		{
			var connectionString = this.connectionFactory.GetTargetConnectionString(
					deploymentSettings.DatabaseName,
					deploymentSettings.Server,
					deploymentSettings.CredentialInfo);

			var timeout = ReadRoundhouseTimeoutValue(this.configurationRepository);
			var results = roundHouseManager.Run(
				connectionString,
				workingDirectory,
				deploymentSettings.CreateScriptName,
				deploymentSettings.DatabaseName,
				deploymentSettings.Server,
				timeout);

			if (isRetry == false
				&& results.Success == false
				&& results.Messages.Any(m =>
					m.Severity == LogSeverity.Error
					&& m.Message.Contains("SqlException")
					&& m.Message.Contains(@"involved in a database mirroring session or an availability group")))
			{
				// remove the database from the availability group and then retry deployment
				var removed = availabilityGroupRepository.RemoveFromAvailabilityGroup(deploymentSettings.DatabaseName);
				if (removed)
					results = this.RetryDeployment(workingDirectory, results);
			}
			else if (isRetry == false
					 && results.Success == false)
			{
				results = this.RetryDeployment(workingDirectory, results);
			}

			if (results.Success)
			{
				// Mute down the logging on Success
				results.Messages = results.Messages.Select(m =>
				{
					// Prepend the messages with their oringinal severity in case we do need them again
					m.Message = $"[{m.Severity}] -- {m.Message}";
					m.Severity = LogSeverity.Debug;
					return m;
				}).ToList();
			}

			//Clean up AppData
			if (!this.keepWorkingDirectory)
				fileService.CleanUpAppDataDirectory();

			//Return migration results
			return results;

		}

		/// <summary>
		/// Re-runs the deployment and merges the result set from the first attempt
		/// </summary>
		/// <param name="workingDirectory">the directory with the scripts</param>
		/// <param name="results">the first attempts result set</param>
		/// <returns>the merged migration result set</returns>
		internal MigrationResultSet RetryDeployment(string workingDirectory, MigrationResultSet results)
		{
			this.timeService.Sleep(DatabaseConstants.DeploymentRetrySleepTimeoutSeconds);

			// execute the deployment again
			var retryResults = this.Execute(workingDirectory, true);

			// mark the success from the retry results overwriting the first attempts success result.
			results.Success = retryResults.Success;
			results.Messages.Add(new LogMessage(LogSeverity.Warning, $"Retrying database deployment."));

			// append the retry result messages to the first attempts messages
			foreach (var retryResultsMessage in retryResults.Messages.ToList())
			{
				results.Messages.Add(retryResultsMessage);
			}

			return results;
		}

		internal void RunCreateDatabaseScript(string workingDirectoryScripts, MigrationResultSet resultSet)
		{
			try
			{
				var createScriptPath = Path.Combine(workingDirectoryScripts, "Create_Database.sql");
				deploymentRepo.RunCreateDatabaseScripts(deploymentSettings, createScriptPath, resultSet);
			}
			catch (Exception ex)
			{
				resultSet.Success = false;
				resultSet.Messages.Add(new LogMessage(LogSeverity.Error, $"Error creating database: {ex.ToString()}"));
			}
		}

		/// <summary>
		/// Retrieves the RHTimeoutSeconds configuration value if it's present, returning a default of 3600 (one hour) if it isn't.
		/// </summary>
		/// <param name="configurationRepository">the configuration repository</param>
		/// <returns>The sql timeout value for round house deployments</returns>
		internal static int ReadRoundhouseTimeoutValue(IConfigurationRepository configurationRepository)
		{
			try
			{
				int timeout;
				var timeoutConfig =
					configurationRepository.ReadConfigurationValue(ConfigurationKeys.Section,
						ConfigurationKeys.RoundhouseTimeoutSeconds);
				if (!string.IsNullOrEmpty(timeoutConfig) && int.TryParse(timeoutConfig, out timeout) && timeout >= 0)
				{
					return timeout;
				}
			}
			catch
			{ /* Not currently logging, will silently swallow exception */
			}

			return Defaults.Database.RoundHouseTimeout;
		}

		/// <summary>
		/// Unzips the zipped resource file, and checks if it exists
		/// </summary>
		/// <returns>the working directory with the scripts in it</returns>
		protected string FetchZippedResource()
		{
			var workingDirectory = fileService.UnzipResourceFile(deploymentSettings.MigrationResource);
			if (!Directory.Exists(workingDirectory))
			{
				throw new Exception("The resource file hasn't been successfully unzipped");
			}

			return workingDirectory;
		}
	}
}
