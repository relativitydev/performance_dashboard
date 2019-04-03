namespace kCura.PDB.Data.Tests.Repositories
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Core.Models;
	using Data.Repositories;
	using NUnit.Framework;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Constants;

	[TestFixture]
	[Category("UnitPlatform")]
	[Category("Integration")] // For TeamCity to ignore this test
	public class EventRepositoryTests
	{
		[OneTimeSetUp]
		public async Task OneTimeSetup()
		{
			var hourRepo = new HourRepository(ConnectionFactorySetup.ConnectionFactory);
			hour = await hourRepo.CreateAsync(new Hour { HourTimeStamp = DateTime.Now });
			this.eventRepository = new EventRepository(ConnectionFactorySetup.ConnectionFactory);
			evnt = await this.eventRepository.CreateAsync(new Event
			{
				Status = EventStatus.Pending,
				HourId = hour.Id,
				TimeStamp = DateTime.Now,
				SourceType = EventSourceType.CreateNextHour,
				SourceId = hour.Id,
				EventHash = "x"
			});
			evnt = await this.eventRepository.ReadAsync(evnt.Id);
			evnt.Retries = (evnt.Retries ?? 0) + 1;
			await this.eventRepository.UpdateAsync(evnt);
			updatedEvent = await this.eventRepository.ReadAsync(evnt.Id);
		}

		private Hour hour;
		private Event evnt;
		private Event updatedEvent;
		private EventRepository eventRepository;

		[Test]
		public void Event_CreateAsync_Success()
		{
			//Assert
			Assert.That(evnt, Is.Not.Null);
			Assert.That(evnt.Id, Is.GreaterThan(0));
		}

		[Test]
		public async Task Event_CreateAsync_Multiple()
		{
			// Arrange
			var events = new[]
			{
				evnt,
				new Event { Status = EventStatus.Completed, HourId = hour.Id, TimeStamp = DateTime.Now, SourceType = EventSourceType.CreateNextHour, SourceId = 987654321 },
				new Event { Status = EventStatus.Completed, HourId = hour.Id, TimeStamp = DateTime.Now, SourceType = EventSourceType.CreateNextHour, SourceId = 987654322 },
			};

			// Act
			await this.eventRepository.CreateAsync(events);

			// Assert
			Assert.Pass();
		}

		[Test]
		public void Event_ReadAsync_ByID_Success()
		{
			//Assert
			Assert.That(evnt, Is.Not.Null);
			Assert.That(evnt.Id, Is.GreaterThan(0));
			Assert.That(evnt.HourId, Is.EqualTo(hour.Id));
			Assert.That(evnt.SourceId, Is.EqualTo(hour.Id));
		}

		[Test]
		public void Event_UpdateAsync_Success()
		{
			// Assert
			Assert.That(updatedEvent.Retries, Is.EqualTo(1));
		}

		[Test]
		public async Task Event_ReadEventSystemStateAsync_Success()
		{
			// Act
			var result = await this.eventRepository.ReadEventSystemStateAsync();

			// Assert
			var possibleStates = Enum.GetValues(typeof(EventSystemState)).Cast<EventSystemState>().ToList();
			Assert.That(possibleStates.Contains(result), Is.True, $"result: {result} should be in possible state");
		}

		[Test]
		public async Task Event_ReadLastBySourceIdAndTypeAsync_WithSourceId()
		{
			// Arrange
			var hourId = 555444;
			var dummyEvent = await this.eventRepository.CreateAsync(new Event { Status = EventStatus.Completed, SourceType = EventSourceType.CheckForHourPrerequisites, SourceId = hourId });

			// Act
			var result = await this.eventRepository.ReadLastBySourceIdAndTypeAsync(EventSourceType.CheckForHourPrerequisites, hourId);

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Id, Is.EqualTo(dummyEvent.Id), "The result should be the last event created with type {EventSourceType.CheckForHourPrerequisites} and source id {hourId}. If its not then check that we're not creating another event in another test after the first one with same type and source id.");
		}

		[Test]
		public async Task Event_ReadLastBySourceIdAndTypeAsync_WithoutSourceId()
		{
			// Arrange
			var dummyEvent = await this.eventRepository.CreateAsync(new Event { Status = EventStatus.Completed, SourceType = EventSourceType.CheckForHourPrerequisites, SourceId = null });

			// Act
			var result = await this.eventRepository.ReadLastBySourceIdAndTypeAsync(EventSourceType.CheckForHourPrerequisites, null);

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Id, Is.EqualTo(dummyEvent.Id), "The result should be the last event created with type {EventSourceType.CheckForHourPrerequisites} and null source id. If its not then check that we're not creating another event in another test after the first one with same type and source id.");
		}

		[Test]
		public async Task Event_UpdateReadEventSystemStateAsync_Success()
		{
			// Act
			await this.eventRepository.UpdateReadEventSystemStateAsync(EventSystemState.Prerequisites);

			// Assert
			Assert.Pass("no result");
		}

		[Test]
		public async Task Event_ReadLastAsync_Success()
		{
			// Act
			var result = await this.eventRepository.ReadLastAsync();

			// Assert
			Assert.That(result, Is.Not.Null);
		}


		[Test]
		public async Task Event_ReadCountByStatusAsync_Success()
		{
			// Arrange
			var event1 = await this.eventRepository.CreateAsync(new Event { Status = EventStatus.Pending, SourceType = EventSourceType.CreateNextHour });

			// Act
			var result = await this.eventRepository.ReadCountByStatusAsync(EventStatus.Pending);

			// Assert
			Assert.That(result, Is.GreaterThanOrEqualTo(1));
		}

		[Test]
		public async Task Event_ReadCountByStatusesAsync_Success()
		{
			// Arrange
			var event1 = await this.eventRepository.CreateAsync(new Event { Status = EventStatus.Pending, SourceType = EventSourceType.CreateNextHour });

			// Act
			var result = await this.eventRepository.ReadCountByStatusAsync(new List<EventStatus> { EventStatus.Pending });

			// Assert
			Assert.That(result, Is.GreaterThanOrEqualTo(1));
		}

		[Test]
		public async Task Event_UpdateEventTypesAsync_Success()
		{
			// Act
			await this.eventRepository.UpdateEventTypesAsync();

			// Assert
			Assert.Pass("no result");
		}

		[Test]
		public async Task EventLog_CreateEventLogAsync_Success()
		{
			// Arrange
			var logRepo = new LogRepository(ConnectionFactorySetup.ConnectionFactory);
			var logId = logRepo.Create(new LogEntry { Module = "Log Test", TaskCompleted = @"Create log entry", AgentId = 3, LogLevel = 2 });

			// Act
			await this.eventRepository.CreateEventLogAsync(evnt.Id, logId);

			// Assert
			Assert.Pass("no result");
		}

		[Test]
		public async Task Event_ExistsAsync()
		{
			// Act
			var result = await this.eventRepository.ExistsAsync(evnt.SourceId, evnt.SourceTypeId);

			// Assert
			Assert.That(result, Is.True);
		}

		[Test]
		public async Task Event_ExistsAsync_NullSourceId()
		{
			// Act
			var result = await this.eventRepository.ExistsAsync(null, evnt.SourceTypeId);

			// Assert
			Assert.That(result, Is.False);
		}

		[Test]
		public async Task Event_ExistsAsync_HourId()
		{
			// Act
			var result = await this.eventRepository.ExistsAsync(this.hour.Id, evnt.SourceType, this.evnt.Status);

			// Assert
			Assert.That(result, Is.True);
		}

		[Test]
		public async Task Event_ReadAnyRemainingHourEventsAsync()
		{
			// Act
			var event1 = await this.eventRepository.CreateAsync(new Event { HourId = hour.Id, Status = EventStatus.Pending, SourceType = EventSourceType.CreateNextHour });

			var result = await this.eventRepository.ReadAnyRemainingHourEventsAsync(hour.Id, EventSourceType.CompleteHour, EventStatus.Pending, EventStatus.InProgress, EventStatus.PendingHangfire);

			// Assert
			Assert.That(result, Is.True);
		}

		[Test]
		public async Task Event_CancelEventsAsync()
		{
			// Act
			await this.eventRepository.CancelEventsAsync(
				new[] { EventStatus.Pending, EventStatus.PendingHangfire, EventStatus.InProgress },
				EventConstants.PrerequisiteEvents);
		}

		[Test]
		public async Task Event_UpdateStatusAsync()
		{
			// Arrange
			var event1 = await this.eventRepository.CreateAsync(new Event { Status = EventStatus.Pending, SourceType = EventSourceType.NoOpSimple });

			// Act
			var results = await this.eventRepository.UpdateStatusAsync(new[] { event1.Id }, EventStatus.Completed);

			// Assert
			Assert.That(results.Select(r => r.Id), Contains.Item(event1.Id));
		}

		[Test]
		public async Task Event_UpdateStatusAndRetriesAsync()
		{
			// Arrange
			var event1 = await this.eventRepository.CreateAsync(new Event { Status = EventStatus.Pending, SourceType = EventSourceType.NoOpSimple });

			// Act
			await this.eventRepository.UpdateStatusAndRetriesAsync(new[] { event1.Id }, EventStatus.Completed);

			// Assert
			var event2 = await this.eventRepository.ReadAsync(event1.Id);
			Assert.That(event2.Status, Is.EqualTo(EventStatus.Completed));
		}

		[Test]
		public async Task Event_ReadIdsByStatusAsync()
		{
			// Arrange
			var event1 = await this.eventRepository.CreateAsync(new Event { Status = EventStatus.Pending, SourceType = EventSourceType.NoOpSimple });

			// Act
			var results = await this.eventRepository.ReadIdsByStatusAsync(EventStatus.Pending, int.MaxValue);

			// Assert
			Assert.That(results, Contains.Item(event1.Id));
		}

		[Test]
		public async Task Event_ReadIdsByStatusAsync_WithEventTypeFilters()
		{
			// Arrange
			var event1 = await this.eventRepository.CreateAsync(new Event { SourceType = EventSourceType.CancelHour }); // should be included
			var event2 = await this.eventRepository.CreateAsync(new Event { SourceType = EventSourceType.CancelEvents }); // should be excluded

			// Act
			var results = await this.eventRepository.ReadIdsByStatusAsync(EventStatus.Pending, int.MaxValue, new[] { EventSourceType.CancelEvents });

			// Assert
			Assert.That(results, Contains.Item(event1.Id));
			Assert.That(results.Any(r=>r == event2.Id), Is.False, "Event 2 should be excluded but it's Id was in the result set");
		}

		[Test]
		public async Task Event_ReadTypesByTypeAndStatusAsync()
		{
			// Arrange
			// created events are always automatically set to pending and must be updated.
			var event1 = await this.eventRepository.CreateAsync(new Event { SourceType = EventSourceType.NoOpSimple });
			event1.Status = EventStatus.InProgress;
			await this.eventRepository.UpdateAsync(event1);

			// Act
			var results = await this.eventRepository.ReadTypesByTypeAndStatusAsync(new[] { EventSourceType.NoOpSimple }, new[] { EventStatus.InProgress });

			// Assert
			Assert.That(results, Contains.Item(EventSourceType.NoOpSimple));
		}

		[Test]
		public async Task Event_ReadSingleByTypeAndStatusAsync_FiltersWhenActiveEvent()
		{
			// Arrange
			// clean up event table from previous tests since this test requires that there aren't any other events with this type
			await this.eventRepository.DeleteAllByTypeAsync(EventSourceType.NoOpSimple);

			// Create an event that is in-progress created events are always automatically set to pending and must be updated.
			var event1 = await this.eventRepository.CreateAsync(new Event { SourceType = EventSourceType.NoOpSimple });
			event1.Status = EventStatus.InProgress;
			await this.eventRepository.UpdateAsync(event1);
			// create a second event that is actually pending. This event should not be picked up since there is a active in-progress event (event1)
			var event2 = await this.eventRepository.CreateAsync(new Event { SourceType = EventSourceType.NoOpSimple });

			// Act
			var results = await this.eventRepository.ReadSingleByTypeAndStatusAsync(
				new[] { EventSourceType.NoOpSimple }, EventStatus.Pending, EventConstants.ActiveEventStatuses);

			// Assert
			Assert.That(results, Is.Empty);
		}

		[Test]
		public async Task Event_ReadSingleByTypeAndStatusAsync_NoActiveEvents()
		{
			// Arrange
			// clean up event table from previous tests since this test requires that there aren't any other events with this type
			await this.eventRepository.DeleteAllByTypeAsync(EventSourceType.NoOpSimple);

			// create a event that is actually pending. This event should be picked up since there is no active events
			var event2 = await this.eventRepository.CreateAsync(new Event { SourceType = EventSourceType.NoOpSimple });

			// Act
			var results = await this.eventRepository.ReadSingleByTypeAndStatusAsync(
				new[] { EventSourceType.NoOpSimple }, EventStatus.Pending, EventConstants.ActiveEventStatuses);

			// Assert
			Assert.That(results, Contains.Item(event2.Id));
		}

		[Test]
		public async Task Event_ZDeleteAsync_Success()
		{
			// Act
			await this.eventRepository.DeleteAsync(evnt);

			// Assert
			var readResult = await this.eventRepository.ReadAsync(evnt.Id);
			Assert.That(readResult, Is.Null);
		}
	}
}