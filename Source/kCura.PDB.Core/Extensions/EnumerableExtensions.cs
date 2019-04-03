namespace kCura.PDB.Core.Extensions
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;

	public static class EnumerableExtensions
	{
		public static IEnumerable<T> ForEach<T>(this IEnumerable<T> value, Action<T> func)
		{
			foreach (var val in value)
			{
				func(val);
			}

			return value;
		}

		private const int DefaultMaxBufferSize = 8;

		/// <summary>
		/// Executes a Enumerable of tasks in a stream-like fashion. Another way to think of it is the tasks are executed in a staggered manner rather than all at once. The underlying buffer executing the tasks will not exceed the maxStreamSize
		/// </summary>
		/// <typeparam name="T">Type</typeparam>
		/// <param name="tasks">An enumerable of tasks. Note that all the tasks will not be executed at once but will be staggered.</param>
		/// <param name="maxBufferSize">Maximum tasks that are executed in parallel.</param>
		/// <returns>Task of list of results</returns>
		public static async Task<IList<T>> WhenAllStreamed<T>(this IEnumerable<Task<T>> tasks, int maxBufferSize = DefaultMaxBufferSize)
		{
			var buffer = new List<Task<T>>();
			var results = new List<T>();
			foreach (var task in tasks)
			{
				// add a new task to the buffer
				buffer.Add(task);

				// if we're at maximum capacity then we must wait for at least one task to finish
				if (buffer.Count >= maxBufferSize)
				{
					// wait for one task to finish
					var result = await Task.WhenAny(buffer);

					// remove it from the buffer
					buffer.Remove(result);

					// add the result to the final results
					results.Add(await result);
				}
			}

			// wait on any additional items in the buffer
			var remaining = await Task.WhenAll(buffer);
			results.AddRange(remaining);
			return results;
		}

		/// <summary>
		/// Executes a Enumerable of tasks in a stream-like fashion. Another way to think of it is the tasks are executed in a staggered manner rather than all at once. The underlying buffer executing the tasks will not exceed the maxStreamSize
		/// </summary>
		/// <param name="tasks">An enumerable of tasks. Note that all the tasks will not be executed at once but will be staggered.</param>
		/// <param name="maxBufferSize">Maximum tasks that are executed in parallel.</param>
		/// <returns>Task of list of results</returns>
		public static async Task WhenAllStreamed(this IEnumerable<Task> tasks, int maxBufferSize = DefaultMaxBufferSize)
		{
			var buffer = new List<Task>();
			foreach (var task in tasks)
			{
				// add a new task to the buffer
				buffer.Add(task);

				// if we're at maximum capacity then we must wait for at least one task to finish
				if (buffer.Count >= maxBufferSize)
				{
					// wait for one task to finish
					var result = await Task.WhenAny(buffer);

					// remove it from the buffer
					buffer.Remove(result);
				}
			}

			// wait on any additional items in the buffer
			await Task.WhenAll(buffer);
		}

		public static async Task<IList<T>> WhereAsync<T>(this IEnumerable<T> items, Func<T, Task<bool>> predicate)
		{
			var itemTaskList = items.Select(async item => new { Item = item, PredTask = await predicate.Invoke(item) });
			var executed = await WhenAllStreamed(itemTaskList);
			return executed.Where(x => x.PredTask).Select(x => x.Item).ToList();
		}

		public static IEnumerable<T> DistinctBy<T, TKey>(
			this IEnumerable<T> value,
			Func<T, TKey> keySelector)
		{
			return value.DistinctBy(keySelector, null);
		}

		public static IEnumerable<T> DistinctBy<T, TKey>(
			this IEnumerable<T> value,
			Func<T, TKey> keySelector,
			IEqualityComparer<TKey> comparer)
		{
			if (value == null)
			{
				throw new ArgumentNullException("source");
			}

			if (keySelector == null)
			{
				throw new ArgumentNullException("keySelector");
			}

			return DistinctByImpl(value, keySelector, comparer);
		}

		public static string Join<T>(this IEnumerable<T> value, string aggregateFormat)
		{
			return value.Any() ? value.Select(x => x.ToString()).Join(aggregateFormat) : string.Empty;
		}

		public static string Join(this IEnumerable<string> value, string aggregateFormat)
		{
			return value.Any() ? value.Aggregate((x1, x2) => string.Format(aggregateFormat, x1, x2)) : string.Empty;
		}

		private static IEnumerable<T> DistinctByImpl<T, TKey>(
			IEnumerable<T> value,
			Func<T, TKey> keySelector,
			IEqualityComparer<TKey> comparer)
		{
			return value.GroupBy(keySelector, comparer).Select(g => g.First());
		}

		public static IEnumerable<IList<T>> AsBatches<T>(this IList<T> vals, int batchSize)
		{
			return Enumerable
				.Range(0, (int) Math.Ceiling((decimal)vals.Count() / (decimal) batchSize))
				.Select(i => vals.Skip(i * batchSize).Take(batchSize).ToList());
		}

		public static async Task<IEnumerable<TResult>> SelectManyAsync<TSource, TResult>(
			this Task<IEnumerable<TSource>> source,
			Func<TSource, IEnumerable<TResult>> selector)
			=> (await source.ConfigureAwait(false)).SelectMany(selector);

		public static async Task<IEnumerable<TResult>> SelectManyAsync<TSource, TResult>(
			this Task<IList<TSource>> source,
			Func<TSource, IEnumerable<TResult>> selector)
			=> (await source.ConfigureAwait(false)).SelectMany(selector);

		public static async Task<IList<T>> ToListAsync<T>(this Task<IEnumerable<T>> source)
		{
			return (await source.ConfigureAwait(false)).ToList();
		}
	}
}