namespace kCura.PDB.Service.DatabaseDeployment
{
	using System;
	using System.IO;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Models.Deployment;
	using roundhouse.databases;
	using roundhouse.infrastructure.app;

	public class RoundHouseSettingsService
	{
		#region Server Information

		/// <summary>
		/// -s, --server, --servername, --instance, --instancename=VALUE | 
		/// ServerName - The server and instance you would like to run on. 
		/// (local) and (local)\SQL2008 are both valid values. Defaults to (local).
		/// </summary>
		public String ServerName = string.Empty;

		#endregion

		#region Restoration Information **(NOT IN USE)**

		/// <summary>
		/// --restore | Restore - This instructs RH to do a restore (with the 
		/// restorefrompath parameter) of a database before running migration 
		/// scripts. Defaults to false.
		/// </summary>
		private Boolean Restore = false;

		/// <summary>
		/// --rfp, --restorefrom, --restorefrompath=VALUE | RestoreFromPath - 
		/// This tells the restore where to get to the backed up database. 
		/// Defaults to null. Required if /restore has been set. NOTE: will 
		/// try to use Litespeed for the restore if the last two characters 
		/// of the name are LS (as in DudeLS.bak).
		/// </summary>
		private String RestoreFromPath = null;

		/// <summary>
		/// --rco, --restoreoptions, --restorecustomoptions=VALUE | 
		/// RestoreCustomOptions - This provides the restoreany custom options 
		/// as in MOVE='Somewhere or another'. Take a look at Token Replacement 
		/// to help out with naming.
		/// </summary>
		private String RestoreCustomOptions = String.Empty;

		/// <summary>
		/// --rt, --restoretimeout=VALUE | RestoreTimeout - Allows you to specify
		/// a restore timeout in seconds. The default is 900 seconds.
		/// </summary>
		private Int32 RestoreTimeout = 900;

		#endregion

		public RoundHouseSettingsService()
		{

		}

		public Action<ConfigurationPropertyHolder> GetRoundHouseSettings(String connectionString, string workingDirectory, string createScript)
		{
			Action<ConfigurationPropertyHolder> roundhouseSettings = delegate(ConfigurationPropertyHolder c)
			{
				//connection string
				c.ConnectionString = connectionString;

				//logging that runs through relativity
				c.Logger = new RelativityRoundHouseLogger();

				//Get directory Specific Settings
				LoadDirectorySpecificSettings(workingDirectory, c);

				//Get common settings
				LoadCommonRoundHouseSettings(workingDirectory, c);

			};

			return roundhouseSettings;
		}

		private static void LoadDirectorySpecificSettings(string workingDirectory, ConfigurationPropertyHolder c)
		{
			//directory path
			c.SqlFilesDirectory = workingDirectory;

			//directory settings
			c.AlterDatabaseFolderName = Path.Combine(workingDirectory, DeploymentDirectoryStructureConstants.AlterDatabaseFolderName);
			c.FunctionsFolderName = Path.Combine(workingDirectory, DeploymentDirectoryStructureConstants.FunctionsFolderName);
			c.IndexesFolderName = Path.Combine(workingDirectory, DeploymentDirectoryStructureConstants.IndexesFolderName);
			c.PermissionsFolderName = Path.Combine(workingDirectory, DeploymentDirectoryStructureConstants.PermissionsFolderName);
			c.RunAfterCreateDatabaseFolderName = Path.Combine(workingDirectory, DeploymentDirectoryStructureConstants.RunAfterCreateDatabaseFolderName);
			c.RunAfterOtherAnyTimeScriptsFolderName = Path.Combine(workingDirectory, DeploymentDirectoryStructureConstants.RunAfterOtherAnyTimeScriptsFolderName);
			c.RunBeforeUpFolderName = Path.Combine(workingDirectory, DeploymentDirectoryStructureConstants.RunBeforeUpFolderName);
			c.SprocsFolderName = Path.Combine(workingDirectory, DeploymentDirectoryStructureConstants.SprocsFolderName);
			c.UpFolderName = Path.Combine(workingDirectory, DeploymentDirectoryStructureConstants.UpFolderName);
			c.ViewsFolderName = Path.Combine(workingDirectory, DeploymentDirectoryStructureConstants.ViewsFolderName);
			c.RunFirstAfterUpFolderName = Path.Combine(workingDirectory, DeploymentDirectoryStructureConstants.LegacyFolderName);
		}

		private void LoadCommonRoundHouseSettings(string workingDirectory, ConfigurationPropertyHolder c)
		{
			c.RecoveryMode = RecoveryMode.NoChange;
			c.RepositoryPath = StaticRoundHouseSettings.RepositoryPath;
			c.SchemaName = StaticRoundHouseSettings.SchemaName;
			c.ScriptsRunErrorsTableName = StaticRoundHouseSettings.ScriptsRunErrorsTableName;
			c.ScriptsRunTableName = StaticRoundHouseSettings.ScriptsRunTableName;
			c.SearchAllSubdirectoriesInsteadOfTraverse = StaticRoundHouseSettings.SearchAllSubdirectoriesInsteadOfTraverse;
			c.Silent = StaticRoundHouseSettings.Silent;
			c.RunAllAnyTimeScripts = StaticRoundHouseSettings.RunAllAnyTimeScripts;
			c.VersionTableName = StaticRoundHouseSettings.VersionTableName;
			c.WarnOnOneTimeScriptChanges = StaticRoundHouseSettings.WarnOnOneTimeScriptChanges;
			c.WithTransaction = StaticRoundHouseSettings.WithTransaction;
			c.OutputPath = DeploymentDirectoryStructureConstants.OutputPath;
			c.VersionFile = Path.Combine(workingDirectory, DeploymentDirectoryStructureConstants.VersionFile);
			c.VersionXPath = DeploymentDirectoryStructureConstants.VersionXPath;
			c.DryRun = DeploymentDirectoryStructureConstants.DryRun;
			c.Restore = this.Restore;
			c.RestoreCustomOptions = this.RestoreCustomOptions;
			c.RestoreFromPath = this.RestoreFromPath;
			c.RestoreTimeout = this.RestoreTimeout;
			c.DatabaseName = StaticRoundHouseSettings.DatabaseName;
			c.DatabaseType = StaticRoundHouseSettings.DatabaseType;
			c.Debug = StaticRoundHouseSettings.Debug;
			c.DisableOutput = StaticRoundHouseSettings.DisableOutput;
			c.DisableTokenReplacement = StaticRoundHouseSettings.DisableTokenReplacement;
			c.DoNotCreateDatabase = StaticRoundHouseSettings.DoNotCreateDatabase;
			c.Drop = StaticRoundHouseSettings.Drop;
			c.EnvironmentName = StaticRoundHouseSettings.EnvironmentName;
			c.CommandTimeout = StaticRoundHouseSettings.CommandTimeout;
			c.CommandTimeoutAdmin = StaticRoundHouseSettings.CommandTimeoutAdmin;
		}

	}
}
