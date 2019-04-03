namespace kCura.PDB.Data.Tests.Repositories
{
	using System;
	using System.Data.SqlTypes;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Data.Repositories;
	using kCura.PDB.Tests.Common;
	using NUnit.Framework;

	[TestFixture]
	[Category("Integration")]
	//[Explicit("This class runs delete operations.  Be careful with the test cases you run and be sure about the input you provide.")]
	public class CleanupTablesRepositoryTests
	{
		private CleanupTablesRepository cleanupTablesRepository;

		[SetUp]
		public void SetUp()
		{
			this.cleanupTablesRepository = new CleanupTablesRepository(TestUtilities.GetIntegrationConnectionFactory());
		}

		[Test]
		[TestCase(Names.Database.EddsdboSchema + "." + Names.Database.SourceDatetimeTable, Names.Database.QuotidianColumn, null, 500, true)]
		[TestCase(Names.Database.EddsdboSchema + "." + Names.Database.VarscatOutputCumulativeTable, Names.Database.SummaryDayHourColumn, null, 500, true)]
		[TestCase(Names.Database.EddsdboSchema + "." + Names.Database.VarscatOutputDetailCumulativeTable, Names.Database.SummaryDayHourColumn, null, 500, true)]
		[TestCase(Names.Database.EddsdboSchema + "." + Names.Database.SourceDatetimeTable, Names.Database.QuotidianColumn, "2017-02-22 18:00:00.000", 500, true, Explicit = true)]
		[TestCase(Names.Database.EddsdboSchema + "." + Names.Database.VarscatOutputCumulativeTable, Names.Database.SummaryDayHourColumn, "2017-02-22 18:00:00.000", 500, true, Explicit = true)]
		[TestCase(Names.Database.EddsdboSchema + "." + Names.Database.VarscatOutputDetailCumulativeTable, Names.Database.SummaryDayHourColumn, "2017-02-22 18:00:00.000", 500, true, Explicit = true)]
		public async Task CleanupPerformanceTable(string tableScope, string dateTimeColumn, DateTime? threshold, int batchSize, bool maxdopLimit)
		{
			// Arrange
			var thresholdToUse = threshold ?? SqlDateTime.MinValue.Value;

			// Act
			await
				this.cleanupTablesRepository.CleanupPerformanceTable(tableScope, dateTimeColumn, thresholdToUse, batchSize, maxdopLimit);

			// Assert
		}

		[Test]
		[TestCase(null, 500, true)]
		[TestCase("2017-02-22 18:00:00.000", 500, true)]
		public async Task CleanupQosGlassRunLogTable(DateTime? threshold, int batchSize, bool maxdopLimit)
		{
			// Arrange
			var thresholdToUse = threshold ?? SqlDateTime.MinValue.Value;

			// Act
			await
				this.cleanupTablesRepository.CleanupQoSGlassRunLog(thresholdToUse, batchSize, maxdopLimit);

			// Assert
		}

		[Test]
		[TestCase(Names.Database.EddsdboSchema + "." + Names.Database.VarscatOutputTable, Names.Database.SummaryDayHourColumn, null, 500, true)]
		[TestCase(Names.Database.EddsdboSchema + "." + Names.Database.VarscatOutputDetailTable, Names.Database.TimestampColumn, null, 500, true)]
		[TestCase(Names.Database.EddsdboSchema + "." + Names.Database.VarscatOutputTable, Names.Database.SummaryDayHourColumn, "2017-08-02 21:00:00.000", 500, true, Explicit = true)]
		[TestCase(Names.Database.EddsdboSchema + "." + Names.Database.VarscatOutputDetailTable, Names.Database.TimestampColumn, "2017-08-02 21:00:00.000", 500, true, Explicit = true)]
		public async Task CleanupQosTable(string tableScope, string dateTimeColumn, DateTime? threshold, int batchSize, bool maxdopLimit)
		{
			// Arrange
			var serverName = Config.Server;
			var thresholdToUse = threshold ?? SqlDateTime.MinValue.Value;

			// Act
			await
				this.cleanupTablesRepository.CleanupQosTable(serverName, tableScope, dateTimeColumn, thresholdToUse, batchSize, maxdopLimit);
		}

		[Test]
		[Explicit]
		[TestCase(Names.Database.EddsdboSchema + "." + Names.Database.VarscatOutputTable, 1015096201721421, 500, true)] 
		[TestCase(Names.Database.EddsdboSchema + "." + Names.Database.VarscatOutputDetailTable, 1015096201721421, 500, true)]
		// Removes rows with columns less than given qosHourID (does this work as intended??  how are we not obiterating other server data?
		public async Task CleanupQosTable(string tableScope, long qosHourIdThreshold, int batchSize, bool maxdopLimit)
		{
			// Arrange
			var serverName = Config.Server;

			// Act
			await
				this.cleanupTablesRepository.CleanupQosTable(serverName, tableScope, qosHourIdThreshold, batchSize, maxdopLimit);

			// Assert
		}



		[Test]
		[TestCase(null, 500, true)]
		[TestCase("2017-02-07 05:00:00.000", 500, true, Explicit = true)]
		public async Task CleanupDecommissionedServers(DateTime? threshold, int batchSize, bool maxdopLimit)
		{
			// Arrange
			var thresholdToUse = threshold ?? SqlDateTime.MinValue.Value;

			// Act
			await this.cleanupTablesRepository.CleanupDecommissionedServers(thresholdToUse, batchSize, maxdopLimit);

			// Assert
		}
	}
}
