namespace kCura.PDB.Service.Queuing
{
	using System;
	using System.Linq.Expressions;
	using System.Reflection;
	using Hangfire;
	using kCura.PDB.Core.Interfaces.Queuing;

	public class QueuingService : IQueuingService
	{
		/// <summary>
		/// Enqueues a task that is delayed
		/// </summary>
		/// <typeparam name="T">The service type</typeparam>
		/// <param name="methodCall">The method call on the service to be enqueued</param>
		/// <param name="delay">How long the task is delayed in seconds</param>
		public void Enqueue<T>(Expression<Action<T>> methodCall, int? delay = null)
		{
			var task = delay.HasValue && delay.Value > 0
				? BackgroundJob.Schedule(methodCall, TimeSpan.FromSeconds(delay.Value))
				: BackgroundJob.Enqueue(methodCall);
		}
	}
}
