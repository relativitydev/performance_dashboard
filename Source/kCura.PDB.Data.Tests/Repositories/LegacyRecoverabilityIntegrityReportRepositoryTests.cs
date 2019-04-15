namespace kCura.PDB.Data.Tests.Repositories
{
	using System;
	using kCura.PDB.Core.Models.BISSummary.Grids;
	using kCura.PDB.Core.Models.BISSummary.ViewColumns;
	using kCura.PDB.Data.Repositories.BISSummary;
	using NUnit.Framework;

	[TestFixture]
	[Category("UnitPlatform")]
	[Category("Integration")]
	public class LegacyRecoverabilityIntegrityReportRepositoryTests
	{
		private LegacyRecoverabilityIntegrityReportRepository reportRepository;

		[SetUp]
		public void Setup()
		{
			this.reportRepository = new LegacyRecoverabilityIntegrityReportRepository(ConnectionFactorySetup.ConnectionFactory);
		}


		[Theory]
		public void GetBackupDbccHistoryDetails(BackupDbccViewColumns sortColumn)
		{
			var gridConditions = new GridConditions { SortColumn = sortColumn.ToString() };
			var details = this.reportRepository.GetBackupDbccHistoryDetails(
				gridConditions,
				new BackupDbccViewFilterConditions(),
				new BackupDbccViewFilterOperands());

			Assert.That(details, Is.Not.Null);
		}

		[Theory]
		public void GetRecoverabilityIntegrityDetails_Default(RecoverabilityIntegrityViewColumns sortColumn)
		{
			var conditions = new GridConditions { SortColumn = sortColumn.ToString() };
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
			var conditions = new GridConditions { SortColumn = sortColumn.ToString() };
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
	}
}
