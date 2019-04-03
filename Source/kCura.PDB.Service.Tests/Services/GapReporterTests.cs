namespace kCura.PDB.Service.Tests.Services
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Servers;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.BISSummary.Models;
	using kCura.PDB.Core.Models.RecoverabilityIntegrity;
	using kCura.PDB.Service.BISSummary;
	using kCura.PDB.Tests.Common.Extensions;
	using Moq;
	using NUnit.Framework;

	[TestFixture]
	[Category("Unit")]
	public class GapReporterTests
	{
		private Mock<IRecoverabilityIntegrityReportWriter> reportRepositoryMock;
		private Mock<IDatabaseService> databaseServiceMock;
		private GapReporter gapReporter;


		[SetUp]
		public void Setup()
		{
			this.reportRepositoryMock = new Mock<IRecoverabilityIntegrityReportWriter>();
			this.databaseServiceMock = new Mock<IDatabaseService>();
			this.gapReporter = new GapReporter(this.reportRepositoryMock.Object, this.databaseServiceMock.Object);
		}

		[Test]
		public async Task CreateGapReport()
		{
			var serverId = 3;
			var server = new Server { ServerId = serverId };
			var databaseId = 2;
			var gapActivityType = GapActivityType.Backup;
			var gapStart = new DateTime(1901, 1, 2, 3, 4, 5);
			var gapEnd = new DateTime(1901, 1, 2, 3, 4, 6);
			var gaps = new List<Gap>
						   {
							   new Gap
								   {
									   DatabaseId = databaseId,
									   Start = gapStart,
									   End = gapEnd,
									   ActivityType = gapActivityType
								   }
						   };
			var now = DateTime.UtcNow.NormilizeToHour();
			var nowHour = new Hour { HourTimeStamp = now };
			var nextHourTime = nowHour.GetNextHour();
			var unresolvedGapStart = now.AddHours(-2);
			var unresolvedGaps = new List<Gap>
			{
				new Gap
				{
					DatabaseId = databaseId,
					ActivityType = gapActivityType,
					Start = unresolvedGapStart
				}
			};
			this.databaseServiceMock.Setup(m => m.ReadUnresolvedGapsAsync(nowHour, server, gapActivityType)).ReturnsAsync(unresolvedGaps);

			this.reportRepositoryMock.Setup(m => m.CreateGapReportData(It.Is<GapReportEntry>(g =>
				g.DatabaseId == databaseId && g.ActivityType == (int)gapActivityType && g.LastActivity == gapStart &&
				g.GapResolutionDate == gapEnd && g.GapSize == (int)(gapEnd - gapStart).TotalSeconds))).ReturnsAsyncDefault();
			this.reportRepositoryMock.Setup(m => m.CreateGapReportData(It.Is<GapReportEntry>(g =>
					g.DatabaseId == databaseId && g.ActivityType == (int)gapActivityType && g.LastActivity == unresolvedGapStart &&
					g.GapResolutionDate == null && g.GapSize == (int)(nextHourTime - g.LastActivity).TotalSeconds)))
				.ReturnsAsyncDefault();
			this.reportRepositoryMock.Setup(m => m.ClearUnresolvedGapReportData(server.ServerId, gapActivityType)).ReturnsAsyncDefault();

			// Act
			await this.gapReporter.CreateGapReport(nowHour, server, gaps, gapActivityType);

			// Assert
			this.reportRepositoryMock.VerifyAll();
			this.databaseServiceMock.VerifyAll();
		}
	}
}
