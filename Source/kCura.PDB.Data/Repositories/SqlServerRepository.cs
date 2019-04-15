namespace kCura.PDB.Data.Repositories
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.Linq;
	using System.Security.Cryptography;
	using System.Text;
	using Dapper;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.BISSummary.Models;
	using kCura.PDB.Data.Properties;
	using kCura.PDB.Data.Repositories.BISSummary;
	using kCura.PDB.Data.Services;

	public class SqlServerRepository : ISqlServerRepository
	{
		public IReportRepository ReportRepository { get; set; }
		public IEnvironmentCheckRepository EnvironmentCheckRepository { get; set; }
		public IAnalyticsRepository AnalyticsRepository { get; set; }
		public IFileLatencyRepository FileLatencyRepository { get; set; }
		public IPDBNotificationRepository PDBNotificationRepository { get; set; }
		public IResourceServerRepository ResourceServerRepository { get; set; }
		public IDeploymentRepository DeploymentRepository { get; set; }
		public IPerformanceSummaryRepository PerformanceSummaryRepository { get; set; }
		public IConfigurationRepository ConfigurationRepository { get; set; }
		public IPrimarySqlServerRepository PrimarySqlServerRepository { get; set; }
		public IServerRepository PerformanceServerRepository { get; set; }
		public IAgentRepository AgentRepository { get; set; }
		public ISampleHistoryRepository SampleHistoryRepository { get; set; }
		public IBackfillRepository BackfillRepository { get; set; }
		public IEventRepository EventRepository { get; set; }
		public ILogRepository LogRepository { get; set; }
		public IProcessControlRepository ProcessControlRepository { get; set; }
		public IConfigurationAuditRepository ConfigurationAuditRepository { get; set; }

		private readonly List<IDbRepository> _dataRepositories = new List<IDbRepository>();

		private readonly IConnectionFactory connectionFactory;

		public SqlServerRepository(IConnectionFactory connectionFactory)
		{
			this.connectionFactory = connectionFactory;
			InitRepositories();
		}

		public SqlServerRepository(IConnectionFactory connectionFactory,
			IEnvironmentCheckRepository environmentCheckRepository, IReportRepository reportRepository,
			IAnalyticsRepository analyticsRepository, IFileLatencyRepository fileLatencyRepository,
			IResourceServerRepository resourceServerRepository,
			IDeploymentRepository deploymentRepository,
			IPerformanceSummaryRepository performanceSummaryRepository, IConfigurationRepository configurationRepository,
			IPrimarySqlServerRepository primarySqlServerRepository, IServerRepository performanceServerRepository,
			IAgentRepository agentRepository, ISampleHistoryRepository sampleHistoryRepository,
			IBackfillRepository backfillRepository, IEventRepository eventRepository, ILogRepository logRepository,
			IProcessControlRepository processControlRepository, IConfigurationAuditRepository configurationAuditRepository)
		{
			this.connectionFactory = connectionFactory;
			_dataRepositories.AddRange(new IDbRepository[]
			{
				ReportRepository = reportRepository,
				EnvironmentCheckRepository = environmentCheckRepository,
				AnalyticsRepository = analyticsRepository,
				FileLatencyRepository = fileLatencyRepository,
				PDBNotificationRepository = new PDBNotificationRepository(this.connectionFactory),
				ResourceServerRepository = resourceServerRepository,
				DeploymentRepository = deploymentRepository,
				PerformanceSummaryRepository = performanceSummaryRepository,
				ConfigurationRepository = configurationRepository,
				PrimarySqlServerRepository = primarySqlServerRepository,
				PerformanceServerRepository = performanceServerRepository,
				AgentRepository = agentRepository,
				SampleHistoryRepository = sampleHistoryRepository,
				BackfillRepository = backfillRepository,
				EventRepository = eventRepository,
				LogRepository = logRepository,
				ProcessControlRepository = processControlRepository,
				ConfigurationAuditRepository = configurationAuditRepository
			});
		}

		public void InitRepositories()
		{
			EnvironmentCheckRepository = new EnvironmentCheckRepository(this.connectionFactory);
			_dataRepositories.AddRange(new IDbRepository[] {
				ReportRepository = new ReportRepository(this.connectionFactory),
				EnvironmentCheckRepository,
				AnalyticsRepository = new AnalyticsRepository(this.connectionFactory, EnvironmentCheckRepository),
				FileLatencyRepository = new FileLatencyRepository(this.connectionFactory),
				PDBNotificationRepository = new PDBNotificationRepository(this.connectionFactory),
				ResourceServerRepository = new ResourceServerRepository(this.connectionFactory),
				DeploymentRepository = new DeploymentRepository(this.connectionFactory),
				PerformanceSummaryRepository = new PerformanceSummaryRepository(this.connectionFactory),
				ConfigurationRepository = new ConfigurationRepository(this.connectionFactory),
				PrimarySqlServerRepository = new PrimarySqlServerRepository(this.connectionFactory),
				PerformanceServerRepository = new ServerRepository(this.connectionFactory),
				AgentRepository = new AgentRepository(this.connectionFactory),
				SampleHistoryRepository = new SampleHistoryRepository(this.connectionFactory),
				BackfillRepository = new BackfillRepository(this.connectionFactory),
				EventRepository = new EventRepository(this.connectionFactory),
				LogRepository = new LogRepository(this.connectionFactory),
				ProcessControlRepository = new ProcessControlRepository(this.connectionFactory),
				ConfigurationAuditRepository = new ConfigurationAuditRepository(this.connectionFactory),
			});
		}

		#region ISqlServerRepository Members

		public bool CanConnect(string targetDatabase, string targetServer)
		{
			try
			{
				using (var conn = this.connectionFactory.GetTargetConnection(targetDatabase, targetServer))
				{
					var value = conn.QueryFirstOrDefault<int>("select 1");
					return true;
				}
			}
			catch (SqlException)
			{
				return false;
			}
		}

		public void TestConnection(string targetDatabase, string targetServer)
		{
			using (var conn = this.connectionFactory.GetTargetConnection(targetDatabase, targetServer))
			{
				conn.QueryFirstOrDefault<int>("select 1");
			}
		}


		public ServerInfo[] GetRegisteredSQLServers()
		{
			var servers = new List<ServerInfo>();
			using (var conn = this.connectionFactory.GetEddsConnection())
			{
				using (var reader = conn.ExecuteReader(Resources.GetRegisteredSQLServers))
				{
					while (reader.Read())
					{
						object[] myObjects = new object[reader.FieldCount];
						var x = reader.GetValues(myObjects);

						var server = new ServerInfo();
						server.ArtifactId = reader.GetInt32(0);
						server.Name = reader.GetString(1);
						server.Status = reader.IsDBNull(2) ? 0 : reader.GetInt32(2);
						server.Version = reader.GetString(3);
						server.ServerType = reader.GetString(4);

						if (!string.IsNullOrEmpty(server.Name))
							server.Name = server.Name.ToLowerInvariant();
						servers.Add(server);
					}
				}
			}
			return servers.ToArray();
		}

		/// <summary>
		/// Determines the tab artifact ID given a tab's name
		/// </summary>
		/// <param name="tabName"></param>
		/// <returns></returns>
		public int ReadTabArtifactId(string tabName)
		{
			try
			{
				using (var conn = this.connectionFactory.GetEddsConnection())
				{
					return conn.QueryFirstOrDefault<int>("SELECT TOP 1 ArtifactId FROM Tab WHERE Name=@tabName", new { tabName });
				}
			}
			catch
			{
				return -1;
			}
		}

		public bool PerformanceExists()
		{
			using (var conn = this.connectionFactory.GetMasterConnection())
			{
				return conn.QueryFirstOrDefault<bool>(Resources.DBExists);
			}
		}

		/// <summary>
		/// If EDDSPerformance exists, but it's a pre-RH version, perform an upgrade for RH compatibility.
		/// Otherwise, do nothing.
		/// </summary>
		public void UpgradeIfMissingRoundhouse()
		{
			using (var conn = this.connectionFactory.GetMasterConnection())
			{
				var preRoundhouse = conn.QueryFirstOrDefault<bool>(Resources.IsPreRoundhousE);
				if (preRoundhouse)
				{
					//Apparently there's a version of kIE_BackupAndDBCCCheck that creates static tables with the same names as the ones used in BiS3.0
					var statements = SplitScript(Resources.PurgeEDDSPerformanceTables);
					foreach (var statement in statements)
						conn.Execute(statement);

					//Apply previous versions' manual upgrades that may have been missed
					statements = SplitScript(Resources.ApplyManualUpgrades);
					foreach (var statement in statements)
						conn.Execute(statement);

					//Perform the RoundhousE upgrade
					conn.Execute(Resources.UpgradeToRHVersion);
				}
			}
		}

		public PopulateFactTableResult PopulateFactTable()
		{
			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{
				return conn.QueryFirstOrDefault<PopulateFactTableResult>("eddsdbo.PopulateFactTableData");
			}
		}

		public bool ClaimRollup(string agentId)
		{
			// Uses both EDDS and EDDSPerformance
			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{
				var claimant = conn.QueryFirstOrDefault<string>(Resources.ClaimRollup, new { agentId });
				return claimant == agentId;
			}
		}

		public bool ClaimLookingGlass(string agentId)
		{
			// Uses both EDDS and EDDSPerformance
			using (var conn = this.connectionFactory.GetEddsConnection())
			{
				var claimant = conn.QueryFirstOrDefault<string>(Resources.ClaimLookingGlass, new { agentId });
				return claimant == agentId;
			}
		}

		public bool ClaimServer(int serverId, string agentId)
		{
			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{
				var claimant = conn.QueryFirstOrDefault<string>(Resources.ClaimServer, new { agentId, serverId });
				return claimant == agentId;
			}
		}

		public void UnclaimServer(int serverId, string agentId, bool withDelay)
		{
			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{
				conn.Execute(Resources.UnclaimServer, new { serverId, agentId, delay = withDelay });
			}
		}

		#endregion

		/// <summary>
		/// Split single script into multiple scripts using "GO" as the delimiter
		/// </summary>
		/// <param name="script"></param>
		/// <returns></returns>
		private string[] SplitScript(string script)
		{
			return script.Split(new string[] { DatabaseConstants.GoStatement }, StringSplitOptions.None);
		}

		public bool IfInDebugModeLaunchDebugger()
		{
			bool result;
			var launchDebugger = this.ConfigurationRepository.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.LaunchDebugger);
			if (null != launchDebugger) //if not null
			{
				if (Boolean.TryParse(launchDebugger, out result)) //if parse succeeds
				{
					if (result)
					{
						System.Diagnostics.Debugger.Launch();
						return true;
					}
				}
			}
			return false;
		}

		/// <summary>
		/// Method queries a database, by the SqlServerCredentialInfo object,
		/// and retrieves the average page life expectancy for that particular SQL Server
		/// </summary>
		/// <param name="sqlServerCredentialInfo"></param>
		/// <returns></returns>
		public UInt64 GetPageLifeExpectancyFromServerInstance(Server server)
		{
			UInt64 pagelifeExpectancy = 0; // default to zero
			var sql = @"SELECT cntr_value
                                            FROM sys.dm_os_performance_counters 
                                            WHERE [object_name] LIKE '%Manager%'
                                            AND [counter_name] = 'Page life expectancy';";

			using (var conn = this.connectionFactory.GetEddsQosConnection(server.ServerName))
			{
				pagelifeExpectancy = conn.QueryFirstOrDefault<UInt64>(sql);
			}
			return pagelifeExpectancy;
		}

		public bool GetLowMemorySignalStateFromServerInstance(Server server)
		{
			using (var conn = this.connectionFactory.GetEddsQosConnection(server.ServerName))
			{
				return conn.QueryFirstOrDefault<bool>(Resources.sys_LowMemorySignalState);
			}
		}

		/// <summary>
		/// When called, this goes out to each server and reads SQL pageout warnings from the SQL error log within the last hour,
		/// then logs the number of these (per server) in EDDSPerformance
		/// </summary>
		public void SummarizeSqlServerPageouts()
		{
			var commandText = Resources.CollectPageoutInformation.Replace($"{{{{{SqlScriptTokens.ResourceDbName}}}}}", Names.Database.PdbResource);
			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{
				conn.Execute(commandText);
			}
		}

		/// <summary>
		/// This cycles the SQL server error log on the current SQL server
		/// </summary>
		public void CycleSqlErrorLog(string server)
		{
			using (var conn = (SqlConnection)this.connectionFactory.GetPdbResourceConnection(server))
			{
				conn.Execute(Names.Database.PdbResource + ".eddsdbo.QoS_CycleErrorLog", commandType: CommandType.StoredProcedure);
			}
		}



		/// <summary>
		/// Removes old data from PDB's (non-QoS) tables using a default threshold of 180 days.
		/// To override, add a configuration value for DataCleanupThresholdDays.
		/// </summary>
		public void CleanupDataTables()
		{
			//By default, keep 180 days of data
			var threshold = Math.Abs(DatabaseConstants.PastHalfYearThreshold);
			var configThreshold = this.ConfigurationRepository.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.DataCleanupThresholdDays);
			if (configThreshold != null)
				Int32.TryParse(configThreshold, out threshold);

			//Keep at least 90 days in these tables so data is available for a backfill
			if (threshold < Math.Abs(DatabaseConstants.PastHalfYearThreshold))
				threshold = Math.Abs(DatabaseConstants.PastHalfYearThreshold);

			var cutoff = DateTime.UtcNow.AddDays(-1 * threshold);

			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{
				conn.Execute(Resources.CleanupDataTables, new { cutoff }, commandTimeout: Defaults.Database.CleanupTimeout);
			}
		}

		/// <summary>
		/// Reads the environment's instance name from the EDDS configuration table
		/// </summary>
		/// <returns></returns>
		public string ReadInstanceName()
		{
			using (var conn = this.connectionFactory.GetEddsConnection())
			{
				return conn.QueryFirstOrDefault<string>(Resources.ReadInstanceName);
			}
		}

		/// <summary>
		/// Inserts and deletes DBCC targets based on the current linked server configuration.
		/// This happens once a day just before the backup/DBCC monitor is called.
		/// </summary>
		public void RefreshDbccTargets()
		{
			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{
				conn.Execute(Resources.RefreshDbccTargets);
			}
		}

		public List<DbccTargetInfo> ListDbccTargets()
		{
			var targets = new List<DbccTargetInfo>();
			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{
				using (var reader = conn.ExecuteReader(Resources.ListDbccTargets))
				{
					while (reader.Read())
					{
						var info = new DbccTargetInfo();
						info.Id = reader.GetInt32(0);
						info.Server = reader.GetString(1).ToUpper();
						info.Database = reader.GetString(2);
						info.IsActive = reader.GetBoolean(3);
						targets.Add(info);
					}
				}
			}
			return targets;
		}

		public void UpdateDbccTarget(int targetId, string database, bool isActive)
		{
			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{
				conn.Execute(Resources.UpdateDbccTarget, new { targetId, dbName = database, active = isActive });
			}
		}

		public void DeployDbccLogView(string targetDatabase, string targetServer)
		{
			using (var conn = this.connectionFactory.GetTargetConnection(targetDatabase, targetServer))
			{
				conn.Execute(Resources.CreateDBCCLogView);
			}
		}

		public void TestDbccLogView(string targetDatabase, string targetServer)
		{
			using (var conn = this.connectionFactory.GetTargetConnection(targetDatabase, targetServer))
			{
				conn.Execute(@"SELECT TOP 1 DatabaseName, DbccTime FROM eddsdbo.QoS_DBCCLog");
			}
		}

		/// <summary>
		/// Indicates whether the AdminScriptsVersion value is the latest version number
		/// </summary>
		/// <returns></returns>
		public bool AdminScriptsInstalled()
		{
			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{
				return conn.QueryFirstOrDefault<bool>(Resources.CheckAdminScriptsInstalled);
			}
		}

		/// <summary>
		/// Executes backup/DBCC table purge script
		/// </summary>
		public void PurgeBackupDBCCTables()
		{
			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{
				var statements = SplitScript(Resources.PurgeBackupDBCCTables);
				foreach (var statement in statements)
					conn.Execute(statement);
			}
		}

		public void ExecuteVirtualLogFileMonitor(int agentId)
		{
			var enableLogging = this.ConfigurationRepository.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.EnableLookingGlassLogging) ?? "true";
			var withLogging = enableLogging.Equals("true", StringComparison.CurrentCultureIgnoreCase);
			using (var conn = (SqlConnection)this.connectionFactory.GetEddsPerformanceConnection())
			{
				conn.Execute("EDDSPerformance.eddsdbo.QoS_VLFMonitor", new { agentId, logging = withLogging, eddsPerformanceServerName = conn.DataSource }, commandType: CommandType.StoredProcedure, commandTimeout: 900);
			}
		}

		public BackupDBCCMonitoringResults ExecuteBackupDBCCMonitor(int agentId, string sproc)
		{
			var results = new BackupDBCCMonitoringResults();
			var enableLogging = this.ConfigurationRepository.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.EnableLookingGlassLogging) ?? "true";
			var withLogging = enableLogging.Equals("true", StringComparison.CurrentCultureIgnoreCase);
			var eddsServerName = string.Empty;
			using (var conn = (SqlConnection)this.connectionFactory.GetEddsConnection())
			{
				eddsServerName = conn.DataSource;
			}

			using (var conn = (SqlConnection)this.connectionFactory.GetEddsPerformanceConnection())
			{
				conn.Execute(
					sproc,
					new { eddsPerformanceServerName = conn.DataSource, eddsServerName, agentId, CreateHistory = true, logging = withLogging },
					commandType: CommandType.StoredProcedure,
					commandTimeout: 60 * 60);

				using (var reader = SqlHelper.ExecuteReader(conn, CommandType.Text, Resources.FailedMonitoringTargets))
				{
					while (reader.Read())
					{
						results.FailedServers = reader.GetInt32(0);
						results.FailedDatabases = reader.GetInt32(1);
						results.ServerErrors = reader.IsDBNull(2) ? string.Empty : reader.GetString(2);
						results.DatabaseErrors = reader.IsDBNull(3) ? string.Empty : reader.GetString(3);
					}
				}
			}
			return results;
		}

		public int UserExperienceForecastForServer(string server)
		{
			using (var conn = this.connectionFactory.GetEddsQosConnection(server))
			{
				return conn.QueryFirstOrDefault<int>(Resources.UserExMonitoring_ScoreForecast);
			}
		}

		public List<SystemLoadForecast> SystemLoadForecast()
		{
			var forecasts = new List<SystemLoadForecast>();
			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{
				using (var reader = conn.ExecuteReader(Resources.SystemLoadMonitoring_ScoreForecast))
				{
					while (reader.Read())
					{
						var forecast = new SystemLoadForecast
						{
							ServerTypeId = reader.IsDBNull(0) ? 1 : reader.GetInt32(0),
							ServerName = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
							CPUScore = reader.IsDBNull(2) ? 100 : reader.GetInt32(2),
							RAMScore = reader.IsDBNull(3) ? 100 : reader.GetInt32(3),
							SQLMemoryScore = reader.IsDBNull(4) ? 100 : reader.GetInt32(4),
							SQLWaitsScore = reader.IsDBNull(5) ? 100 : reader.GetInt32(5),
							SQLVirtualLogFilesScore = reader.IsDBNull(6) ? 100 : reader.GetInt32(6)
						};
						forecasts.Add(forecast);
					}
				}
			}
			return forecasts;
		}

		public void ExecuteWaitMonitor(int agentId)
		{
			var enableLogging = this.ConfigurationRepository.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.EnableLookingGlassLogging) ?? "true";
			var withLogging = enableLogging.Equals("true", StringComparison.CurrentCultureIgnoreCase);
			var eddsServerName = string.Empty;
			using (var conn = this.connectionFactory.GetEddsConnection())
			{
				eddsServerName = ((SqlConnection)conn).DataSource;
			}

			using (var conn = (SqlConnection)this.connectionFactory.GetEddsPerformanceConnection())
			{
				conn.Execute(
					"EDDSPerformance.eddsdbo.QoS_WaitMonitor",
					new
					{
						agentId,
						logging = withLogging,
						eddsServerName,
						eddsPerformanceServerName = conn.DataSource
					},
					commandType: CommandType.StoredProcedure,
					commandTimeout: Defaults.Database.WaitMonitorTimeout);
			}
		}

		/// <summary>
		/// Inserts configuration changes into eddsdbo.Configuration
		/// </summary>
		/// <param name="changes"></param>
		public void AuditConfigurationChanges(IList<ConfigurationAudit> changes, bool triggerAlert)
		{
			if (changes != null && changes.Any())
			{
				//Track the provided configuration changes
				this.ConfigurationAuditRepository.Create(changes);

				//Tell the agent to send out the notice
				if (triggerAlert)
				{
					using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
						conn.Execute(Resources.StageReconfigNoticeDelivery);
				}
			}
		}

		public SmtpSettings ReadRelativitySMTPSettings()
		{
			var settings = new SmtpSettings();
			using (var conn = this.connectionFactory.GetEddsConnection())
			{
				using (var reader = conn.ExecuteReader(Resources.SMTPSettings))
				{
					while (reader.Read())
					{
						//If the server setting is missing, assume localhost
						var serverConfig = reader.GetString(0);
						settings.Server = string.IsNullOrEmpty(serverConfig)
							? "localhost"
							: serverConfig;

						//If the configured port is not an integer or the setting is missing, use Relativity's default SMTP port of 25
						int port;
						var portConfig = reader.GetString(1);
						settings.Port = int.TryParse(portConfig, out port)
							? port
							: 25;

						//There are no suitable defaults for these
						settings.EmailFrom = reader.GetString(2);
						settings.EmailTo = reader.GetString(3);
						settings.Username = reader.GetString(4);
						settings.Password = reader.IsDBNull(5) ? null : reader.GetString(5);
						settings.EncryptedPassword = reader.IsDBNull(6) ? null : reader.GetString(6);
						settings.SSLisRequired = reader.IsDBNull(7) || string.IsNullOrEmpty(reader.GetString(7))
							? false
							: Convert.ToBoolean(reader.GetString(7));
					}
				}
			}
			return settings;
		}

		/// <summary>
		/// Indicates whether there are legacy hashes (Base64 MD5) in RHScriptsRun based on the installed version.
		/// </summary>
		/// <returns></returns>
		public bool IsVersionedWithLegacyHashes()
		{
			try
			{
				// ConfigurationKeys.AssemblyFileVersion was removed
				var version = this.ConfigurationRepository.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.AssemblyFileVersion);
				if (!string.IsNullOrEmpty(version))
				{
					var versionParts = version.Split('.');
					var year = int.Parse(versionParts[0]);
					var month = int.Parse(versionParts[1]);
					var day = int.Parse(versionParts[2]);
					var versionDate = new DateTime(year, month, day);
					if (versionDate >= new DateTime(2014, 8, 27))
						return false;
				}
				return false;
			}
			catch
			{
				//If we can't tell from the configuration, assume we need to do this once (after which the version should be updated)
			}

			return true;
		}

		/// <summary>
		/// RoundhousE uses MD5 hashes by default, but these are not FIPS compliant. To support FIPS, we have switched to SHA1 hashes.
		/// Given a database context, this method retrieves existing script information and updates hashes based on the text of the script.
		/// This needs to execute BEFORE RoundhousE is permitted to touch the database. You only need to call this if the database uses legacy hashes.
		/// </summary>
		public void ConvertLegacyRHScriptHashes(string targetDatabase, string targetServer = null)
		{
			using (var conn = (SqlConnection)this.connectionFactory.GetTargetConnection(targetDatabase, targetServer))
			{
				SqlHelper.ExecuteNonQuery(conn, CommandType.Text, Resources.CreateRHScriptsRunTableType);

				using (var command = new SqlCommand(Resources.ConvertRH_MD5toSHA1, conn))
				{
					//SHA1 is FIPS compliant
					var crypto_provider = SHA1.Create();

					//Get id, text_of_script, and text_hash from RHScriptsRun
					var scriptsRun = SqlHelper.ExecuteDataset(conn, CommandType.Text, Resources.ReadRHScriptInfo);
					if (scriptsRun.Tables.Count > 0)
					{
						foreach (DataRow row in scriptsRun.Tables[0].Rows)
						{
							//Get the text that RH would have actually run
							var text = row.Field<string>("text_of_script").Replace(@"'", @"''");

							//Use the same method as RH to compute the hash
							var clear_text_bytes = Encoding.UTF8.GetBytes(text);
							var cypher_bytes = crypto_provider.ComputeHash(clear_text_bytes);
							var hashed = BitConverter.ToString(cypher_bytes).Replace("-", String.Empty);

							//Update the hash in our data set
							row.SetField<string>("text_hash", hashed);
						}

						//Pass modified table of script info in as a structured parameter
						var modified = command.Parameters.Add("@modified", SqlDbType.Structured);
						modified.TypeName = "eddsdbo.RHScriptsRunType";
						modified.Value = scriptsRun.Tables[0];

						//Update existing hashes with new SHA1 versions
						command.ExecuteNonQuery();
					}
				}
			}
		}

		public DateTime ReadServerUtcTime()
		{
			using (var conn = this.connectionFactory.GetEddsConnection())
			{
				return conn.QuerySingle<DateTime>(Resources.ReadServerUtcTime);
			}
		}
	}
}
