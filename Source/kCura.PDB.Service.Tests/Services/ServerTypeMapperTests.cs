namespace kCura.PDB.Service.Tests.Services
{
	using System;
	using System.Linq;
	using Core.Models;
	using NUnit.Framework;
	using Service.Services;

	[TestFixture]
	[Category("Unit")]
	public class ServerTypeMapperTests
	{
		[Test]
		[TestCase(CategoryType.Uptime, new ServerType[] { })]
		[TestCase(CategoryType.RecoverabilityIntegrity, new[] { ServerType.Database })]
		[TestCase(CategoryType.UserExperience, new[] { ServerType.Database })]
		[TestCase(CategoryType.InfrastructurePerformance, new[] { ServerType.Database })]
		public void ServerTypeMapper_GetServerTypes_CategoryType(CategoryType categoryType, ServerType[] expecteServerTypes)
		{
			//Act
			var results = ServerTypeMapper.GetServerTypes(categoryType);

			//Assert
			Assert.That(results.OrderBy(r => r), Is.EqualTo(expecteServerTypes.OrderBy(r => r)));
		}

		//[Test]
		//[TestCase(CategoryType.InfrastructurePerformance)]
		public void ServerTypeMapper_GetServerType_CategoryType_AllServerTypes(CategoryType categoryType)
		{
			var expecteServerTypes = Enum.GetValues(typeof(ServerType)).Cast<ServerType>().ToArray();
			ServerTypeMapper_GetServerTypes_CategoryType(categoryType, expecteServerTypes);
		}

		[Test]
		public void ServerTypeMapper_GetServerTypes_CategoryType_CannotBeMapped()
		{
			//Act & Assert
			Assert.Throws<ArgumentException>(() => ServerTypeMapper.GetServerTypes((CategoryType)0), "CategoryType 0 cannot be mapped.");
		}

		[Test]
		// Serverless
		[TestCase(MetricType.WebUptime, new ServerType[] { })]
		[TestCase(MetricType.AgentUptime, new ServerType[] { })]
		// Agent
		[TestCase(MetricType.NumberOfAgentsPerServer, new[] { ServerType.Agent })]
		// Database
		[TestCase(MetricType.AuditAnalysis, new[] { ServerType.Database })]
		[TestCase(MetricType.BackupGaps, new[] { ServerType.Database })]
		[TestCase(MetricType.DbccGaps, new[] { ServerType.Database })]
		[TestCase(MetricType.Rpo, new[] { ServerType.Database })]
		[TestCase(MetricType.Rto, new[] { ServerType.Database })]
		[TestCase(MetricType.BackupFrequency, new[] { ServerType.Database })]
		[TestCase(MetricType.DbccFrequency, new[] { ServerType.Database })]
		[TestCase(MetricType.BackupCoverage, new[] { ServerType.Database })]
		[TestCase(MetricType.DbccCoverage, new[] { ServerType.Database })]
		[TestCase(MetricType.SqlServerWaits, new[] { ServerType.Database })]
		[TestCase(MetricType.SqlServerPageOuts, new[] { ServerType.Database })]
		[TestCase(MetricType.SqlServerLatencyForDataFile, new[] { ServerType.Database })]
		[TestCase(MetricType.SqlServerLatencyForLogFile, new[] { ServerType.Database })]
		[TestCase(MetricType.SqlServerVirtualLogFileCount, new[] { ServerType.Database })]
		[TestCase(MetricType.Cpu, new[] { ServerType.Database })] // Temp fix
		[TestCase(MetricType.Ram, new[] { ServerType.Database })] // Temp fix
		public void ServerTypeMapper_GetServerTypes_MetricType(MetricType metricType, ServerType[] expecteServerTypes)
		{
			//Act
			var results = ServerTypeMapper.GetServerTypes(metricType);

			//Assert
			Assert.That(results.OrderBy(r => r), Is.EqualTo(expecteServerTypes.OrderBy(r => r)));
		}

		//[Test]
		//	[TestCase(MetricType.Cpu)]
		//	[TestCase(MetricType.Ram)]
		public void ServerTypeMapper_GetServerType_MetricType_AllServerTypes(MetricType metricType)
		{
			var expecteServerTypes = Enum.GetValues(typeof(ServerType)).Cast<ServerType>().ToArray();
			ServerTypeMapper_GetServerTypes_MetricType(metricType, expecteServerTypes);
		}
	}
}
