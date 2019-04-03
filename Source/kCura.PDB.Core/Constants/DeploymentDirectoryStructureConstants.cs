namespace kCura.PDB.Core.Constants
{
	using System;

	public class DeploymentDirectoryStructureConstants
	{
		/* ================================ ORDER OF FOLDER EXECUTION ================================
		 alter database ** requires Admin connection type **
			run after create database ** only performed if database was created **
			up
			run first after up
			functions
			views
			sprocs
			indexes
			run after other any time scripts
			permissions ** must execute outside of transaction **
		 ==================================================================
		 * alter_database_folder
		 * run_after_create_database_folder 
		 * run_before_up_folder 
		 * up_folder 
		 * down_folder 
		 * run_first_folder
		 * functions_folder 
		 * views_folder 
		 * sprocs_folder 
		 * indexes_folder
		 * runAfterOtherAnyTimeScripts_folder
		 * permissions_folder
		 */

		/// <summary>
		/// --ad, --alterdatabase, --alterdatabasefolder, --alterdatabasefoldername=DatabaseName 
		/// | AlterDatabaseFolderName - The name of the folder where you keep 
		/// your alter database scripts. Read up on token replacement. 
		/// You will want to use {{DatabaseName}} here instead of specifying 
		/// a database name. Will recurse through subfolders. Defaults to 
		/// alterDatabase. New in v0.8.5!
		/// </summary>
		public static string AlterDatabaseFolderName = @"1_Alter";

		/// <summary>
		/// --racd, --runaftercreatedatabase, --runaftercreatedatabasefolder, 
		/// --runaftercreatedatabasefoldername=VALUE | RunAfterCreateDatabaseFolderName - 
		/// The name of the folder where you will keep scripts that ONLY run after 
		/// a database is created. Will recurse through subfolders. 
		/// Defaults to runAfterCreateDatabase. New in v0.8.5!
		/// </summary>
		public static string RunAfterCreateDatabaseFolderName = @"2_RunAfterCreateOrAlter";

		/// <summary>
		/// --rb, --runbefore, --runbeforeupfolder, --runbeforeupfoldername=VALUE | 
		/// RunBeforeUpFolderName - The name of the folder where you keep scripts that 
		/// you want to run before your update scripts. Will recurse through 
		/// subfolders. Defaults to runBeforeUp. New in v0.8.6
		/// </summary>
		public static string RunBeforeUpFolderName = @"3_RunBeforeUpdate";

		/// <summary>
		/// -u, --up, --upfolder, --upfoldername=VALUE | UpFolderName - The name of the 
		/// folder where you keep your update scripts. Will recurse through subfolders. 
		/// Defaults to up.
		/// </summary>
		public static string UpFolderName = @"4_Update";
		
		/// <summary>
		/// folder with legacy code that only runs if conditions are met.
		/// </summary>
		public static string LegacyFolderName = @"5_Legacy";

		/// <summary>
		/// --fu, --functions, --functionsfolder, --functionsfoldername=VALUE | FunctionsFolderName 
		/// - The name of the folder where you keep your functions. Will recurse through subfolders. 
		/// Defaults to functions.
		/// </summary>
		public static string FunctionsFolderName = @"6_Functions";

		/// <summary>
		/// --vw, --views, --viewsfolder, --viewsfoldername=VALUE | ViewsFolderName - The name of 
		/// the folder where you keep your views. Will recurse through subfolders. Defaults to views.
		/// </summary>
		public static string ViewsFolderName = @"7_Views";

		/// <summary>
		/// --sp, --sprocs, --sprocsfolder, --sprocsfoldername=VALUE | SprocsFolderName - The name of the folder where you keep your stored procedures. Will recurse through subfolders. Defaults to sprocs.
		/// </summary>
		public static string SprocsFolderName = @"8_StoredProcedures";

		/// <summary>
		/// --ix, --indexes, --indexesfolder, --indexesfoldername=VALUE | IndexesFolderName - The name of the folder where you keep your indexes. Will recurse through subfolders. Defaults to indexes. New in v0.8.5!
		/// </summary>
		public static string IndexesFolderName = @"9_Indexes";

		/// <summary>
		/// --ra, --runAfterOtherAnyTimeScripts, --runAfterOtherAnyTimeScriptsfolder, --runAfterOtherAnyTimeScriptsfoldername=VALUE | RunAfterOtherAnyTimeScriptsFolderName - The name of the folder where you keep scripts that will be run after all of the other any time scripts complete. Will recurse through subfolders. Defaults to runAfterOtherAnyTimeScripts.
		/// </summary>
		public static string RunAfterOtherAnyTimeScriptsFolderName = @"10_RunAfterAllAnyTime";

		/// <summary>
		/// -p, --permissions, --permissionsfolder, --permissionsfoldername=VALUE | PermissionsFolderName - The name of the folder where you keep your permissions scripts. Will recurse through subfolders. Defaults to permissions.
		/// </summary>
		public static string PermissionsFolderName = @"11_Permissions";

		/// <summary>
		/// CreateDatabaseCustomScript properties - These instructs RH to use a given script for creating
		/// a database instead of the default based on the SQLType.
		/// </summary>
		public static string CreatePerformanceCustomScript = @"0000001Create_EDDSPerformance_DB.sql";
		public static string CreateQoSCustomScript = @"0000001Create_EDDSQoS_DB.sql";
		public static string CreateResouceCustomScript = @"0000001Create_Resource_DB.sql";
		public static string CheckTargetExistsCustomScript = @"0000001TargetDatabaseExists.sql";

		/// <summary>
		/// -o, --output, --outputpath=VALUE | OutputPath - This is where 
		/// everything related to the migration is stored. This includes 
		/// any backups, all items that ran, permission dumps, logs, etc. 
		/// Defaults to a special folder, common application data with 
		/// roundhouse as subdirectory, i.e. C:\ProgramData\ChuckNorris\RoundhousE.
		/// </summary>
		public static string OutputPath = String.Empty;

		/// <summary>
		/// --vf, --versionfile=VALUE | VersionFile - Either a .XML file, a .DLL 
		/// or a .TXT file that a version can be resolved from. Defaults to 
		/// _BuildInfo.xml.
		/// </summary>
		public static string VersionFile = @"RoundHouseConfiguration.xml";

		/// <summary>
		/// --vx, --versionxpath=VALUE | VersionXPath - Works in conjunction with an 
		/// XML version file. Defaults to //buildInfo/version.
		/// </summary>
		public static string VersionXPath = String.Empty;

		/// <summary>
		/// Down Folder Name
		/// </summary>
		public static string DownFolderName = String.Empty;

		/// <summary>
		/// "DryRun - This instructs RH to log what would have run, but not to actually run
		/// anything against the database. Use this option if you are trying to figure out 
		/// what RH is going to do."
		/// </summary>
		public static Boolean DryRun = true;
	}
}
