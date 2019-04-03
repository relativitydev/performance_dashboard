namespace kCura.PDB.Service.Tests.Logic.Audits
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Interfaces.Audits;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.Audits;
	using kCura.PDB.Core.Models.Reports;
	using kCura.PDB.Service.Audits;
	using Moq;
	using NUnit.Framework;

	[TestFixture, Category("Unit")]
	public class WorkspaceAuditReporterTests
	{
		[SetUp]
		public void Setup()
		{
			this.userExperienceReportRepository = new Mock<IUserExperienceReportRepository>();
			this.searchAuditReporter = new Mock<ISearchAuditReporter>();
			this.workspaceAuditApmReporter = new Mock<IWorkspaceAuditApmReporter>();
			this.workspaceAuditReporter = new WorkspaceAuditReporter(this.userExperienceReportRepository.Object, searchAuditReporter.Object, this.workspaceAuditApmReporter.Object);
		}

		private Mock<IUserExperienceReportRepository> userExperienceReportRepository;
		private Mock<ISearchAuditReporter> searchAuditReporter;
		private Mock<IWorkspaceAuditApmReporter> workspaceAuditApmReporter;
		private WorkspaceAuditReporter workspaceAuditReporter;

		[Test]
		public async Task ReportWorkspaceSearchAudits()
		{
			// Arrange
			var searchAudits =
				Enumerable.Range(0, 8)
				.Select(i => new SearchAuditGroup
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
			this.userExperienceReportRepository.Setup(r => r.CreateUserExperienceWorkspaceRecord(It.IsAny<IList<UserExperienceWorkspace>>()))
				.Returns(Task.Delay(10));
			this.searchAuditReporter.Setup(r => r.ReportWorkspaceSearchAudits(It.IsAny<IList<SearchAuditGroup>>(), It.IsAny<Hour>(), It.IsAny<int>()))
				.Returns(Task.Delay(10));

			// Act
			await this.workspaceAuditReporter.ReportWorkspaceAudits(searchAudits, new Hour(), 333);

			// Assert
			this.userExperienceReportRepository.Setup(r => r.CreateUserExperienceWorkspaceRecord(It.IsAny<IList<UserExperienceWorkspace>>()));
		}

		[Test]
		public void GroupSearchAuditsForWorkspaceReport()
		{
			// Arrange
			var searchAudits =
				Enumerable.Range(0, 8)
				.Select(i => new SearchAuditGroup {
					Audits = new List<SearchAudit> {
						new SearchAudit
						{
							Audit = new Audit { ArtifactID = i % 4, UserID = i % 2, ExecutionTime = 10, WorkspaceId = 123 },
							Search = new Search(),
							IsComplex = false
						}
					}
				})
				.ToList();

			// Act
			var result = WorkspaceAuditReporter.GroupSearchAuditsForWorkspaceReport(searchAudits, new Hour(), 333);

			//Assert
			Assert.That(result.Count, Is.EqualTo(4)); // Group by ArtifactID + UserID
		}

		[Test]
		public void GroupSearchAuditsForWorkspaceReport_AdHocSearches()
		{
			// Arrange
			var searchAudits =
				Enumerable.Range(0, 8)
				.Select(i => new SearchAuditGroup
				{
					Audits = new List<SearchAudit>
					{ 
						new SearchAudit
						{
							Audit = new Audit { ArtifactID = i % 4, UserID = i % 2, ExecutionTime = 10, WorkspaceId = 123 },
							Search = null,
							IsComplex = false
						}
					}
				})
				.ToList();

			// Act
			var result = WorkspaceAuditReporter.GroupSearchAuditsForWorkspaceReport(searchAudits, new Hour(), 333);

			//Assert
			Assert.That(result.Count, Is.EqualTo(4)); // Group by ArtifactID + UserID
		}
	}
}
