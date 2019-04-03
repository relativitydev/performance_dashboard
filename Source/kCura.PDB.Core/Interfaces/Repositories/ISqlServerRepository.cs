namespace kCura.PDB.Core.Interfaces.Repositories
{
	using System;
	using System.Collections.Generic;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.BISSummary.Models;

	public interface ISqlServerRepository
	{
		// Repositories
		IReportRepository ReportRepository { get; set; }

		IEnvironmentCheckRepository EnvironmentCheckRepository { get; set; }

		IAnalyticsRepository AnalyticsRepository { get; set; }

		IFileLatencyRepository FileLatencyRepository { get; set; }

		IPDBNotificationRepository PDBNotificationRepository { get; set; }

		IResourceServerRepository ResourceServerRepository { get; set; }

		IDeploymentRepository DeploymentRepository { get; set; }

		IPerformanceSummaryRepository PerformanceSummaryRepository { get; set; }

		IConfigurationRepository ConfigurationRepository { get; set; }

		IPrimarySqlServerRepository PrimarySqlServerRepository { get; set; }

		IServerRepository PerformanceServerRepository { get; set; }

		IAgentRepository AgentRepository { get; set; }

		ISampleHistoryRepository SampleHistoryRepository { get; set; }

		IBackfillRepository BackfillRepository { get; set; }

		IEventRepository EventRepository { get; set; }

		ILogRepository LogRepository { get; set; }

		IProcessControlRepository ProcessControlRepository { get; set; }

		IConfigurationAuditRepository ConfigurationAuditRepository { get; set; }

		bool CanConnect(string targetDatabase, string targetServer);

		void TestConnection(string targetDatabase, string targetServer);

		ServerInfo[] GetRegisteredSQLServers();

		int ReadTabArtifactId(string tabName);

		bool PerformanceExists();

		void UpgradeIfMissingRoundhouse();

		bool IsVersionedWithLegacyHashes();

		void ConvertLegacyRHScriptHashes(string targetDatabase, string targetServer = null);

		bool ClaimRollup(string agentId);

		[Obsolete("Looking glass has been removed and only one QoS Manager agent can be created so no need to claim")]
		bool ClaimLookingGlass(string agentId);

		bool ClaimServer(int serverId, string agentId);

		void UnclaimServer(int serverId, string agentId, bool withDelay);

		bool IfInDebugModeLaunchDebugger();

		UInt64 GetPageLifeExpectancyFromServerInstance(Server server);

		bool GetLowMemorySignalStateFromServerInstance(Server server);

		void SummarizeSqlServerPageouts();

		void CycleSqlErrorLog(string server);

		string ReadInstanceName();

		PopulateFactTableResult PopulateFactTable();

		void RefreshDbccTargets();

		List<DbccTargetInfo> ListDbccTargets();

		void UpdateDbccTarget(int targetId, string database, bool isActive);

		void DeployDbccLogView(string targetDatabase, string targetServer);

		void TestDbccLogView(string targetDatabase, string targetServer);

		bool AdminScriptsInstalled();

		void PurgeBackupDBCCTables();

		void ExecuteVirtualLogFileMonitor(int agentId);

		int UserExperienceForecastForServer(string server);

		List<SystemLoadForecast> SystemLoadForecast();

		BackupDBCCMonitoringResults ExecuteBackupDBCCMonitor(int agentId, string sproc);

		void ExecuteWaitMonitor(int agentId);

		void AuditConfigurationChanges(IList<ConfigurationAudit> changes, bool triggerAlert);

		SmtpSettings ReadRelativitySMTPSettings();

		void CleanupDataTables();

		DateTime ReadServerUtcTime();
	}
}
