namespace kCura.PDB.Service.Queuing
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Interfaces.Queuing;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Service.Logging;
	using Ninject;

	public class EventRunner : IEventRunner
	{
		public EventRunner(IKernel kernel)
		{
			this.kernel = kernel;
		}

		private readonly IKernel kernel;

		public Task<EventResult> HandleEvent<T>(Func<T, Task> wrappedExpression, Event evnt)
		{
			return this.GenericHandleEvent<T>(async service =>
			{
				await wrappedExpression.Invoke(service);
				return EventResult.Continue;
			},
			evnt);
		}

		public Task<EventResult> HandleEvent<T>(Func<T, Task<int>> wrappedExpression, Event evnt)
		{
			return this.GenericHandleEvent<T>(async service =>
			{
				var result = await wrappedExpression.Invoke(service);
				return new EventResult(result);
			},
			evnt);
		}

		public Task<EventResult> HandleEvent<T>(Func<T, int> wrappedExpression, Event evnt)
		{
			return this.GenericHandleEvent<T>(service =>
			{
				var result = wrappedExpression.Invoke(service);
				return Task.FromResult(new EventResult(result));
			},
			evnt);
		}

		public Task<EventResult> HandleEvent<T>(Func<T, Task<IList<int>>> wrappedExpression, Event evnt)
		{
			return this.GenericHandleEvent<T>(async service =>
			{
				var result = await wrappedExpression.Invoke(service);
				return new EventResult(result);
			},
			evnt);
		}

		public Task<EventResult> HandleEvent<T>(Func<T, Task<EventResult>> wrappedExpression, Event evnt)
		{
			return this.GenericHandleEvent<T>(async service => await wrappedExpression.Invoke(service), evnt);
		}

		public Task<EventResult> HandleEvent<T>(Action<T> wrappedExpression, Event evnt)
		{
			return this.GenericHandleEvent<T>(service =>
			{
				wrappedExpression.Invoke(service);
				return Task.FromResult(EventResult.Continue);
			},
			evnt);
		}

		public Task<EventResult> HandleEvent<T>(Func<T, Task<bool>> wrappedExpression, Event evnt)
		{
			return this.GenericHandleEvent<T>(async service =>
			{
				var shouldContinue = await wrappedExpression.Invoke(service);
				return EventConstants.LoopEventTypes.Contains(evnt.SourceType)
				? new LoopEventResult(shouldContinue)
				: shouldContinue ? EventResult.Continue : EventResult.Stop;
			}, evnt);
		}

		internal async Task<EventResult> GenericHandleEvent<T>(Func<T, Task<EventResult>> wrappedExpression, Event evnt)
		{
			var service = (T)kernel.Get(typeof(T));
			return await wrappedExpression(service);
		}
	}
}
