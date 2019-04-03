namespace kCura.PDB.Core.Models.Deployment
{
	using kCura.PDB.Core.Constants;
	
	public class StaticRoundHouseSettings
	{
		/* More documentation at also some of the terms below aren't in the readme
		// file located at the following url, you need to physically search through
		// the repository to get the definitions
		// https://github.com/chucknorris/roundhouse/wiki/ConfigurationOptions
		//*/

		#region General Settings

		/// <summary>
		/// --ct, --commandtimeout=VALUE | CommandTimeout - 
		/// This is the timeout when commands are run. This is 
		/// not for admin commands or restore. Defaults to 60. 
		/// New in v0.8.5!
		/// </summary>
		public static int CommandTimeout = 1800;

		/// <summary>
		/// --cta, --commandtimeoutadmin=VALUE | CommandTimeoutAdministration - 
		/// This is the timeout when administration commands are run (except 
		/// for restore, which has its own). Defaults to 300. New in v0.8.5!
		/// </summary>
		public static int CommandTimeoutAdmin = 300;

		/// <summary>
		/// --csa, --connstringadmin, --connectionstringadministration=VALUE 
		/// | ConnectionstringAdministration - This is used for connecting 
		/// to master when you may have a different uid and password than normal.
		/// </summary>
		public static string ConnectionstringAdmin = string.Empty;

		/// <summary>
		/// -d, --db, --database, --databasename=VALUE | REQUIRED: DatabaseName 
		/// - The database you want to create/migrate.
		/// </summary>
		public static string DatabaseName = Names.Database.EddsPerformance;

		/// <summary>
		/// --dt, --dbt, --databasetype=VALUE | DatabaseType - Tells RH 
		/// what type of database it is running on. This is a plugin model. 
		/// This is the fully qualified name of a class that implements the 
		/// interface roundhouse.sql.Database, roundhouse. If you have your own 
		/// assembly, just set it next to rh.exe and set this value 
		/// appropriately. Defaults to sqlserver which is a synonym for 
		/// roundhouse.databases.sqlserver.SqlServerDatabase, 
		/// roundhouse.databases.sqlserver.
		/// </summary>
		public static string DatabaseType = @"sqlserver";

		/// <summary>
		/// --env, --environment, --environmentname=VALUE | EnvironmentName - 
		/// This allows RH to be environment aware and only run scripts that 
		/// are in a particular environment based on the namingof the script. 
		/// LOCAL.something**.ENV.**sql would only be run in the LOCAL environment. 
		/// Defaults to LOCAL.
		/// </summary>
		public static string EnvironmentName = @"LOCAL";

		/// <summary>
		/// -r, --repo, --repositorypath=VALUE | RepositoryPath - The repository. 
		/// A string that can be anything. Used to track versioning along with 
		/// the version. Defaults to null.
		/// </summary>
		public static string RepositoryPath = @"kCura Corporation - Performance Dashboard";

		#endregion

		#region Schema & Tables

		/// <summary>
		/// --sc, --schema, --schemaname=VALUE | SchemaName - This is the schema 
		/// where RH stores it's tables. Once you set this a certain way, do not 
		/// change this. This is definitely running with scissors and very sharp. 
		/// I am allowing you to have flexibility, but because this is a knife 
		/// you can still get cut if you use it wrong. I'm just saying. You've 
		/// been warned. Defaults to RoundhousE.
		/// </summary>
		public static string SchemaName = Names.Database.EddsdboSchema;

		/// <summary>
		/// --vt, --versiontable, --versiontablename=VALUE | VersionTableName - 
		/// This is the table where RH stores versioning information. Once you set this, 
		/// do not change this. This is definitely running with scissors and very sharp.
		/// Defaults to Version.
		/// </summary>
		public static string VersionTableName = @"RHVersion";

		/// <summary>
		/// --srt, --scriptsruntable, --scriptsruntablename=VALUE | ScriptsRunTableName - 
		/// This is the table where RH stores information about scripts that have been run. 
		/// Once you set this a certain way, do not change this. This is definitely 
		/// running with scissors and very sharp. Defaults to ScriptsRun.
		/// </summary>
		public static string ScriptsRunTableName = @"RHScriptsRun";

		/// <summary>
		/// --sret, --scriptsrunerrorstable, --scriptsrunerrorstablename=VALUE | 
		/// ScriptsRunErrorsTableName - This is the table where RH stores information 
		/// about scripts that have been run with errors. Once you set this a certain 
		/// way, do not change this. This is definitelly running with scissors and 
		/// very sharp. Defaults to ScriptsRunErrors.
		/// </summary>
		public static string ScriptsRunErrorsTableName = @"RHScriptsRunErrors";

		#endregion

		#region Switches

		/// <summary>
		/// --drop | Drop - This instructs RH to remove a database and not run 
		/// migration scripts. Defaults to false.
		/// </summary>
		public static bool Drop = false;

		/// <summary>
		/// --dc, --dnc, --donotcreatedatabase | DoNotCreateDatabase - This instructs RH 
		/// to not create a database if it does not exists. Defaults to false.
		/// </summary>
		public static bool DoNotCreateDatabase = true;

		/// <summary>
		/// -w, --warnononetimescriptchanges | WarnOnOneTimeScriptChanges - Instructs RH to 
		/// execute changed one time scripts (DDL/DML in Up folder) that have previously been 
		/// run against the database instead of failing. A warning is logged for each one time 
		/// scripts that is rerun. Defaults to false.
		/// </summary>
		public static bool WarnOnOneTimeScriptChanges = false; //only execute one time scripts once dammit!

		/// <summary>
		/// --disableoutput | DisableOutput - Disable output of backups, items ran, permissions 
		/// dumps, etc. Log files are kept. Useful for example in CI environment. Defaults to False.
		/// </summary>
		public static bool DisableOutput = true; //keep backups just need to specify location in FolderStructureService

		/// <summary>
		/// --silent, --ni, --noninteractive | Silent - tells RH not to ask for any input 
		/// when it runs. Defaults to false.
		/// </summary>
		public static bool Silent = true;

		/// <summary>
		/// -t, --trx, --transaction, --wt, --withtransaction | WithTransaction - This 
		/// instructs RH to run inside of a transaction. Defaults to false.
		/// </summary>
		public static bool WithTransaction = false;

		/// <summary>
		/// --debug | Debug - This instructs RH to write out all messages. Defaults to false.
		/// </summary>
		public static bool Debug = false;

		/// <summary>
		/// --runallanytimescripts, --forceanytimescripts | RunAllAnyTimeScripts - This 
		/// instructs RH to run any time scripts every time it is run. Defaults to false.
		/// </summary>
		public static bool RunAllAnyTimeScripts = false;

		/// <summary>
		/// --disabletokens, --disabletokenreplacement | DisableTokenReplacement - This 
		/// instructs RH to not perform token replacement {{somename}}. Defaults to false. New in v0.8.5!
		/// </summary>
		public static bool DisableTokenReplacement = false;

		/// <summary>
		/// --searchallinsteadoftraverse, --searchallsubdirectoriesinsteadoftraverse |
		/// SearchAllSubdirectoriesInsteadOfTraverse - Each Migration folder's subdirectories
		/// are traversed by default. This option pulls back scripts from the main 
		/// directory and all subdirectories at once. Defaults to false. New in v0.8.5!
		/// </summary>
		public static bool SearchAllSubdirectoriesInsteadOfTraverse = false;

		/// <summary>
		/// Baseline - This instructs RH to create an insert for its recording tables, 
		/// but not to actually run anything against the database. Use this option if 
		/// you already have scripts that have been run through other means 
		/// (and BEFORE you start the new ones).",
		/// </summary>
		public static bool Baseline = false; //only need to use if someone updates scripts inside round house...

		#endregion

	}
}
