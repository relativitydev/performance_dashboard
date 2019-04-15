using kCura.PDD.Web.Controllers;
using kCura.PDD.Web.Mapper;
using kCura.PDD.Web.Models.BISSummary;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace kCura.PDD.Web.Test.Controllers
{
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Service.BISSummary;

	[TestFixture]
	[Category("Integration")]
	public class ServiceQualiityAPIControllerTests
	{
		[SetUp]
		public void Setup()
		{
			this.pdbNotificationService = new Mock<IPDBNotificationService>();
		}

		private Mock<IPDBNotificationService> pdbNotificationService;

		[Test]
		public void GetServiceIndicators_Test()
		{
			// Arrange
			var model = new QualityOfServiceViewModel
			{
				OverallScore = 83,
				WeeklyScore = 52,
				UserExperience = new ServerDetailCategory
				{
					QuarterlyScore = 80,
					WeeklyScore = 80,
					Servers = new List<ServerScore>
												{
													new ServerScore {
														ArtifactId = 1,
														Name = "Server 1",
														QuarterlyScore = 64,
														WeeklyScore = 64
													}
												}
				},
				SystemLoad = new ServerDetailCategory
				{
					QuarterlyScore = 64,
					WeeklyScore = 64,
					Servers = new List<ServerScore>
												{
													new ServerScore {
														ArtifactId = 1,
														Name = "Server 1",
														QuarterlyScore = 64,
														WeeklyScore = 64
													}
												}
				},
				Backup = new MaintenanceCategory
				{
					Score = 64,
					BackupCoverageScore = 68,
					BackupFrequencyScore = 72,
					DbccCoverageScore = 60,
					DbccFrequencyScore = 72,
					MissedBackups = 1,
					MissedIntegrityChecks = 1
				},
				Uptime = new AvailabilityCategory
				{
					Score = 79,
					UptimePercentage = 85.0,
					DatesToReview = new List<DateToReview>
								{
									new DateToReview
										{
											Date = DateTime.Now.ToShortDateString(),
											HoursDown = 50
										}
								}
				}
			};

			var bis = new Mock<BestInServiceReportingService>();
			var mapper = new Mock<BestInServiceMapper>();
			mapper.Setup(service => BestInServiceMapper.ToQualityOfServiceReportModel(bis.Object, 0)).Returns(model);


			// Act
			var serviceController = new ServiceQualityController();
			var indicators = serviceController.GetServiceQualityIndicators();


			// Assert
			Assert.AreEqual(80, indicators.UserExperience.QuarterlyScore);
		}

		[Test]
		public void GetNotificationAlert_Tests()
		{
			// Arrange
			pdbNotificationService.Setup(pdb => pdb.GetNext()).Returns(new PDBNotification()
			{
				Message = "Critical",
				Type = NotificationType.Critical
			});


			// Act
			var service = new ServiceQualityController();
			var notifications = service.GetNotifications();


			// Assert
			Assert.AreEqual(NotificationType.Critical, notifications.Type);
		}
	}
}
