namespace kCura.PDB.Service.Tests.Logic.Metrics.UserExperience
{
	using System.Collections.Generic;
	using System.Linq;
	using kCura.PDB.Core.Enumerations;
	using kCura.PDB.Core.Models.Audits;
	using kCura.PDB.Service.Audits;
	using Moq;
	using NUnit.Framework;

	[TestFixture, Category("Unit")]
	public class AuditBatchAnalyzerTests
	{
		private AuditBatchAnalyzer auditBatchAnalyzer;

		[SetUp]
		public void Setup()
		{
			this.auditBatchAnalyzer = new AuditBatchAnalyzer();
		}

		[Test]
		public void GetBatchResults()
		{
			var auditA = new SearchAuditTestArguments
			{
				UserId = 1,
				ExecutionTime = 2,
				IsComplex = true
			};
			var auditB = new SearchAuditTestArguments
			{
				UserId = 1,
				ExecutionTime = 2,
				IsComplex = false
			};
			var auditC = new SearchAuditTestArguments
			{
				UserId = 2,
				ExecutionTime = 1,
				IsComplex = false
			};
			var auditD = new SearchAuditTestArguments
			{
				UserId = 3,
				IsComplex = false
			};
			var givenArgs = new List<SearchAuditTestArguments>
			{
				auditA,
				auditB,
				auditC,
				auditD
			};

			var batchId = 12;
			var expectedResult = givenArgs.GroupBy(a => a.UserId).Select(group => new SearchAuditBatchResult
			{
				TotalComplexQueries = group.Count(a => a.IsComplex),
				TotalLongRunningQueries = group.Count(a => SearchAnalysisService.IsLongRunning(a.IsComplex, a.ExecutionTime)),
				TotalSimpleLongRunningQueries = group.Count(a => !a.IsComplex && SearchAnalysisService.IsLongRunning(a.IsComplex, a.ExecutionTime)),
				TotalQueries = group.Count(),
				UserId = group.Key,
				BatchId = batchId,
				TotalExecutionTime = group.Sum(a => a.ExecutionTime ?? 0)
			}).ToList();


			var searchAudits = givenArgs.Select(s => new SearchAuditGroup
			{
				Audits = new List<SearchAudit>
				{ 
					new SearchAudit
					{
						Audit = new Audit
						{
							Action = AuditActionId.DocumentQuery,
							ExecutionTime = s.ExecutionTime,
							UserID = s.UserId
						},
						Search = new Search(),
						IsComplex = s.IsComplex
					}
				}
			});

			var result = this.auditBatchAnalyzer.GetBatchResults(searchAudits, batchId);

			Assert.That(result, Is.Not.Empty);
			Assert.That(result.Count, Is.EqualTo(expectedResult.Count));
			for (int i = 0; i < result.Count; ++i)
			{
				Assert.That(result[i].UserId, Is.EqualTo(expectedResult[i].UserId));
				Assert.That(result[i].TotalComplexQueries, Is.EqualTo(expectedResult[i].TotalComplexQueries));
				Assert.That(result[i].TotalLongRunningQueries, Is.EqualTo(expectedResult[i].TotalLongRunningQueries));
				Assert.That(result[i].TotalSimpleLongRunningQueries, Is.EqualTo(expectedResult[i].TotalSimpleLongRunningQueries));
				Assert.That(result[i].TotalQueries, Is.EqualTo(expectedResult[i].TotalQueries));
				Assert.That(result[i].BatchId, Is.EqualTo(expectedResult[i].BatchId));
				Assert.That(result[i].TotalExecutionTime, Is.EqualTo(expectedResult[i].TotalExecutionTime));
			}
		}

		public class SearchAuditTestArguments
		{
			public int UserId { get; set; }
			public bool IsComplex { get; set; }
			public long? ExecutionTime { get; set; }
		}
	}
}
