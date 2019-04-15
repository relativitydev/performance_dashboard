namespace kCura.PDB.Service.ProcessControls.ProcessTasks.EnvironmentCheck
{
	using System;
	using System.ComponentModel;
	using System.Data;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Enumerations;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.ProcessControls;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Service.ProcessControls.ProcessTasks;

	[Description(EnvironmentCheckSqlConfigTask.ServerInfoTaskDescription)]
	public class EnvironmentCheckSqlConfigTask : BaseProcessControlTask, IProcessControlTask
	{
		public EnvironmentCheckSqlConfigTask(ILogger logger, ISqlServerRepository sqlRepo, int agentId)
			: base(logger, sqlRepo, agentId)
		{

		}

		private const String ServerInfoTaskDescription = "Environment Check SQL Configuration";

		public ProcessControlId ProcessControlID { get { return ProcessControlId.EnvironmentCheckSqlConfig; } }

		public bool Execute(ProcessControl processControl)
		{
			if (SqlRepo.AdminScriptsInstalled() == false)
			{
				LogWarning("Installation of Performance Dashboard is incomplete. Please install the latest scripts from PDB's custom pages.");
				return true;
			}

			var checkIFIEnabledValue = SqlRepo.ConfigurationRepository.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.EnableInstantFileInitializationCheck);
			bool checkIFIEnabled;
			Boolean.TryParse(checkIFIEnabledValue, out checkIFIEnabled);

			Exception firstFailureException = null;
			Server firstFailureServer = null;

			ExecuteForServers(server =>
			{
				if (server.ServerTypeId == (int)ServerType.Database)
				{
					try
					{
						ExecuteTuningForkForServer(server, checkIFIEnabled);
					}
					catch (Exception ex)
					{
						firstFailureServer = firstFailureServer ?? server;
						firstFailureException = firstFailureException ?? ex;
					}
				}
			});

			if (firstFailureException != null)
			{
				var message = firstFailureException.GetExceptionDetails();
				LogError("Failure Running tunning for {0}. Details: {1}", firstFailureServer.ServerName, message);
				processControl.LastErrorMessage = firstFailureException.ToString();
				return false;
			}

			return true;
		}

		public void ExecuteTuningForkForServer(Server server, bool checkIFIEnabled)
		{
			var tuningForkData = SqlRepo.EnvironmentCheckRepository.ExecuteTuningForkSystem(server.ServerName);
			if (checkIFIEnabled)
			{
				GetIFISettings(server, tuningForkData);
			}

			SqlRepo.EnvironmentCheckRepository.SaveTuningForkSystemData(server.ServerName, tuningForkData);
		}

		public void GetIFISettings(Server server, DataTable tuningForkData)
		{
			var mdfldfDirs = SqlRepo.DeploymentRepository.ReadMdfLdfDirectories(server.ServerName);
			var ifiEnabled = SqlRepo.EnvironmentCheckRepository.ReadCheckIFISettings(mdfldfDirs);
			var row = tuningForkData.Rows.Add();
			row["Scope"] = server.ServerName;
			row["name"] = "Instant File Initialization";
			row["description"] = "This permission improves file allocation performance by preventing SQL Server from zeroing out space on disk when allocated.";
			row["Status"] = ifiEnabled.HasValue && ifiEnabled.Value ? "Good" : "Warning"; ;
			row["Recommendation"] = ifiEnabled.HasValue && ifiEnabled.Value ? "None" : "Instant File Initialization should be enabled";
			row["Value"] = ifiEnabled.HasValue && ifiEnabled.Value ? "Enabled" : "Disabled";
			row["Section"] = "SQL Configuration";
			row["Severity"] = ifiEnabled.HasValue && ifiEnabled.Value ? 0 : 50;
		}
	}
}
