namespace kCura.PDB.Core.Constants
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Models;

	public static class MetricConstants
	{
		// TODO: Replace with this when more metric types are implemented.
		// Enum.GetValues(typeof(MetricType)).Cast<MetricType>().ToList();
		public static readonly MetricType[] ActiveMetricTypes =
		{
			// Uptime
			MetricType.AgentUptime,
			MetricType.WebUptime,
			
			// UX
			MetricType.AuditAnalysis,

			// IP
			MetricType.Ram,
			MetricType.SqlServerWaits,
			MetricType.SqlServerVirtualLogFileCount,
			
			// R/I
			MetricType.BackupGaps,
			MetricType.DbccGaps,
			MetricType.Rpo,
			MetricType.Rto,
			MetricType.DbccFrequency,
			MetricType.DbccCoverage,
			MetricType.BackupFrequency,
			MetricType.BackupCoverage,
		};

		public static readonly CategoryType[] ActiveCategoryTypes = Enum.GetValues(typeof(CategoryType)).Cast<CategoryType>().ToArray();

		public static readonly ILookup<MetricType, EventSourceType> MetricPrerequisites = new[]
		{
			new
			{
				MetricType = MetricType.AuditAnalysis,
				EventSourceTypes = new[] { EventSourceType.CreateAuditProcessingBatches }
			},
			new
			{
				MetricType = MetricType.BackupGaps,
				EventSourceTypes = new[] { EventSourceType.ProcessRecoverabilityForServer }
			},
		}
		.SelectMany(i => i.EventSourceTypes.Select(et => new { i.MetricType, EventSourceType = et }))
		.ToLookup(k => k.MetricType, v => v.EventSourceType);

		public static readonly ILookup<CategoryType, MetricType> CategoryTypesToMetricTypes = new[]
		{
			new
			{
				CategoryType = CategoryType.UserExperience,
				MetricTypes = new[] { MetricType.AuditAnalysis }
			},
			new
			{
				CategoryType = CategoryType.InfrastructurePerformance,
				MetricTypes = new[]
				{
					MetricType.Ram,
					MetricType.Cpu,
					MetricType.NumberOfAgentsPerServer,
					MetricType.SqlServerWaits,
					MetricType.SqlServerPageOuts,
					MetricType.SqlServerLatencyForDataFile,
					MetricType.SqlServerLatencyForLogFile,
					MetricType.SqlServerVirtualLogFileCount
				}
			},
			new
			{
				CategoryType = CategoryType.RecoverabilityIntegrity,
				MetricTypes = new[]
				{
					MetricType.BackupGaps,
					MetricType.DbccGaps,
					MetricType.Rpo,
					MetricType.Rto,
					MetricType.BackupFrequency,
					MetricType.DbccFrequency,
					MetricType.BackupCoverage,
					MetricType.DbccCoverage
				}
			},
			new
			{
				CategoryType = CategoryType.Uptime,
				MetricTypes = new[] { MetricType.WebUptime, MetricType.AgentUptime }
			},
		}
		.SelectMany(i => i.MetricTypes.Select(mt => new { i.CategoryType, MetricType = mt }))
		.ToLookup(k => k.CategoryType, v => v.MetricType);

		public static readonly IDictionary<MetricType, CategoryType> MetricTypesToCategoryTypes =
			CategoryTypesToMetricTypes
				.SelectMany(ctmt => ctmt.Select(mt => new { CategoryType = ctmt.Key, MetricType = mt }))
				.ToDictionary(k => k.MetricType, v => v.CategoryType);
	}
}
