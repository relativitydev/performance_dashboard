namespace kCura.PDB.Core.Interfaces.Queuing
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Models;

	public interface IEventRunner
	{
		Task<EventResult> HandleEvent<T>(Func<T, Task> wrappedExpression, Event evnt);

		Task<EventResult> HandleEvent<T>(Func<T, Task<int>> wrappedExpression, Event evnt);

		Task<EventResult> HandleEvent<T>(Func<T, int> wrappedExpression, Event evnt);

		Task<EventResult> HandleEvent<T>(Func<T, Task<IList<int>>> wrappedExpression, Event evnt);

		Task<EventResult> HandleEvent<T>(Func<T, Task<EventResult>> wrappedExpression, Event evnt);

		Task<EventResult> HandleEvent<T>(Func<T, Task<bool>> wrappedExpression, Event evnt);

		Task<EventResult> HandleEvent<T>(Action<T> wrappedExpression, Event evnt);
	}
}
