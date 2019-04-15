namespace kCura.PDB.Service.Services
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Core.Models;

	public static class ServerTypeMapper
	{
		public static IEnumerable<ServerType> GetServerTypes(CategoryType categoryType)
		{
			var serverTypes = Enum.GetValues(typeof(ServerType)).Cast<ServerType>();

			switch (categoryType)
			{
				case CategoryType.Uptime:
					return new ServerType[] { };
				case CategoryType.InfrastructurePerformance:
					return new[] { ServerType.Database }; // Temp fix
					
					// return serverTypes;
				case CategoryType.RecoverabilityIntegrity:
					return new[] { ServerType.Database };
				case CategoryType.UserExperience:
					return new[] { ServerType.Database };
			}

			throw new ArgumentException($"CategoryType {categoryType} cannot be mapped.");
		}

		public static IEnumerable<ServerType> GetServerTypes(MetricType metricType)
		{
			return MapMetricTypeToServerTypes(metricType).SelectMany(st => st).Distinct();
		}

		private static IEnumerable<IEnumerable<ServerType>> MapMetricTypeToServerTypes(MetricType metricType)
		{
			var serverTypes = Enum.GetValues(typeof(ServerType)).Cast<ServerType>();

			yield return GetMetricTypesForAnyServer().Any(mt => mt == metricType)
				? serverTypes
				: new ServerType[] { };

			yield return GetMetricTypesForAgentServer().Any(mt => mt == metricType)
				? new[] { ServerType.Agent }
				: new ServerType[] { };

			yield return GetMetricTypesForAnyDatabaseServer().Any(mt => mt == metricType)
				? new[] { ServerType.Database }
				: new ServerType[] { };
		}

		private static IEnumerable<MetricType> GetMetricTypesForAnyServer()
		{
			// Temp fix until we can figure out IP issues
			yield break;

			// yield return MetricType.Cpu;
			// yield return MetricType.Ram;
		}

		private static IEnumerable<MetricType> GetMetricTypesForAgentServer()
		{
			yield return MetricType.NumberOfAgentsPerServer;
		}

		private static IEnumerable<MetricType> GetMetricTypesForAnyDatabaseServer()
		{
			yield return MetricType.AuditAnalysis;
			yield return MetricType.BackupGaps;
			yield return MetricType.DbccGaps;
			yield return MetricType.Rpo;
			yield return MetricType.Rto;
			yield return MetricType.BackupFrequency;
			yield return MetricType.DbccFrequency;
			yield return MetricType.BackupCoverage;
			yield return MetricType.DbccCoverage;
			yield return MetricType.SqlServerWaits;
			yield return MetricType.SqlServerPageOuts;
			yield return MetricType.SqlServerLatencyForDataFile;
			yield return MetricType.SqlServerLatencyForLogFile;
			yield return MetricType.SqlServerVirtualLogFileCount;

			// Temp fix until we can figure out IP issues
			yield return MetricType.Cpu;
			yield return MetricType.Ram;
		}
	}
}
