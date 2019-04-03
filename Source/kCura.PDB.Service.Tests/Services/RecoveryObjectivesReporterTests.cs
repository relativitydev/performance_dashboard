namespace kCura.PDB.Service.Tests.Services
{
	using System.Linq;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Models.BISSummary.Models;
	using kCura.PDB.Service.BISSummary;
	using kCura.PDB.Tests.Common.Extensions;
	using Moq;
	using NUnit.Framework;

	[TestFixture]
	[Category("Unit")]
	public class RecoveryObjectivesReporterTests
	{
		private Mock<IRecoverabilityIntegrityReportWriter> reportRepositoryMock;
		private RecoveryObjectivesReporter recoveryObjectivesReporter;

		[SetUp]
		public void Setup()
		{
			this.reportRepositoryMock = new Mock<IRecoverabilityIntegrityReportWriter>();
			this.recoveryObjectivesReporter = new RecoveryObjectivesReporter(this.reportRepositoryMock.Object);
		}

		[Test]
		public async Task UpdateRecoveryObjectivesRpoReport()
		{
			var databaseId = 1;
			var rpoScore = 12m;
			var rpoMaxDataLoss = 14;
			var rpo = new DatabaseRpoScoreData
				          {
					          DatabaseId = databaseId,
					          RpoScore = rpoScore,
					          PotentialDataLoss = rpoMaxDataLoss
				          };
			var rpoList = new[] { rpo }.ToList();

			this.reportRepositoryMock.Setup(m => m.UpdateRecoveryObjectivesRpoReport(rpo)).ReturnsAsyncDefault();

			await this.recoveryObjectivesReporter.UpdateRpoReport(rpoList);

			this.reportRepositoryMock.VerifyAll();
		}

		[Test]
		public async Task UpdateRecoveryObjectivesRtoReport()
		{
			var databaseId = 1;
			var rtoScore = 14m;
			var rtoTimeToRecover = 13m;
			var rto = new DatabaseRtoScoreData
				          {
					          DatabaseId = databaseId,
					          RtoScore = rtoScore,
					          TimeToRecoverHours = rtoTimeToRecover
				          };
			var rtoList = new[] { rto }.ToList();

			this.reportRepositoryMock.Setup(m => m.UpdateRecoveryObjectivesRtoReport(rto)).ReturnsAsyncDefault();

			// act
			await this.recoveryObjectivesReporter.UpdateRtoReport(rtoList);

			// assert
			this.reportRepositoryMock.VerifyAll();
		}
	}
}
