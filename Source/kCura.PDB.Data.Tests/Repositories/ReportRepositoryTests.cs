namespace kCura.PDB.Data.Tests.Repositories
{
	using System;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.BISSummary.Grids;
	using kCura.PDB.Core.Models.BISSummary.ViewColumns;
	using kCura.PDB.Data.Repositories.BISSummary;
	using kCura.PDB.Tests.Common;
	using kCura.PDD.Web.Enum;
	using NUnit.Framework;

	[TestFixture]
	[Category("Integration")]
	public class ReportRepositoryTests
	{
		[SetUp]
		public void SetUp()
		{
			this.reportRepository = new ReportRepository(TestUtilities.GetIntegrationConnectionFactory());
		}

		private ReportRepository reportRepository;

		[Test]
		public void LookingGlassHasRun()
		{
			// Act 
			var result = this.reportRepository.LookingGlassHasRun();

			// Assert 
			Assert.Pass($"Result can be any value: {result}");
		}

		[Test]
		public void ReadQuality()
		{
			// Act
			var result = this.reportRepository.ReadQuality(1);

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result, Is.Not.Empty);
		}
	}

	[TestFixture]
	[Category("UnitPlatform")]
	[Category("Integration")]
	public class ReportRepositoryUnitPlatformTests
	{
		[SetUp]
		public void SetUp()
		{
			this.reportRepository = new ReportRepository(ConnectionFactorySetup.ConnectionFactory);
		}

		private ReportRepository reportRepository;

		[Theory]
		public void GetSystemLoadWaitsDetails(WaitsViewColumns sortColumn)
		{
			//Arrange
			var gridConditions = new GridConditions();
			var filterConditions = new WaitsViewFilterConditions();
			var filterOperands = new WaitsViewFilterOperands();
			//Grid conditions
			gridConditions.SortColumn = sortColumn.ToString();
			gridConditions.SortDirection = "ASC";
			gridConditions.TimezoneOffset = 0;
			gridConditions.StartRow = 1;
			gridConditions.EndRow = 25;

			//Filter conditions
			filterConditions.Server = "";
			filterConditions.WaitType = "";

			//Filter operands
			filterOperands.OverallScore = FilterOperand.Equals;
			filterOperands.SignalWaitsRatio = FilterOperand.Equals;
			filterOperands.SignalWaitTime = FilterOperand.Equals;
			filterOperands.TotalWaitTime = FilterOperand.Equals;
			filterOperands.PercentOfCPUThreshold = FilterOperand.Equals;
			filterOperands.DifferentialWaitingTasksCount = FilterOperand.Equals;

			//Page-level filters
			gridConditions.StartDate = DateTime.Now.AddYears(-1);
			gridConditions.EndDate = DateTime.Now.AddDays(1);

			//Act
			var result = this.reportRepository.GetSystemLoadWaitsDetails(gridConditions, filterConditions, filterOperands);

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result, Is.Not.Empty);
		}

		[Theory]
		public void GetUserExperienceServerDetails(ServerViewColumns sortColumn)
		{
			//Arrange
			var gridConditions = new GridConditions();
			var filterConditions = new ServerViewFilterConditions();
			var filterOperands = new ServerViewFilterOperands();
			//Grid conditions
			gridConditions.SortColumn = sortColumn.ToString();
			gridConditions.SortDirection = "ASC";
			gridConditions.TimezoneOffset = 0;
			gridConditions.StartRow = 1;
			gridConditions.EndRow = 25;

			//Filter conditions
			filterConditions.Server = "";
			filterConditions.Workspace = "";

			//Filter operands
			filterOperands.Score = FilterOperand.Equals;
			filterOperands.TotalUsers = FilterOperand.Equals;
			filterOperands.TotalSearchAudits = FilterOperand.Equals;
			filterOperands.TotalNonSearchAudits = FilterOperand.Equals;
			filterOperands.TotalLongRunning = FilterOperand.Equals;
			filterOperands.TotalExecutionTime = FilterOperand.Equals;
			filterOperands.TotalAudits = FilterOperand.Equals;

			//Page-level filters
			gridConditions.StartDate = DateTime.Now.AddYears(-1);
			gridConditions.EndDate = DateTime.Now.AddDays(1);

			//Act
			var result = this.reportRepository.GetUserExperienceServerDetails(gridConditions, filterConditions, filterOperands);

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result, Is.Not.Empty);
		}

		[Theory]
		public void GetUserExperienceSearchDetails(SearchViewColumns sortColumn)
		{
			//Arrange
			var gridConditions = new GridConditions();
			var filterConditions = new SearchViewFilterConditions();
			var filterOperands = new SearchViewFilterOperands();
			//Grid conditions
			gridConditions.SortColumn = sortColumn.ToString();
			gridConditions.SortDirection = "ASC";
			gridConditions.TimezoneOffset = 0;
			gridConditions.StartRow = 1;
			gridConditions.EndRow = 25;

			//Filter conditions
			filterConditions.Search = "";
			filterConditions.User = "";

			//Filter operands
			filterOperands.PercentLongRunning = FilterOperand.Equals;
			filterOperands.TotalRunTime = FilterOperand.Equals;
			filterOperands.AverageRunTime = FilterOperand.Equals;
			filterOperands.TotalRuns = FilterOperand.Equals;
			filterOperands.QoSHourID = FilterOperand.Equals;

			//Page-level filters
			gridConditions.StartDate = DateTime.Now.AddYears(-1);
			gridConditions.EndDate = DateTime.Now.AddDays(1);

			//Act
			var result = this.reportRepository.GetUserExperienceSearchDetails(gridConditions, filterConditions, filterOperands);

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result, Is.Not.Empty);
		}

		[Theory]
		public void GetUserExperienceHourDetails(HoursViewColumns sortColumn)
		{
			//Arrange
			var gridConditions = new GridConditions();
			var filterConditions = new HoursViewFilterConditions();
			var filterOperands = new HoursViewFilterOperands();
			//Grid conditions
			gridConditions.SortColumn = sortColumn.ToString();
			gridConditions.SortDirection = "ASC";
			gridConditions.TimezoneOffset = 0;
			gridConditions.StartRow = 1;
			gridConditions.EndRow = 25;

			//Filter conditions
			filterConditions.Search = "";
			filterConditions.Workspace = "";
			
			//Filter operands
			filterOperands.TotalRunTime = FilterOperand.Equals;
			filterOperands.AverageRunTime = FilterOperand.Equals;
			filterOperands.TotalRuns = FilterOperand.Equals;

			//Page-level filters
			gridConditions.StartDate = DateTime.Now.AddYears(-1);
			gridConditions.EndDate = DateTime.Now.AddDays(1);

			//Act
			var result = this.reportRepository.GetUserExperienceHourDetails(gridConditions, filterConditions, filterOperands);

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result, Is.Not.Empty);
		}

		[Theory]
		public void GetSystemLoadServerDetails(LoadViewColumns sortColumn)
		{
			//Arrange
			var gridConditions = new GridConditions();
			var filterConditions = new LoadViewFilterConditions();
			var filterOperands = new LoadViewFilterOperands();

			//Grid conditions
			gridConditions.SortColumn = sortColumn.ToString();
			gridConditions.SortDirection = "ASC";
			gridConditions.TimezoneOffset = 0;
			gridConditions.StartRow = 1;
			gridConditions.EndRow = 25;

			//Filter conditions
			filterConditions.Server = "";
			filterConditions.ServerType = "";

			//Filter operands
			filterOperands.OverallScore = FilterOperand.Equals;
			filterOperands.CPUUtilizationScore = FilterOperand.Equals;
			filterOperands.RAMUtilizationScore = FilterOperand.Equals;
			filterOperands.MemorySignalScore = FilterOperand.Equals;
			filterOperands.WaitsScore = FilterOperand.Equals;
			filterOperands.VirtualLogFilesScore = FilterOperand.Equals;
			filterOperands.LatencyScore = FilterOperand.Equals;

			//Page-level filters
			gridConditions.StartDate = DateTime.Now.AddYears(-1);
			gridConditions.EndDate = DateTime.Now.AddDays(1);

			//Act
			var result = this.reportRepository.GetSystemLoadServerDetails(gridConditions, filterConditions, filterOperands);

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result, Is.Not.Empty);
		}

		[Theory]
		public void GetRecoverabilityIntegrityDetails(RecoverabilityIntegrityViewColumns sortColumn)
		{
			//Arrange
			var gridConditions = new GridConditions();
			var filterConditions = new RecoverabilityIntegrityViewFilterConditions();
			var filterOperands = new RecoverabilityIntegrityViewFilterOperands();

			//Grid conditions
			gridConditions.SortColumn = sortColumn.ToString();
			gridConditions.SortDirection = "ASC";
			gridConditions.TimezoneOffset = 0;
			gridConditions.StartRow = 1;
			gridConditions.EndRow = 25;

			//Filter conditions

			//Filter operands
			filterOperands.RecoverabilityIntegrityScore = FilterOperand.Equals;
			filterOperands.BackupFrequencyScore = FilterOperand.Equals;
			filterOperands.BackupCoverageScore = FilterOperand.Equals;
			filterOperands.DbccFrequencyScore = FilterOperand.Equals;
			filterOperands.DbccCoverageScore = FilterOperand.Equals;
			filterOperands.RPOScore = FilterOperand.Equals;
			filterOperands.RTOScore = FilterOperand.Equals;

			//Page-level filters
			gridConditions.StartDate = DateTime.Now.AddYears(-1);
			gridConditions.EndDate = DateTime.Now.AddDays(1);

			//Act
			var result = this.reportRepository.GetRecoverabilityIntegrityDetails(gridConditions, filterConditions, filterOperands);

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result, Is.Not.Empty);
		}

		[Theory]
		public void GetUptimeHours(UptimeViewColumns sortColumn)
		{
			// Arrange
			var gridConditions = new GridConditions();
			var filterConditions = new UptimeViewFilterConditions();
			var filterOperands = new UptimeViewFilterOperands();

			//Grid conditions
			gridConditions.SortColumn = sortColumn.ToString();
			gridConditions.SortDirection = "ASC";
			gridConditions.TimezoneOffset = 0;
			gridConditions.StartRow = 1;
			gridConditions.EndRow = 25;

			// Act
			var result = this.reportRepository.GetUptimeHours(gridConditions, filterConditions, filterOperands);

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result, Is.Not.Empty);
		}

		[Test]
		[TestCase("1234")]
		[TestCase("1234, 12, 1")]
		[TestCase("")]
		public void ReadScoreHistory(string servers)
		{
			var result = this.reportRepository.ReadScoreHistory(DateTime.UtcNow.AddYears(-2), DateTime.UtcNow, servers);

			// Assert
			Assert.That(result, Is.Not.Null);
		}
	}
}