namespace kCura.PDB.Data.Tests.Repositories
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Data.Repositories;
	using NUnit.Framework;
	using PDB.Tests.Common;

	[TestFixture]
	[Category("UnitPlatform")]
	[Category("Integration")] // For TeamCity to ignore this test
	public class EventLockRepositoryTests
	{
		private EventLockRepository eventLockRepository;
		private Event eventToLock;
		private IList<EventLock> testLocks;
		private EventWorkerRepository eventWorkerRepository;
		private EventWorker eventWorker;

		[OneTimeSetUp]
		public async Task SetUp()
		{
			this.testLocks = new List<EventLock>();
			this.eventLockRepository = new EventLockRepository(ConnectionFactorySetup.ConnectionFactory);
			this.eventWorkerRepository = new EventWorkerRepository(ConnectionFactorySetup.ConnectionFactory);

			var worker = new EventWorker { Id = 1234588, Name = "Delete me. Create from tests", Type = EventWorkerType.Other };
			this.eventWorker = await this.eventWorkerRepository.CreateAsync(worker);
		}

		[OneTimeTearDown]
		public async Task TearDown()
		{
			if (this.testLocks.Any())
			{
				await this.testLocks
					.Where(tl => tl != null)
					.Select(async testLock => await this.eventLockRepository.Release(testLock))
					.WhenAllStreamed();
			}

			if (this.eventWorker != null)
			{
				await this.eventWorkerRepository.DeleteAsync(this.eventWorker);
			}
		}

		[Test]
		[TestCase(12)]
		[TestCase(null)]
		public async Task Claim_Once_Success(int? sourceId)
		{
			// Arrange
			eventToLock = new Event
			{
				Id = 123,
				SourceType = EventSourceType.ScoreHour,
				SourceId = sourceId,
			};

			// Act
			var testLock = await this.eventLockRepository.Claim(eventToLock, this.eventWorker.Id);
			this.testLocks.Add(testLock);

			// Assert
			Assert.That(testLock.EventId, Is.EqualTo(eventToLock.Id));
			Assert.That(testLock.SourceId, Is.EqualTo(eventToLock.SourceId));
			Assert.That(testLock.EventTypeId, Is.EqualTo(eventToLock.SourceTypeId));
		}

		[Test]
		[TestCase(12)]
		[TestCase(null)]
		public async Task Claim_Twice_Success(int? sourceId)
		{
			// Arange
			eventToLock = new Event
			{
				Id = 123,
				SourceType = EventSourceType.HourCleanup,
				SourceId = sourceId
			};

			var testLock = await this.eventLockRepository.Claim(eventToLock, this.eventWorker.Id);
			this.testLocks.Add(testLock);
			var testEvent = new Event
			{
				Id = eventToLock.Id + 1,
				SourceType = eventToLock.SourceType,
				SourceId = eventToLock.SourceId
			};

			// Act
			var result = await this.eventLockRepository.Claim(testEvent, this.eventWorker.Id);

			// Assert
			Assert.That(result, Is.Null);
		}
	}
}
