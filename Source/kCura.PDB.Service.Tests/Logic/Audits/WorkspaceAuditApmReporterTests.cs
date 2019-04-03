namespace kCura.PDB.Service.Tests.Logic.Audits
{
	using System;
	using System.Collections.Generic;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Interfaces.Apm;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.Audits;
	using kCura.PDB.Service.Audits;
	using Moq;
	using NUnit.Framework;

	[TestFixture]
	public class WorkspaceAuditApmReporterTests
	{
		[SetUp]
		public void Setup()
		{
			this.apmMetricsService = new Mock<IApmMetricsService>();
			this.workspaceAuditApmReporter = new WorkspaceAuditApmReporter(this.apmMetricsService.Object);
		}

		private Mock<IApmMetricsService> apmMetricsService;
		private WorkspaceAuditApmReporter workspaceAuditApmReporter;
		private const int HourId = 1234;
		private const int WorkspaceId = 5555;

		[Test]
		[TestCase(Names.Apm.TotalCount, 12, TestName = Names.Apm.TotalCount)]
		[TestCase(Names.Apm.TotalAuditGroupCount, 8, TestName = Names.Apm.TotalAuditGroupCount)]
		[TestCase(Names.Apm.SimpleQueryNonAdhoc, 2, TestName = Names.Apm.SimpleQueryNonAdhoc)]
		[TestCase(Names.Apm.SimpleQueryAdhoc, 2, TestName = Names.Apm.SimpleQueryAdhoc)]
		[TestCase(Names.Apm.ComplexQueryNonAdhoc, 2, TestName = Names.Apm.ComplexQueryNonAdhoc)]
		[TestCase(Names.Apm.ComplexQueryAdhoc, 2, TestName = Names.Apm.ComplexQueryAdhoc)]
		[TestCase(Names.Apm.SimpleLonRunningQueryNonAdhoc, 1, TestName = Names.Apm.SimpleLonRunningQueryNonAdhoc)]
		[TestCase(Names.Apm.SimpleLonRunningQueryAdhoc, 1, TestName = Names.Apm.SimpleLonRunningQueryAdhoc)]
		[TestCase(Names.Apm.ComplexLonRunningQueryNonAdhoc, 1, TestName = Names.Apm.ComplexLonRunningQueryNonAdhoc)]
		[TestCase(Names.Apm.ComplexLonRunningQueryAdhoc, 1, TestName = Names.Apm.ComplexLonRunningQueryAdhoc)]
		public void WorkspaceAuditApmReporter_ReportAuditDataToApm(string metricName, int expectedValue)
		{
			// Arrange
			this.apmMetricsService.Setup(s => s.RecordGauge(
				It.IsAny<string>(),
				It.IsAny<int>(),
				Names.Apm.AuditUnitOfMeasure,
				It.IsAny<string>(),
				It.IsAny<object>()));

			// Act
			this.workspaceAuditApmReporter.ReportAuditDataToApm(SearchAuditGroups, new Hour { Id = HourId, HourTimeStamp = DateTime.UtcNow });

			// Each metric should have two audits, one long running and one not
			this.apmMetricsService.Verify(s => s.RecordGauge(
				metricName, expectedValue, Names.Apm.AuditUnitOfMeasure, $"{HourId}-{WorkspaceId}", It.IsAny<object>()));
		}


		private readonly IList<SearchAuditGroup> SearchAuditGroups = new[]
			{
				// SimpleQueryNonAdhocCount (non-long running)
				new SearchAuditGroup
				{
					Audits = new[]
					{
						new SearchAudit
						{
							Audit = new Audit { ExecutionTime = 5, WorkspaceId = WorkspaceId },
							Search = new Search { Name = "Test Search" },
							IsComplex = false
						},
						new SearchAudit
						{
							Audit = new Audit { ExecutionTime = 5 },
							Search = new Search { Name = "Test Search" },
							IsComplex = false
						}
					}
				},
				// SimpleQueryAdhocCount (non-long running)
				new SearchAuditGroup
				{
					Audits = new[]
					{
						new SearchAudit
						{
							Audit = new Audit { ExecutionTime = 5 },
							Search = new Search (),
							IsComplex = false
						},
						new SearchAudit
						{
							Audit = new Audit { ExecutionTime = 5 },
							Search = new Search (),
							IsComplex = false
						}
					}
				},
				// ComplexQueryNonAdhocCount (non-long running)
				new SearchAuditGroup
				{
					Audits = new[]
					{
						new SearchAudit
						{
							Audit = new Audit { ExecutionTime = 5 },
							Search = new Search { Name = "Test Search" },
							IsComplex = true
						},
					}
				},
				// ComplexQueryAdhocCount (non-long running)
				new SearchAuditGroup
				{
					Audits = new[]
					{
						new SearchAudit
						{
							Audit = new Audit { ExecutionTime = 5 },
							Search = new Search(),
							IsComplex = true
						},
					}
				},




				// SimpleLonRunningQueryNonAdhoc
				new SearchAuditGroup
				{
					Audits = new[]
					{
						new SearchAudit
						{
							Audit = new Audit { ExecutionTime = 5000 },
							Search = new Search { Name = "Test Search" },
							IsComplex = false
						},
						new SearchAudit
						{
							Audit = new Audit { ExecutionTime = 5000 },
							Search = new Search { Name = "Test Search" },
							IsComplex = false
						}
					}
				},
				// SimpleLonRunningQueryAdhoc
				new SearchAuditGroup
				{
					Audits = new[]
					{
						new SearchAudit
						{
							Audit = new Audit { ExecutionTime = 5000 },
							Search = new Search (),
							IsComplex = false
						},
						new SearchAudit
						{
							Audit = new Audit { ExecutionTime = 5000 },
							Search = new Search (),
							IsComplex = false
						}
					}
				},
				// ComplexLonRunningQueryNonAdhoc
				new SearchAuditGroup
				{
					Audits = new[]
					{
						new SearchAudit
						{
							Audit = new Audit { ExecutionTime = 9000 },
							Search = new Search { Name = "Test Search" },
							IsComplex = true
						},
					}
				},
				// ComplexLonRunningQueryAdhoc
				new SearchAuditGroup
				{
					Audits = new[]
					{
						new SearchAudit
						{
							Audit = new Audit { ExecutionTime = 9000 },
							Search = new Search(),
							IsComplex = true
						},
					}
				},
			};
	}
}
