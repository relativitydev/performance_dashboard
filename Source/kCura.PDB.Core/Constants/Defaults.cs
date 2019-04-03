namespace kCura.PDB.Core.Constants
{
	using System;
	using System.Collections.Generic;
	using kCura.PDB.Core.Models.RecoverabilityIntegrity;

	public static class Defaults
	{
		public const int BackfillDays = -7;

		public static class Queuing
		{
			public static readonly int DefaultServerTimeout = 30 * 60; // 30 minutes (in seconds)
			public static readonly int DefaultServerExecution = 450; // 7.5 minutes (in seconds)
			public static readonly int MinServerExecution = 10; // (in seconds)
			public static readonly int DefaultServerSleep = 5; // (in seconds)
			public static readonly int MinServerSleep = 1; // (in seconds)
			public static readonly int DefaultQueuePollInterval = 2; // (in seconds)
			public static readonly int MinQueuePollInterval = 1; // (in seconds)
			public static readonly int DefaultWorkerCount = 4;
		}

		public static class Log
		{
			public const int DefaultLogFileSize = 10000;
			public const int MinLogFileSize = 1;
		}

		public static class Database
		{
			public const int ConnectionTimeout = 5 * 60;
			public const int CleanupTimeout = 900;
			public const int WaitMonitorTimeout = 900;
			public const int TuningForkTimeout = 900;
			public const int RoundHouseTimeout = 3600;
			public const int DeleteBatchSize = 1000;
		}

		public static class Agent
		{
			public const int MetricManagerInterval = 5;
			public const int QoSManagerInterval = 300;
			public const int QoSWorkerInterval = 5;
			public const int WmiWorkerInterval = 5;

			public const int EnabledStatusRetryCount = 5;
			public const int EnabledStatusDelaySeconds = 45;

			public static int IntervalFromGuid(Guid agentTypeGuid)
			{
				switch (agentTypeGuid.ToString().ToUpper())
				{
					case Guids.Agent.MetricManagerAgentGuidString:
						return MetricManagerInterval;
					case Guids.Agent.QoSManagerAgentGuidString:
						return QoSManagerInterval;
					case Guids.Agent.QosWorkerAgentGuidString:
						return QoSWorkerInterval;
					case Guids.Agent.WmiWorkerAgentGuidString:
						return WmiWorkerInterval;
					default:
						return 25;
				}
			}
		}

		public class Scores
		{
			public const decimal OneHundred = 100.0m;
			public const decimal Zero = 0.0m;

			public const decimal Uptime = OneHundred;
			public const decimal UserExperience = OneHundred;

			public const int ArrivalRateCountThreshold = 8; // Min hours required
			public const int ConcurrencyCountThreshold = 2; // Min hours required

			public const int DefaultPassScore = 90;
			public const int DefaultWarnScore = 80;
		}

		public class RecoverabilityIntegrity
		{
			public const int DatabaseBatchSize = 250;
			public const int WindowInDays = 9;
			public static readonly IList<BackupType> AllSupportedBackupTypes = new[] { BackupType.Full, BackupType.Differential, BackupType.Log };
			public static readonly IList<BackupType> FullAndDiffBackupTypes = new[] { BackupType.Full, BackupType.Differential };
		}
	}
}
