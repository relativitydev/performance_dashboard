namespace kCura.PDB.Data.Tests.Repositories
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.Audits;
	using kCura.PDB.Data.Repositories;
	using kCura.PDB.Tests.Common;
	using NUnit.Framework;

	[TestFixture]
	[Category("Integration")]
	public class SearchAuditBatchRepositoryTests
	{
		private SearchAuditBatchRepository searchAuditBatchRepository;

		[OneTimeSetUp]
		public async Task SetUp()
		{
			var connFactory = TestUtilities.GetIntegrationConnectionFactory();
			this.searchAuditBatchRepository = new SearchAuditBatchRepository(connFactory);
			var hourRepo = new HourRepository(connFactory);
			hour = await hourRepo.CreateAsync(new Hour { HourTimeStamp = DateTime.UtcNow.NormilizeToHour() });
			Assert.That(hour, Is.Not.Null, "Must have a valid hour for tests");
			var serverRepo = new ServerRepository(connFactory);
			server = (await serverRepo.ReadAllActiveAsync()).FirstOrDefault();
			Assert.That(server, Is.Not.Null, "Must have a valid server for tests");

			this.HourAuditSearchBatchId =
				await this.searchAuditBatchRepository.CreateHourSearchAuditBatch(hour.Id, server.ServerId, 1);

			await this.searchAuditBatchRepository.CreateBatches(new[] {
			new SearchAuditBatch
			{
				HourSearchAuditBatchId = this.HourAuditSearchBatchId,
				WorkspaceId = Config.WorkSpaceId,
				BatchStart = 0,
				BatchSize = 5000
			}});

			this.searchAuditBatch = (await this.searchAuditBatchRepository.ReadBatchesByHourAndServer(hour.Id, server.ServerId)).FirstOrDefault();
		}

		private int HourAuditSearchBatchId;
		private SearchAuditBatch searchAuditBatch;
		private Hour hour;
		private Server server;

		[Test]
		public void CreateBatch()
		{
			// Assert
			Assert.That(searchAuditBatch, Is.Not.Null);
			Assert.That(searchAuditBatch.Id, Is.GreaterThan(0));
			Assert.That(searchAuditBatch.HourId, Is.EqualTo(this.hour.Id));
			Assert.That(searchAuditBatch.ServerId, Is.EqualTo(this.server.ServerId));
			Assert.That(searchAuditBatch.Completed, Is.EqualTo(false));
			Assert.That(searchAuditBatch.BatchStart, Is.EqualTo(0));
			Assert.That(searchAuditBatch.BatchSize, Is.EqualTo(5000));
            Assert.That(this.searchAuditBatch.HourSearchAuditBatchId, Is.EqualTo(this.HourAuditSearchBatchId));
		}

		[Test]
		public void CreateBatchResult()
		{
			// Arrange
			var searchAuditBatchResult = new SearchAuditBatchResult
			{
				BatchId = searchAuditBatch.Id,
				UserId = 1,
				TotalComplexQueries = 11,
				TotalLongRunningQueries = 22,
				TotalQueries = 3
			};
			var batchResults = new List<SearchAuditBatchResult> { searchAuditBatchResult };

			// Act
			var result = this.searchAuditBatchRepository.CreateBatchResults(batchResults);

			// Assert
			Assert.That(result, Is.Not.Empty);
		}

		[Test]
		public void ReadBatch()
		{
			// Act
			var result = this.searchAuditBatchRepository.ReadBatch(searchAuditBatch.Id);

			// Assert
			Assert.That(result, Is.Not.Null);
		}

		[Test]
		public void ReadBatchResults()
		{
			// Act
			var result = this.searchAuditBatchRepository.ReadBatchResults(searchAuditBatch.Id);

			// Assert
			Assert.That(result, Is.Not.Empty);
		}

		[Test]
		public async Task ExistsForHourAndServer()
		{
			// Act
			var result = await this.searchAuditBatchRepository.ExistsForHourAndServer(hour.Id, server.ServerId);

			// Assert
			Assert.That(result, Is.False);
		}

		[Test]
		public void ReadByHourAndServer()
		{
			// Act
			var result = this.searchAuditBatchRepository.ReadByHourAndServer(hour.Id, server.ServerId);

			// Assert
			Assert.That(result, Is.Not.Empty);
		}

		[Test]
		public async Task CreateHourSearchAuditBatch()
		{
			// Act
			await this.searchAuditBatchRepository.CreateHourSearchAuditBatch(hour.Id, server.ServerId, 123);

			// Assert
			Assert.Pass("Created hour search audit batch with no returned result");
		}

		[Test]
		public async Task Update_SearchAuditBatch()
		{
			// Arrange
			await this.searchAuditBatchRepository.CreateBatches(new[] {
			new SearchAuditBatch
			{
				HourSearchAuditBatchId = this.HourAuditSearchBatchId,
				WorkspaceId = 5555,
				BatchStart = 0,
				BatchSize = 5000
			}});

			var batch = (await this.searchAuditBatchRepository.ReadBatchesByHourAndServer(hour.Id, server.ServerId)).FirstOrDefault(b => b.WorkspaceId == 5555);

			batch.BatchSize = 1000;
			batch.BatchStart = 123;
			batch.Completed = true;
			batch.WorkspaceId = 2222; // this value shouldn't get changed with update

			// Act
			await this.searchAuditBatchRepository.UpdateAsync(batch);
			var updatedBatch = this.searchAuditBatchRepository.ReadBatch(batch.Id);

			// Assert
			Assert.That(updatedBatch.BatchSize, Is.EqualTo(1000));
			Assert.That(updatedBatch.BatchStart, Is.EqualTo(123));
			Assert.That(updatedBatch.Completed, Is.EqualTo(true));
			Assert.That(updatedBatch.WorkspaceId, Is.EqualTo(5555), "workspace id shouldn't change");
		}

		[Test]
		public async Task SearchAuditBatchRepository_CreateBatches()
		{
			// Arrange
			var batches = Enumerable.Range(0, 10)
				.Select(
					i =>
						new SearchAuditBatch
						{
							ServerId = server.ServerId,
							WorkspaceId = Config.WorkSpaceId,
							BatchStart = i * 1000,
							BatchSize = 1000
						})
				.ToList();

			// Act
			await this.searchAuditBatchRepository.CreateBatches(batches);

			// Assert
			Assert.Pass("Updated hour search audit batch with no returned result");
		}

		[Test]
		[TestCase(100, TestName = "Create audit batches with 100")] // smaller number used to keep test under 1 second
		//[TestCase(100 * 1000, TestName = "Create audit batches with 100,000")] // large test for integration testing
		[Description("Created as response to a bug found in R1 where thousands of cases were being created at once")]
		public async Task SearchAuditBatchRepository_CreateBatches_LargeNumberOfBatches(int numberOfBatches)
		{
			// Arrange
			var batches = Enumerable.Range(0, numberOfBatches)
				.Select(
					i =>
						new SearchAuditBatch
						{
							ServerId = server.ServerId,
							WorkspaceId = Config.WorkSpaceId,
							BatchStart = i * 1000,
							BatchSize = 1000
						})
				.ToList();

			// Act
			await this.searchAuditBatchRepository.CreateBatches(batches);

			// Assert
			//Assert.That(result.Count, Is.EqualTo(numberOfBatches));
			Assert.Pass("Created search audit batches with no returned result");
		}

	    [Test]
	    public async Task SearchAuditBatchRepository_DeleteBatchByHourBatchId()
	    {
            // Arrange
	        var initReadResult = this.searchAuditBatchRepository.ReadBatch(this.searchAuditBatch.Id);
	        Assert.That(initReadResult.Id, Is.Not.Zero);

            this.CreateBatchResult();
            var initResultReadResult = this.searchAuditBatchRepository.ReadBatchResults(this.searchAuditBatch.Id);
	        Assert.That(initResultReadResult, Is.Not.Empty);

            // Act
	        await this.searchAuditBatchRepository.DeleteAllBatchesAsync(this.HourAuditSearchBatchId);

            // Assert
	        var readResult = this.searchAuditBatchRepository.ReadBatch(this.searchAuditBatch.Id);
	        Assert.That(readResult, Is.Null);
	        var resultReadResult = this.searchAuditBatchRepository.ReadBatchResults(this.searchAuditBatch.Id);
	        Assert.That(resultReadResult, Is.Empty);
        }
	}
}
