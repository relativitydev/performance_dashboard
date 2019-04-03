using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using kCura.PDD.Web.Mapper;

namespace kCura.PDD.Web.Test.Mappers
{
	using kCura.PDB.Core.Models.BISSummary.Models;
	using kCura.PDB.Service.BISSummary;
	using NUnit.Framework;

    [TestFixture]
	public class BestInServiceMapperTests
	{
		[Test]
		public void ToQualityOfServiceReportModel()
		{
			//Arrange
			var scores = new List<QualityOfServiceModel>
					{
						new QualityOfServiceModel {
							ServerName = "Server 1",
							ServerArtifactId = 1,
							OverallScore = 83,
							WeeklyScore = 86,
							UserExperienceScore = 98,
							SystemLoadScore = 64,
							IntegrityScore = 81,
							UptimeScore = 89,
							WeeklyUserExperienceScore = 100,
							WeeklySystemLoadScore = 52,
							WeeklyIntegrityScore = 98,
							WeeklyUptimeScore = 95
						}
					};
			var worstUptime = (new List<WorstUptimeDaysModel>
                {
                    new WorstUptimeDaysModel
                        {
                            HoursDown = 3,
                            ScoreDate = new DateTime(2015, 5, 1, 0, 0, 0),
                        },
                    new WorstUptimeDaysModel
                        {
                            HoursDown = 5,
                            ScoreDate = new DateTime(2015, 6, 1, 0, 0, 0)
                        }
                }).AsQueryable();
			var uptimePct = new UptimePercentageInfo
			{
				QuarterlyUptimePercent = 92.95,
				WeeklyUptimePercent = 89.33
			};
			var integrity = new MissedBackupIntegrityInfo()
			{
				MissedIntegrityChecks = 1,
				MissedBackups = 1,
                BackupCoverageScore = 95,
                BackupFrequencyScore = 94,
                DbccCoverageScore = 93,
                DbccFrequencyScore = 92,
                MaxDataLossMinutes = 120,
                TimeToRecoverHours = 24,
                RPOScore = 90,
                RTOScore = 80
			};

			var service = new Mock<BestInServiceReportingService>();
			service.Setup(x => x.GetQualityOfServiceScores(It.IsAny<int>())).Returns(scores);
			service.Setup(x => x.ListWorstUptimeDays(0)).Returns(worstUptime);
			service.Setup(x => x.SummarizeBackupIntegrityInfo()).Returns(integrity);
			service.Setup(x => x.CalculateUptimePercentages()).Returns(uptimePct);

			//Act
			var model = BestInServiceMapper.ToQualityOfServiceReportModel(service.Object, 0);

			//Assert
			Assert.AreEqual(83, model.OverallScore, "Incorrect quarterly score");
			Assert.AreEqual(86, model.WeeklyScore, "Incorrect weekly score");
			Assert.AreEqual(98, model.UserExperience.QuarterlyScore, "Incorrect quarterly user experience score");
			Assert.AreEqual(64, model.SystemLoad.QuarterlyScore, "Incorrect quarterly system load score");
			Assert.AreEqual(81, model.Backup.Score, "Incorrect quarterly backup score");
			Assert.AreEqual(89, model.Uptime.Score, "Incorrect quarterly uptime score");
			Assert.AreEqual(100, model.UserExperience.WeeklyScore, "Incorrect weekly user experience score");
			Assert.AreEqual(52, model.SystemLoad.WeeklyScore, "Incorrect weekly system load score");
			Assert.AreEqual(94, model.Backup.BackupFrequencyScore, "Incorrect backup frequency score");
			Assert.AreEqual(95, model.Backup.BackupCoverageScore, "Incorrect backup coverage score");
            Assert.AreEqual(92, model.Backup.DbccFrequencyScore, "Incorrect DBCC frequency score");
            Assert.AreEqual(93, model.Backup.DbccCoverageScore, "Incorrect DBCC coverage score");
            Assert.AreEqual(80, model.Backup.RTOScore, "Incorrect RTO score");
            Assert.AreEqual(90, model.Backup.RPOScore, "Incorrect RPO Score");
			Assert.AreEqual(95, model.Uptime.WeeklyScore, "Incorrect weekly uptime score");

			Assert.AreEqual(1, model.UserExperience.Servers.Count(), "Incorrect number of user experience servers");
			Assert.AreEqual(98, model.UserExperience.Servers.First().QuarterlyScore, "Incorrect score for first user experience server");

			Assert.AreEqual(1, model.SystemLoad.Servers.Count(), "Incorrect number of system load servers");
			Assert.AreEqual(64, model.SystemLoad.Servers.First().QuarterlyScore, "Incorrect score for first system load date");

			Assert.AreEqual(1, model.Backup.MissedBackups, "Incorrect number of missed backups");
			Assert.AreEqual(1, model.Backup.MissedIntegrityChecks, "Incorrect number of missed DBCC checks");
            Assert.AreEqual(24, model.Backup.TimeToRecoverHours, "Incorrect time to recover");
            Assert.AreEqual(120, model.Backup.MaxDataLossMinutes, "Incorrect data loss");
			Assert.AreEqual(9, model.BackupWindow, "Incorrect backup window");

			Assert.AreEqual(2, model.Uptime.DatesToReview.Count(), "Incorrect number of uptime dates");
			Assert.AreEqual(3, model.Uptime.DatesToReview.First().HoursDown, "Incorrect count of hours down for first uptime grouping");
			Assert.AreEqual("92.95", model.Uptime.UptimePercentage.ToString("N2"), "Incorrect quarterly uptime percentage");
			Assert.AreEqual("89.33", model.Uptime.WeeklyUptimePercentage.ToString("N2"), "Incorrect weekly uptime percentage");
		}
	}
}
