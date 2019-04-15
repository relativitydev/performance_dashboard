namespace kCura.PDB.Service.Tests.Services
{
	using System.Threading.Tasks;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.BISSummary.Models;
	using kCura.PDB.Service.BISSummary;
	using kCura.PDB.Tests.Common.Extensions;
	using Moq;
	using NUnit.Framework;

	[TestFixture]
	[Category("Unit")]
	public class RecoverabilityIntegritySummaryReporterTests
	{
		private Mock<IRecoverabilityIntegrityReportWriter> reportRepositoryMock;
		private Mock<IDatabaseRepository> databaseRepositoryMock;
		private RecoverabilityIntegritySummaryReporter recoverabilityIntegritySummaryReporter;
		
		[SetUp]
		public void Setup()
		{
			this.reportRepositoryMock = new Mock<IRecoverabilityIntegrityReportWriter>();
			this.databaseRepositoryMock = new Mock<IDatabaseRepository>();
			this.recoverabilityIntegritySummaryReporter = new RecoverabilityIntegritySummaryReporter(this.reportRepositoryMock.Object, this.databaseRepositoryMock.Object);
		}

		[Test]
		public async Task CreateRecoverabilityIntegritySummaryReport()
		{
			var hourId = 2;
			var overallScore = 13m;
			var rtoScore = 23m;
			var rpoScore = 22m;
			var backupFrequencyScore = 22m;
			var backupCoverageScore = 23m;
			var dbccFrequencyScore = 12m;
			var dbccCoverageScore = 11m;

			this.reportRepositoryMock.Setup(
					m => m.CreateRecoverabilityIntegrityReportData(
						It.Is<RecoverabilityIntegrityReportEntry>(
							r => r.HourId == hourId && r.OverallScore == overallScore && r.RtoScore == rtoScore
							     && r.BackupFrequencyScore == backupFrequencyScore && r.BackupCoverageScore == backupCoverageScore
							     && r.DbccCoverageScore == dbccCoverageScore && r.DbccFrequencyScore == dbccFrequencyScore)))
				.ReturnsAsyncDefault();

			await this.recoverabilityIntegritySummaryReporter.CreateRecoverabilityIntegritySummaryReport(
				hourId,
				overallScore,
				rpoScore,
				rtoScore,
				backupFrequencyScore,
				backupCoverageScore,
				dbccFrequencyScore,
				dbccCoverageScore);

			this.reportRepositoryMock.VerifyAll();
		}

		[Test]
		public async Task UpdateWorstRpo()
		{
			var hourId = 4;
			var databaseId = 2;
			var maxDataLoss = 34;

			var database = new Database { Name = "Test", Id = databaseId };
			this.databaseRepositoryMock.Setup(m => m.ReadAsync(databaseId)).ReturnsAsync(database);
			this.reportRepositoryMock
				.Setup(
					m => m.CreateRecoverabilityIntegrityRpoReport(
						It.Is<WorstRpoReportEntry>(
							e => e.HourId == hourId && e.WorstRpoDatabase == database.Name && e.RpoMaxDataLoss == maxDataLoss)))
				.ReturnsAsyncDefault();

			await this.recoverabilityIntegritySummaryReporter.UpdateWorstRpo(hourId, databaseId, maxDataLoss);

			this.databaseRepositoryMock.VerifyAll();
			this.reportRepositoryMock.VerifyAll();
		}

		[Test]
		public async Task UpdateWorstRto()
		{
			var hourId = 4;
			var databaseId = 2;
			var timeToRecover = 34;

			var database = new Database { Name = "Test", Id = databaseId };
			this.databaseRepositoryMock.Setup(m => m.ReadAsync(databaseId)).ReturnsAsync(database);
			this.reportRepositoryMock
				.Setup(
					m => m.CreateRecoverabilityIntegrityRtoReport(
						It.Is<WorstRtoReportEntry>(
							e => e.HourId == hourId && e.WorstRtoDatabase == database.Name && e.RtoTimeToRecover == timeToRecover)))
				.ReturnsAsyncDefault();

			await this.recoverabilityIntegritySummaryReporter.UpdateWorstRto(hourId, databaseId, timeToRecover);

			this.databaseRepositoryMock.VerifyAll();
			this.reportRepositoryMock.VerifyAll();
		}
	}
}
