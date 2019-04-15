namespace kCura.PDB.Data.Repositories
{
	using System;
	using System.Data;
	using System.Data.SqlClient;
	using System.IO;
	using System.Text.RegularExpressions;
	using Dapper;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.Deployment;
	using kCura.PDB.Core.Models.ScriptInstallation;
	using kCura.PDB.Data.Properties;
	using kCura.PDB.Data.Services;

	public class DeploymentRepository : BaseRepository, IDeploymentRepository
	{
		public DeploymentRepository(IConnectionFactory connectionFactory)
			: base(connectionFactory)
		{
		}

		/// <summary>
		/// Obtains information about the MDF/LDF directories used for Relativity databases on this environment
		/// </summary>
		/// <param name="server"></param>
		/// <returns></returns>
		public DatabaseDirectoryInfo ReadMdfLdfDirectories(string server = "")
		{
			var directoryInfo = new DatabaseDirectoryInfo();

			using (var conn = (SqlConnection)this.connectionFactory.GetEddsConnection())
			{
				using (var command = new SqlCommand())
				{
					var serverName = command.CreateParameter();
					serverName.ParameterName = "@serverName";
					serverName.DbType = DbType.String;
					serverName.Value = server ?? "";

					var results = SqlHelper.ExecuteDataset(conn, CommandType.Text, Resources.ReadMdfLdfDirectories, new[] { serverName });
					if (results.Tables.Count > 0 && results.Tables[0].Rows.Count > 0)
					{
						directoryInfo.MdfPath = results.Tables[0].Rows[0].Field<string>("MDF");
						directoryInfo.LdfPath = results.Tables[0].Rows[0].Field<string>("LDF");

						// Doing this because when we use the path in SQL, it assumes the trailing slash already exists
						if (!directoryInfo.MdfPath.EndsWith("\\"))
						{
							directoryInfo.MdfPath += "\\";
						}
						if (!directoryInfo.LdfPath.EndsWith("\\"))
						{
							directoryInfo.LdfPath += "\\";
						}
					}
				}
			}

			return directoryInfo;
		}

		/// <summary>
		/// Reads the collation for EDDS database
		/// </summary>
		/// <returns></returns>
		public string ReadCollationSettings()
		{
			using (var conn = this.connectionFactory.GetEddsConnection())
			{
				return conn.ExecuteScalar<string>(Resources.ReadDatabaseCollation);
			}
		}

		public void RunSqlScripts(DeploymentSettings settings, string spocsScriptPath, MigrationResultSet result)
		{
			try
			{
				using (var conn = this.connectionFactory.GetTargetConnection(settings.DatabaseName, settings.Server, settings.CredentialInfo))
				{
					foreach (var sqlFile in Directory.GetFiles(spocsScriptPath, "*.sql"))
					{
						//This is a non-critical procedure, and replacing it could interrupt a VARSCAT call
						if (sqlFile.Contains("LogAppend"))
							continue;

						var msg = new LogMessage(LogSeverity.Info, $"Redeploying script {sqlFile}");
						result.Messages.Add(msg);
						string scriptText = File.ReadAllText(sqlFile);
						string[] commandTexts = Regex.Split(scriptText + "\n", @"^\s*GO\s*$", RegexOptions.IgnoreCase | RegexOptions.Multiline);
						foreach (string commandText in commandTexts)
						{
							if (!string.IsNullOrWhiteSpace(commandText))
								conn.Execute(commandText);
						}
					}
				}
			}
			catch (SqlException)
			{
				//eddsdbo can't connect to this case - it doesn't exist or isn't accessible
				//throw; // Used for integration tests
				return; // Do we really not want to throw in this case?  We'd be swallowing errors and progressing as successful anyway. :/
			}
		}

		public void RunCreateDatabaseScripts(DeploymentSettings settings, string createScriptPath, MigrationResultSet result)
		{
			if (!File.Exists(createScriptPath)) return;
			using (var conn = this.connectionFactory.GetMasterConnection(settings.Server, settings.CredentialInfo))
			{
				var scriptText = File.ReadAllText(createScriptPath);
				result.Messages.Add(new LogMessage(LogSeverity.Info, $"Creating database started."));
				string[] commandTexts = Regex.Split(scriptText + "\n", @"^\s*GO\s*$", RegexOptions.IgnoreCase | RegexOptions.Multiline);
				foreach (var commandText in commandTexts)
				{
					if (string.IsNullOrWhiteSpace(commandText)) continue;

					result.Messages.Add(new LogMessage(LogSeverity.Info, $"Create database script {commandText}"));
					conn.Execute(commandText);
				}
			}

			// Test to see if we can connect to new database before finishing
			result.Messages.Add(new LogMessage(LogSeverity.Info, $"Testing to see if we can connect to database."));
			var stop = 0;
			var timeout = DateTime.UtcNow.AddSeconds(120);
			while (stop == 0 && timeout > DateTime.UtcNow)
			{
				System.Threading.Thread.Sleep(1000);
				try
				{
					using (var conn = this.connectionFactory.GetTargetConnection(settings.DatabaseName, settings.Server, settings.CredentialInfo))
					{
						stop = conn.ExecuteScalar<int>("select top(1) 1 FROM [sys].[objects]");
					}
				}
				catch { /* -- Intentionally eating exception -- */}
			}

			// Create initial roundhouse databases if they don't exist
			using (var conn = this.connectionFactory.GetTargetConnection(settings.DatabaseName, settings.Server, settings.CredentialInfo))
			{
				result.Messages.Add(new LogMessage(LogSeverity.Info, $"Creating Roundhouse Tables if they don't exist."));
				conn.Execute(Resources.CreateRoundHouseTables);
			}

			result.Messages.Add(new LogMessage(LogSeverity.Info, "Creating database completed."));
		}

		/// <summary>
		/// If RHTimeoutSeconds is not present in the configuration table, it will be inserted with a default value
		/// </summary>
		public void InsertRoundhouseTimeoutSettingIfNotExists()
		{
			try
			{
				using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
				{
					conn.Execute(Resources.InsertRHTimeoutConfig);
				}
			}
			catch { }
		}

		public void RemoveOldApplicationReferences()
		{
			// Cleanup from main EDDS
			using (var connection = this.connectionFactory.GetEddsConnection())
			{
				connection.Execute(Resources.CleanupAndDeleteOldAgent, new { oldAgentGuid = Guids.Agent.AgentGuidsToRemove });
			}
		}

		public void RemoveOldApplicationReferencesFromWorkspace(Guid agentGuid, int workspaceId)
		{
			// Cleanup from Workspace first 
			using (var connection = this.connectionFactory.GetWorkspaceConnection(workspaceId))
			{
				connection.Execute(Resources.RemoveOldAgentTypeFromWorkspace, new { oldAgentGuid = agentGuid });
			}
		}

		public void RemoveOldResourceFiles()
		{
			// Cleanup from main EDDS
			using (var connection = this.connectionFactory.GetEddsConnection())
			{
				connection.Execute(Resources.RemoveOldResourceFiles);
			}
		}

	}
}
