namespace kCura.PDB.Service.Tests.Queuing
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Interfaces.Queuing;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Service.Queuing;
	using NUnit.Framework;

	[TestFixture, Category("Unit")]
	public class EventRouterTests
	{
		[SetUp]
		public void Setup()
		{
			this.eventRunner = new MockEventRunner();
			this.eventRouter = new EventRouter(this.eventRunner);
		}

		private IEventRunner eventRunner;
		private EventRouter eventRouter;

		public static IEnumerable<Tuple<EventSourceType, Event>> Events
		{
			get
			{
				return EventConstants.AllEventTypes
					.Select(type => new Tuple<EventSourceType, Event>(type, new Event { Id = 123, SourceType = type, SourceId = 555 }));
			}
		}

		[TestCaseSource(nameof(Events))]
		public async Task EventTaskFactory_RouteEvent_AllEventsAreConfigured(Tuple<EventSourceType, Event> evntt)
		{
			//Arrange
			var evnt = evntt.Item2;

			//Act

			var result = await this.eventRouter.RouteEvent(evnt);

			//Assert
			Assert.That(result, Is.Not.Null);
		}

		[TestCaseSource(nameof(Events))]
		public void EventConstants_EventHierarchies(Tuple<EventSourceType, Event> evntt)
		{
			//Arrange
			var evnt = evntt.Item2;

			//Act
			var result = EventConstants.GetNextEvents(evnt.SourceType);

			//Assert
			Assert.That(result, Is.Not.Null, $"The hierarchy should never return null {evnt.SourceType}");
		}

		private class MockEventRunner : IEventRunner
		{
			public Task<EventResult> HandleEvent<T>(Func<T, Task> wrappedExpression, Event evnt)
			{
				return Task.FromResult(new EventResult(1));
			}

			public Task<EventResult> HandleEvent<T>(Func<T, Task<int>> wrappedExpression, Event evnt)
			{
				return Task.FromResult(new EventResult(1));
			}

			public Task<EventResult> HandleEvent<T>(Func<T, Task<IList<int>>> wrappedExpression, Event evnt)
			{
				return Task.FromResult(new EventResult(1));
			}

			public Task<EventResult> HandleEvent<T>(Func<T, Task<EventResult>> wrappedExpression, Event evnt)
			{
				return Task.FromResult(new EventResult(1));
			}

			public Task<EventResult> HandleEvent<T>(Func<T, Task<bool>> wrappedExpression, Event evnt)
			{
				return Task.FromResult(new EventResult(1));
			}

			public Task<EventResult> HandleEvent<T>(Action<T> wrappedExpression, Event evnt)
			{
				return Task.FromResult(new EventResult(1));
			}

			public Task<EventResult> HandleEvent<T>(Func<T, int> wrappedExpression, Event evnt)
			{
				return Task.FromResult(new EventResult(1));
			}
		}
	}
}
