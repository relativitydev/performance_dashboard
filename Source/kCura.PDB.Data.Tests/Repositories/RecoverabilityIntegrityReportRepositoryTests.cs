namespace kCura.PDB.Data.Tests.Repositories
{
	using System;
	using System.Threading.Tasks;

	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.BISSummary.Grids;
	using kCura.PDB.Core.Models.BISSummary.Models;
	using kCura.PDB.Core.Models.BISSummary.ViewColumns;
	using kCura.PDB.Core.Models.RecoverabilityIntegrity;
	using kCura.PDB.Data.Repositories;
	using kCura.PDB.Data.Repositories.BISSummary;

	using NUnit.Framework;

	[TestFixture]
	[Category("UnitPlatform")]
	[Category("Integration")]
	public class RecoverabilityIntegrityReportRepositoryTests
	{
		private RecoverabilityIntegrityReportRepository reportRepository;

		[SetUp]
		public void Setup()
		{
			var connectionFactory = ConnectionFactorySetup.ConnectionFactory;
			this.reportRepository = new RecoverabilityIntegrityReportRepository(connectionFactory);
		}

		[Theory]
		public void GetBackupDbccHistoryDetails(BackupDbccViewColumns sortColumn)
		{
			var gridConditions = new GridConditions{SortColumn = sortColumn.ToString()};
			var details = this.reportRepository.GetBackupDbccHistoryDetails(
				gridConditions,
				new BackupDbccViewFilterConditions(),
				new BackupDbccViewFilterOperands());

			Assert.That(details, Is.Not.Null);
		}

		[Theory]
		public void GetRecoverabilityIntegrityDetails_Default(RecoverabilityIntegrityViewColumns sortColumn)
		{
			var conditions = new GridConditions{SortColumn = sortColumn.ToString()};
			var otherConditions = new RecoverabilityIntegrityViewFilterConditions();

			var details = this.reportRepository.GetRecoverabilityIntegrityDetails(
				conditions,
				otherConditions,
				new RecoverabilityIntegrityViewFilterOperands());

			Assert.That(details, Is.Not.Null);
		}

		[Theory]
		public void GetRecoverabilityIntegrityDetails_DateFilter(RecoverabilityIntegrityViewColumns sortColumn)
		{
			var conditions = new GridConditions{SortColumn = sortColumn.ToString()};
			var otherConditions = new RecoverabilityIntegrityViewFilterConditions() { SummaryDayHour = DateTime.UtcNow };

			var details = this.reportRepository.GetRecoverabilityIntegrityDetails(
				conditions,
				otherConditions,
				new RecoverabilityIntegrityViewFilterOperands());

			Assert.That(details, Is.Not.Null);
		}

		[Theory]
		public void GetRecoveryObjectivesDetails(RecoveryObjectivesViewColumns sortColumn)
		{
			var conditions = new GridConditions { SortColumn = sortColumn.ToString() };
			var details = this.reportRepository.GetRecoveryObjectivesDetails(
				conditions,
				new RecoveryObjectivesViewFilterConditions(),
				new RecoveryObjectivesViewFilterOperands());

			Assert.That(details, Is.Not.Null);
		}

		[Test]
		public async Task CreateRecoveryObjectivesReportData_WithUpdate()
		{
			var rpo1 = new DatabaseRpoScoreData { DatabaseId = 1, RpoScore = 33m, PotentialDataLoss = 22 };
			var rto1 = new DatabaseRtoScoreData { DatabaseId = 1, RtoScore = 34m, TimeToRecoverHours = 32m };

			await this.reportRepository.UpdateRecoveryObjectivesRpoReport(rpo1);
			await this.reportRepository.UpdateRecoveryObjectivesRtoReport(rto1);

			var rpo2 = new DatabaseRpoScoreData { DatabaseId = 1, RpoScore = 12m, PotentialDataLoss = 55 };
			var rto2 = new DatabaseRtoScoreData { DatabaseId = 1, RtoScore = 13m, TimeToRecoverHours = 60m };

			await this.reportRepository.UpdateRecoveryObjectivesRpoReport(rpo2);
			await this.reportRepository.UpdateRecoveryObjectivesRtoReport(rto2);

			Assert.Pass("No results returned");
		}

		[Test]
		public async Task CreateRecoverabilityIntegrityReportData()
		{
			var entry = new RecoverabilityIntegrityReportEntry
			{
				HourId = 2,
				OverallScore = 2,
				RpoScore = 3,
				RtoScore = 4,
				BackupCoverageScore = 3,
				BackupFrequencyScore = 4,
				DbccCoverageScore = 3,
				DbccFrequencyScore = 4
			};
			await this.reportRepository.CreateRecoverabilityIntegrityReportData(entry);

			Assert.Pass("No results returned");
		}

		[Test]
		public async Task CreateGapReportData()
		{
			var entry = new GapReportEntry
			{
				DatabaseId = 123,
				ActivityType = (int)GapActivityType.Backup,
				LastActivity = new DateTime(1901, 1, 2, 3, 4, 5),
				GapResolutionDate = new DateTime(1901, 1, 2, 3, 4, 6),
				GapSize = 1
			};

			await this.reportRepository.CreateGapReportData(entry);

			Assert.Pass("No results returned");
		}

		[Test]
		public async Task ClearUnresolvedGapReportData()
		{
			// Arrange
			// Create test server
			var serverRepository = new ServerRepository(ConnectionFactorySetup.ConnectionFactory);
			var testServer = new Server
				                 {
					                 ServerName = "TestServer",
					                 CreatedOn = DateTime.UtcNow,
					                 ServerType = ServerType.Database
				                 };
			var server = await serverRepository.CreateAsync(testServer);
			
			// Create test database
			var databaseRepository = new DatabaseRepository(ConnectionFactorySetup.ConnectionFactory);
			var testDatabase = new Database
				                   {
					                   Type = DatabaseType.Workspace,
					                   Name = "TestDatabase",
					                   WorkspaceId = 1,
					                   ServerId = server.ServerId
				                   };
			var database = await databaseRepository.CreateAsync(testDatabase);

			// Create test gap report (unresolved)
			var entry = new GapReportEntry
			{
				DatabaseId = database.Id,
				ActivityType = (int)GapActivityType.Backup,
				LastActivity = new DateTime(1901, 1, 2, 3, 4, 5),
				GapSize = 10002000
			};

			await this.reportRepository.CreateGapReportData(entry);
			
			// Act - Clear test gap report
			await this.reportRepository.ClearUnresolvedGapReportData(server.ServerId, GapActivityType.Backup);

			// Assert
			Assert.Pass("No results returned");

			// Tear down
			await serverRepository.DeleteAsync(server);
		}
	}
}
