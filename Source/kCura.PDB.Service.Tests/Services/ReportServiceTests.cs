namespace kCura.PDB.Service.Tests.Services
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Linq;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Models.BISSummary.Models;
	using kCura.PDB.Service.BISSummary;
	using Moq;
	using NUnit.Framework;

	[TestFixture, Category("Unit")]
	public class ReportServiceTests
	{
		[SetUp]
		public void SetUp()
		{
			this.reportRepoistory = new Mock<IReportRepository>();
			this.sqlServerRepository = new Mock<ISqlServerRepository>();
			this.sqlServerRepository.SetupGet(r => r.ReportRepository).Returns(this.reportRepoistory.Object);
			this.service = new BestInServiceReportingService(this.sqlServerRepository.Object);
		}

		private Mock<IReportRepository> reportRepoistory;
		private Mock<ISqlServerRepository> sqlServerRepository;
		private BestInServiceReportingService service;

		[Test]
		public void BaseService_GetScores()
		{
			//Arrange
			var model = new List<QualityOfServiceModel>
			{
				new QualityOfServiceModel {
					ServerArtifactId = 1,
					ServerName = "Test",
					OverallScore = 50,
					WeeklyScore = 97,
					UserExperienceScore = 100,
					WeeklyUserExperienceScore = 95,
					SystemLoadScore = 75,
					WeeklySystemLoadScore = 94,
					IntegrityScore = 25,
					WeeklyIntegrityScore = 85,
					UptimeScore = 0,
					WeeklyUptimeScore = 40
				}
			};

			this.reportRepoistory.Setup(x => x.ReadQuality(It.IsAny<int>())).Returns(model);

			//Act
			var results = service.GetQualityOfServiceScores().FirstOrDefault();

			//Assert
			Assert.AreEqual(1, results.ServerArtifactId);
			Assert.AreEqual("Test", results.ServerName);
			Assert.AreEqual(100, results.UserExperienceScore);
			Assert.AreEqual(95, results.WeeklyUserExperienceScore);
			Assert.AreEqual(75, results.SystemLoadScore);
			Assert.AreEqual(94, results.WeeklySystemLoadScore);
			Assert.AreEqual(25, results.IntegrityScore);
			Assert.AreEqual(85, results.WeeklyIntegrityScore);
			Assert.AreEqual(0, results.UptimeScore);
			Assert.AreEqual(40, results.WeeklyUptimeScore);
			Assert.AreEqual(50, results.OverallScore);
			Assert.AreEqual(97, results.WeeklyScore);
		}

		[Test]
		public void BaseService_GetScores_HandlesEmptyDataset()
		{
			//Arrange
			var model = new List<QualityOfServiceModel>();

			this.reportRepoistory.Setup(x => x.ReadQuality(It.IsAny<int>())).Returns(model);

			//Act
			var results = this.service.GetQualityOfServiceScores();

			//Assert
			Assert.AreEqual(0, results.Count, "Incorrect count of scores returned");
		}

		[Test]
		public void BaseService_RateUptime()
		{
			//Arrange
			var data = new DataTable();
			data.Columns.Add("SummaryDayHour", typeof(DateTime));
			data.Columns.Add("HoursDown", typeof(decimal));
			data.Columns.Add("UptimeScore", typeof(decimal));

			var row = data.NewRow();
			row["UptimeScore"] = 50;
			row["HoursDown"] = 1;
			row["SummaryDayHour"] = DateTime.Now;
			data.Rows.Add(row);

			this.reportRepoistory.Setup(x => x.GetUptimeDetail(true, false, 0)).Returns(data);

			//Act
			var results = service.ListWorstUptimeDays(0);

			//Assert
			Assert.AreEqual(1, results.Count());
			Assert.AreEqual(1, results.First().HoursDown);
			Assert.AreEqual(DateTime.Now.Date, results.First().ScoreDate.Date);
		}

		[Test]
		public void BaseService_RateUptime_HandlesEmptyDataset()
		{
			//Arrange

			var data = new DataTable();
			data.Columns.Add("SummaryDayHour", typeof(DateTime));
			data.Columns.Add("HoursDown", typeof(Int32));
			data.Columns.Add("UptimeScore", typeof(decimal));

			this.reportRepoistory.Setup(x => x.GetUptimeDetail(true, false, 0)).Returns(data);

			//Act
			var results = service.ListWorstUptimeDays(0);

			//Assert
			Assert.AreEqual(0, results.Count());
		}

		[Test]
		public void BaseService_ListAllServers()
		{
			//Arrange
			var data = new DataTable();
			data.Columns.Add("ArtifactID", typeof(int));
			data.Columns.Add("ServerName", typeof(string));

			var row = data.NewRow();
			row["ArtifactID"] = 1;
			row["ServerName"] = "Server 1";
			data.Rows.Add(row);

			this.reportRepoistory.Setup(x => x.ReadServers()).Returns(data);

			//Act
			var results = service.ListAllServers();

			//Assert
			Assert.AreEqual(1, results.Count());
			Assert.AreEqual(1, results.First().ArtifactId);
			Assert.AreEqual("Server 1", results.First().Name);
		}

		[Test]
		public void BaseService_ListAllServers_HandlesEmptyDataset()
		{
			//Arrange
			var data = new DataTable();
			data.Columns.Add("ArtifactID", typeof(int));
			data.Columns.Add("ServerName", typeof(string));

			this.reportRepoistory.Setup(x => x.ReadServers()).Returns(data);

			//Act
			var results = this.service.ListAllServers();

			//Assert
			Assert.AreEqual(0, results.Count());
		}

		[Test]
		public void BaseService_GetTabArtifactId()
		{
			//Arrange
			var db = new Mock<ISqlServerRepository>();
			db.Setup(x => x.ReadTabArtifactId(It.IsAny<string>())).Returns(0);

			//Act
			var results = this.service.GetTabArtifactId("Test Tab");

			//Assert
			Assert.AreEqual(0, results);
		}
	}
}
