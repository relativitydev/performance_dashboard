namespace kCura.PDB.Service.Tests.RecoverabilityIntegrity
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Interfaces.DataProviders;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.RecoverabilityIntegrity;
	using kCura.PDB.Service.RecoverabilityIntegrity;
	using kCura.PDB.Tests.Common;
	using kCura.PDB.Tests.Common.Extensions;
	using Moq;
	using NUnit.Framework;

	[TestFixture]
	[Category("Unit")]
	public class ServerDbccProcessorTests
	{
		private Mock<IDbccProvider> dbccProvider;
		private Mock<IDatabaseRepository> databaseRepository;
		private Mock<IDatabaseGapsRepository> databaseGapsRepository;
		private Mock<ILogger> logger;
		private ServerDbccProcessor serverDbccProcessor;

		[SetUp]
		public void Setup()
		{
			this.dbccProvider = new Mock<IDbccProvider>();
			this.databaseRepository = new Mock<IDatabaseRepository>();
			this.databaseGapsRepository = new Mock<IDatabaseGapsRepository>();
			this.logger = TestUtilities.GetMockLogger();
			this.serverDbccProcessor = new ServerDbccProcessor(
				this.dbccProvider.Object,
				this.databaseRepository.Object,
				this.databaseGapsRepository.Object,
				this.logger.Object);
		}

		[Test]
		public async Task ServerDbccProcessor_ProcessDbccsForServer()
		{
			// Arrange
			var hour = new Hour();
			var server = new Server();

			var db7 = new Database { Name = $"EDDS7", Id = 7, LastDbccDate = new DateTime(2018, 1, 1) };
			var db9 = new Database { Name = $"EDDS9", Id = 9, LastDbccDate = new DateTime(2018, 1, 1) };
			var databases = new[] { db7, db9 };

			var dbccs = new[]
			{
				new Dbcc {DatabaseId = db7.Id, End = new DateTime(2018, 2, 1)},
				new Dbcc {DatabaseId = db9.Id, End = new DateTime(2018, 2, 1)}
			};

			this.databaseRepository.Setup(r => r.ReadByServerIdAsync(server.ServerId))
				.ReturnsAsync(databases);

			this.dbccProvider.Setup(p => p.GetDbccsAsync(server, databases))
				.ReturnsAsync(dbccs);
			this.databaseGapsRepository.Setup(r => r.CreateDatabaseGapsAsync(It.IsAny<IList<DbccGap>>()))
				.ReturnsAsyncDefault();

			// Act
			await this.serverDbccProcessor.ProcessDbccsForServer(hour, server);

			// Assert
			this.databaseGapsRepository.Verify(r => r.CreateDatabaseGapsAsync(It.Is<IList<DbccGap>>(list => list.Count() == 2)));
		}

		[Test]
		public async Task ServerDbccProcessor_ProcessDbccForDatabaseBatches()
		{
			// Arrange
			var server = new Server();

			var db7 = new Database { Name = $"EDDS7", Id = 7, LastDbccDate = new DateTime(2018, 1, 1) };
			var db9 = new Database { Name = $"EDDS9", Id = 9, LastDbccDate = new DateTime(2018, 1, 1) };
			var databases = new[] { db7, db9 };

			var dbccs = new[]
			{
				new Dbcc {DatabaseId = db7.Id, End = new DateTime(2018, 2, 1)},
				new Dbcc {DatabaseId = db9.Id, End = new DateTime(2018, 2, 1)}
			};

			this.dbccProvider.Setup(p => p.GetDbccsAsync(server, databases))
				.ReturnsAsync(dbccs);

			// Act
			var results = await this.serverDbccProcessor.ProcessDbccForDatabaseBatches(server, databases);

			// Assert
			Assert.That(results.Count, Is.EqualTo(2));
			Assert.That(results.Any(gap => gap.DatabaseId == db7.Id), Is.True);
			Assert.That(results.Any(gap => gap.DatabaseId == db9.Id), Is.True);
		}

		[Test]
		[TestCase(-81761, null, false, false, TestName = "CreateDbccGap with current dbcc before default 1900")]
		[TestCase(0, null, false, true, TestName = "CreateDbccGap with current dbcc with no last dbcc")]
		[TestCase(0, 0, false, false, TestName = "CreateDbccGap with current dbcc and last dbcc have same date")]
		[TestCase(0, -5, true, true, TestName = "CreateDbccGap with current dbcc with -5 day old last dbcc")]
		public async Task ServerDbccProcessor_CreateDbccGap(int dbccEnd, int? lastDbccEnd, bool expectedResultNotNull, bool expectedResultUpdateLast)
		{
			// Arrange
			var dbcc = new Dbcc { DatabaseId = 123, End = DateTime.UtcNow.AddDays(dbccEnd) };
			var lastDbcc = lastDbccEnd.HasValue ? DateTime.UtcNow.AddDays(lastDbccEnd.Value) : (DateTime?)null;
			var database = new Database { Id = 2, LastDbccDate = lastDbcc };

			this.databaseRepository.Setup(r => r.UpdateAsync(database))
				.ReturnsAsyncDefault();

			// Act
			var result = await this.serverDbccProcessor.CreateDbccGap(database, dbcc);

			// Assert
			Assert.That(result, expectedResultNotNull ? Is.Not.Null : Is.Null);
			this.databaseRepository.Verify(
				r => r.UpdateAsync(database),
				expectedResultUpdateLast ? Times.Once() : Times.Never());
		}
	}
}
