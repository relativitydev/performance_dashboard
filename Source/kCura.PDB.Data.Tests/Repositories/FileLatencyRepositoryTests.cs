namespace kCura.PDB.Data.Tests.Repositories
{
	using System;
	using System.Linq;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.BISSummary.Grids;
	using kCura.PDB.Core.Models.BISSummary.Models;
	using kCura.PDB.Data.Repositories.BISSummary;
	using kCura.PDB.Tests.Common;
	using NUnit.Framework;

	[TestFixture]
	[Category("Integration")]
	public class FileLatencyRepositoryTests
	{
		[SetUp]
		public void SetUp()
		{
			this.ecRepo = new FileLatencyRepository(TestUtilities.GetIntegrationConnectionFactory());
		}

		private FileLatencyRepository ecRepo;

		[Test]
		[Ignore("TODO Fix this")]
		public void ExecuteDatabaseDetails()
		{
			//Arrange
			var targetQoSServer = Config.Server;
			var eddsPerformanceServer = Config.Server;

			//Act
			ecRepo.ExecuteSaveFileLevelLatencyDetails(targetQoSServer, eddsPerformanceServer);

			//Assert

		}
	}

	[TestFixture]
	[Category("Integration")]
	[Category("UnitPlatform")]
	public class FileLatencyRepositoryUnitPlatformTests
	{
		[SetUp]
		public void SetUp()
		{
			this.ecRepo = new FileLatencyRepository(ConnectionFactorySetup.ConnectionFactory);
		}

		private FileLatencyRepository ecRepo;

		[Theory]
		public void GetFileLevelLatencyDetails_WithServerNameFilter(FileLatency.Columns sortColumn)
		{
			//Arrange
			var gridConditions = new GridConditions() { SortColumn = sortColumn.ToString(), SortDirection = "desc" };
			var filterConditions = System.Enum
				.GetValues(typeof(FileLatency.Columns))
				.Cast<FileLatency.Columns>()
				.ToDictionary(k => k, v => (String)null);
			var filterOperands = System.Enum
				.GetValues(typeof(FileLatency.Columns))
				.Cast<FileLatency.Columns>()
				.ToDictionary(k => k, v => FilterOperand.Equals);

			//filterConditions[FileLatency.Columns.DatabaseName] = "ED"; //begin fileter for EDDSXYZ
			filterConditions[FileLatency.Columns.ServerName] = "k12-"; //begin fileter for EDDSXYZ

			//Act
			var result = ecRepo.GetFileLevelLatencyDetails(gridConditions, filterConditions, filterOperands);

			//Assert
			Assert.That(result, Is.Not.Null);
		}

		[Theory]
		public void GetFileLevelLatencyDetails_WithServerIdFilter(FileLatency.Columns sortColumn)
		{
			//Arrange
			var gridConditions = new GridConditions() { SortColumn = sortColumn.ToString(), SortDirection = "desc" };
			var filterConditions = System.Enum
				.GetValues(typeof(FileLatency.Columns))
				.Cast<FileLatency.Columns>()
				.ToDictionary(k => k, v => (String)null);
			var filterOperands = System.Enum
				.GetValues(typeof(FileLatency.Columns))
				.Cast<FileLatency.Columns>()
				.ToDictionary(k => k, v => FilterOperand.Equals);

			//filterConditions[FileLatency.Columns.DatabaseName] = "ED"; //begin fileter for EDDSXYZ
			filterConditions[FileLatency.Columns.ServerName] = "1015096"; //begin fileter for EDDSXYZ

			//Act
			var result = ecRepo.GetFileLevelLatencyDetails(gridConditions, filterConditions, filterOperands);

			//Assert
			Assert.That(result, Is.Not.Null);
		}
	}
}
