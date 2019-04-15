namespace kCura.PDB.Core.Constants
{
	using System;
	using System.Collections.Generic;

	public static class Names
	{
		public static class Agent
		{
			public const string MetricManagerAgentName = "Performance Dashboard - Metric Manager";
			public const string TrustWorkerAgentName = "Performance Dashboard - Trust [UNINSTALL]";
			public const string QoSManagerAgentName = "Performance Dashboard - QoS Manager";
			public const string QosWorkerAgentName = "Performance Dashboard - QoS Worker";
			public const string WmiWorkerAgentName = "Performance Dashboard - WMI Worker";

			public static string FromGuid(Guid agentTypeGuid)
			{
				switch (agentTypeGuid.ToString().ToUpper())
				{
					case Guids.Agent.MetricManagerAgentGuidString:
						return MetricManagerAgentName;
					case Guids.Agent.QoSManagerAgentGuidString:
						return QoSManagerAgentName;
					case Guids.Agent.QosWorkerAgentGuidString:
						return QosWorkerAgentName;
					case Guids.Agent.TrustWorkerAgentGuidString:
						return TrustWorkerAgentName;
					case Guids.Agent.WmiWorkerAgentGuidString:
						return WmiWorkerAgentName;
					default:
						return string.Empty;
				}
			}
		}

		public static class Apm
		{
			public const string TotalCount = "TotalCount";
			public const string TotalAuditGroupCount = "TotalAuditGroupCount";
			public const string SimpleQueryNonAdhoc = "SimpleQuery NonAdhoc";
			public const string SimpleQueryAdhoc = "SimpleQuery Adhoc";
			public const string ComplexQueryNonAdhoc = "ComplexQuery NonAdhoc";
			public const string ComplexQueryAdhoc = "ComplexQuery Adhoc";
			public const string SimpleLonRunningQueryNonAdhoc = "SimpleLonRunningQuery NonAdhoc";
			public const string SimpleLonRunningQueryAdhoc = "SimpleLonRunningQuery Adhoc";
			public const string ComplexLonRunningQueryNonAdhoc = "ComplexLonRunningQuery NonAdhoc";
			public const string ComplexLonRunningQueryAdhoc = "ComplexLonRunningQuery Adhoc";

			public const string AuditUnitOfMeasure = "audit(s)";
		}

		public static class Tab
		{
			public const string PerformanceDashboard = @"Performance Dashboard";
			public const string ServerHealth = @"Server Health";
			public const string QualityOfService = @"Quality of Service";
			public const string UserExperience = @"User Experience";
			public const string InfrastructurePerformance = @"Infrastructure Performance";
			public const string RecoverabilityIntegrity = @"Recoverability/Integrity";
			public const string Uptime = @"Uptime";
			public const string Configuration = @"Configuration";
			public const string BackfillConsole = @"Backfill Console";
			public const string EnvironmentCheck = @"Environment Check";
			public const string MaintenanceScheduling = @"Maintenance Schedules";

			public static readonly IList<string> AllChildTabNames = new[]
			{
				ServerHealth,
				QualityOfService,
				UserExperience,
				InfrastructurePerformance,
				RecoverabilityIntegrity,
				Uptime,
				Configuration,
				BackfillConsole,
				EnvironmentCheck,
				MaintenanceScheduling
			};
		}

		public static class Queuing
		{
			public static readonly string RecurringQueue = "recurring";
			public static readonly string TaskManagerLogicCreateTasks = "task_manager_logic_create_tasks";
			public static readonly string Manager = "manager";
			public static readonly string Worker = "worker";
			public static readonly string DefaultQueue = "default";
		}

		public static class LogCategory
		{
			public static readonly string Week = "Week";
			public static readonly string Hour = "Hour";
			public static readonly string Metric = "Metric";
			public static readonly string MetricData = "Metric Data";
			public static readonly string Category = "Category";
			public static readonly string CategoryScore = "Category Score";
			public static readonly string Event = "Event";
			public static readonly string DatabaseDeployment = "DatabaseDeployment";
			public static readonly string UserExperience = "User Experience";
			public static readonly string Audit = "Audit";
			public static readonly string System = "System";
			public static readonly string DataGrid = "Data Grid";
			public static readonly string ProcessControl = "ProcessControl";
			public static readonly string RecoverabilityIntegrity = "Recoverability Integrity";
		}

		public static class Database
		{
			public const string Edds = "EDDS";
			public const string EddsQoS = "EDDSQoS";
			public const string EddsPerformance = "EDDSPerformance";
			public const string PdbResource = "PDBResource";  // if this db name is changed also update `integration-CompileResourceDbScripts.ps1`
			public const string Msdb = "msdb";
			public const string Master = "Master";
			public const string EddsWorkspacePrefix = "EDDS{0}";

			public const string EddsdboSchema = "eddsdbo";

			public const string GlassRunLogTable = "QoS_GlassRunLog";
			public const string CasesToAuditTable = "QoS_CasesToAudit";
			public const string SourceDatetimeTable = "QoS_SourceDatetime";
			public const string VarscatOutputCumulativeTable = "QoS_VarscatOutputCumulative";
			public const string VarscatOutputDetailCumulativeTable = "QoS_VarscatOutputDetailCumulative";
			public const string VarscatOutputTable = "QoS_VarscatOutput";
			public const string VarscatOutputDetailTable = "QoS_VarscatOutputDetail";

			public const string AuditStartDateColumn = "AuditStartDate"; // QoS_CasesToAudit column
			public const string LogTimestampColumn = "LogTimestampUTC"; // QoS_GlassRunLog column
			public const string SummaryDayHourColumn = "SummaryDayHour"; // Various table datetime columns
			public const string QuotidianColumn = "quotidian"; // QoS_SourceDatetime column
			public const string TimestampColumn = "Timestamp"; // QoS_VarscatOutputDetail column

			public const string BackupAndDBCCMonLauncherSproc = EddsdboSchema + ".QoS_BackupAndDBCCCheckMonLauncher";
			public const string BackupAndDBCCMonLauncherSproc_Test = EddsdboSchema + ".QoS_BackupAndDBCCCheckMonLauncherTest";
		}

		public static class Application
		{
			public const string PerformanceDashboard = @"Performance Dashboard";

			public static readonly string CoreAssembly = $"{nameof(kCura)}.{nameof(PDB)}.{nameof(Core)}";
			public static readonly string DataAssembly = $"{nameof(kCura)}.{nameof(PDB)}.Data";
			public static readonly string ServiceAssembly = $"{nameof(kCura)}.{nameof(PDB)}.Service";
			public static readonly string DataGridServiceAssembly = $"{nameof(kCura)}.{nameof(PDB)}.Service.DataGrid";
			public static readonly string AgentAssembly = $"{nameof(kCura)}.{nameof(PDB)}.Agent";
		}

		public static class Configuration
		{
			public const string ManagerAgentInRelativity = "ManangerAgentInRelativity";
			public const string WorkerAgentInRelativity = "WorkerAgentInRelativity";
		}

		public static class Testing
		{
			public const string BackupSheet = "MockBackup";
			public const string HourSheet = "MockHour";
			public const string DbccSheet = "MockDBCC";
			public const string DatabasesCheckedSheet = "MockDB";
			public const string ServerSheet = "MockServer";
		}
	}
}
