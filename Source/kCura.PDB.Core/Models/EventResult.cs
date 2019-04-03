namespace kCura.PDB.Core.Models
{
	using System;
	using System.Collections.Generic;

	public class EventResult
	{
		public EventResult(int sourceId, int? delay = null)
			: this(new[] { sourceId }, new EventSourceType[0], delay)
		{
		}

		public EventResult(IList<int> sourceIds, int? delay = null)
			: this(sourceIds, null, delay)
		{
		}

		public EventResult(EventSourceType type, int? delay = null)
			: this(null, new[] { type }, delay)
		{
		}

		public EventResult(EventSourceType[] types, int? delay = null)
			: this(null, types, delay)
		{
		}

		public EventResult(int sourceId, EventSourceType type, int? delay = null)
			: this(new[] { sourceId }, new[] { type }, delay)
		{
		}

		public EventResult(int sourceId, IList<EventSourceType> types, int? delay = null)
			: this(new[] { sourceId }, types, delay)
		{
		}

		public EventResult(IList<int> sourceIds, EventSourceType type, int? delay = null)
			: this(sourceIds, new[] { type }, delay)
		{
		}

		public EventResult(IList<int> sourceIds, IList<EventSourceType> types, int? delay = null)
		{
			this.SourceIds = sourceIds;
			this.Types = types;
			this.Delay = delay;
			this.Succeeded = true;
		}

		protected EventResult(bool succeeded)
		{
			this.Succeeded = succeeded;
			this.SourceIds = null;
			this.Types = null;
		}

		public IList<int> SourceIds { get; set; }

		public int? Delay { get; set; }

		/// <summary>
		/// Gets or sets the number of milliseconds that elapsed while processing an event.
		/// </summary>

		public int? ExecutionTime { get; set; }

		public IList<EventSourceType> Types { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the event completed successfully.
		/// </summary>
		public bool Succeeded { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the subscribers (next events) should be fired off. Many times if the event is not successful it will not continue either.
		/// </summary>
		public bool ShouldContinue { get; protected set; } = true;

		/// <summary>
		/// Gets a default result indicating to continue on to subscribers (next events)
		/// </summary>
		public static EventResult Continue => _continue;

		/// <summary>
		/// Gets a default result indicating to NOT continue on to subscribers (next events)
		/// </summary>
		public static EventResult Stop => _stop;

		private static EventResult _stop => new EventResult(true) { ShouldContinue = false };

		private static EventResult _continue => new EventResult(true);
	}
}
