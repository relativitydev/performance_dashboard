namespace kCura.PDB.Service.Tests.Logic.Audits
{
	using System.Linq;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.Reports;
	using Moq;
	using NUnit.Framework;
	using System.Collections.Generic;
	using kCura.PDB.Core.Models.Audits;
	using kCura.PDB.Service.Audits;

	[TestFixture, Category("Unit")]
	public class SearchAuditReporterTests
	{
		[SetUp]
		public void Setup()
		{
			this.userExperienceReportRepository = new Mock<IUserExperienceReportRepository>();
		}

		private Mock<IUserExperienceReportRepository> userExperienceReportRepository;


		[Test, Explicit("TODO - investigate and fix null references"), Category("Explicit")]
		public async Task ReportWorkspaceSearchAudits()
		{
			// Arrange
			var searchAudits =
				Enumerable.Range(0, 8)
				.Select(i =>
				new SearchAuditGroup
				{
					Audits = new List<SearchAudit>
					{
						new SearchAudit
						{
							Audit = new Audit { ArtifactID = i % 4, UserID = i % 2, ExecutionTime = 10, WorkspaceId = 123 },
							Search = new Search(),
							IsComplex = false
						}
					}
				}).ToList();
			this.userExperienceReportRepository.Setup(r => r.CreateUserExperienceSearchRecord(It.IsAny<IList<UserExperienceSearch>>()));

			// Act
			var reporter = new SearchAuditReporter(this.userExperienceReportRepository.Object);
			await reporter.ReportWorkspaceSearchAudits(searchAudits, new Hour(), 333);

			// Assert
			this.userExperienceReportRepository.Setup(r => r.CreateUserExperienceSearchRecord(It.IsAny<IList<UserExperienceSearch>>()));
		}

		[Test]
		public void GroupSearchAuditsForReport()
		{
			// Arrange
			var searchAudits =
				Enumerable.Range(0, 8)
				.Select(i =>
				new SearchAuditGroup
				{
					Audits = new List<SearchAudit>
					{
						new SearchAudit
						{
							Audit = new Audit { ArtifactID = i % 4, UserID = i % 2, ExecutionTime = 10, WorkspaceId = 123 },
							Search = new Search(),
							IsComplex = false
						}
					}
				}).ToList();

			// Act
			var result = SearchAuditReporter.GroupSearchAuditsForReport(searchAudits, new Hour(), 333);

			// Assert
			Assert.That(result.Count, Is.EqualTo(4)); // Group by ArtifactID & UserID
		}
	}
}
